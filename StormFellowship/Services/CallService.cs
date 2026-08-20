using System.Timers;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class CallService : ICallService
{
    private static CallService? _instance;
    public static CallService Instance => _instance ??= new CallService();

    private readonly System.Timers.Timer _callTimer;
    private readonly System.Timers.Timer _waveformTimer;
    private readonly Random _random = new();

    public CallSession? ActiveCall { get; private set; }
    public bool IsInCall => ActiveCall != null && ActiveCall.State == CallState.Connected;

    public event Action<CallSession?>? CallStateChanged;
    public event Action<double[]>? WaveformUpdated;

    public CallService()
    {
        _callTimer = new System.Timers.Timer(1000);
        _callTimer.Elapsed += OnCallTimerElapsed;

        _waveformTimer = new System.Timers.Timer(40); // 25 FPS wave visualizer
        _waveformTimer.Elapsed += OnWaveformTimerElapsed;

        // Default initial session: Setup the iconic Sakura & You call matching reference image!
        InitializeDemoCall();
    }

    private void InitializeDemoCall()
    {
        var sakura = new User
        {
            Id = "user_sakura",
            Username = "Sakura",
            DisplayName = "Sakura",
            Tag = "7721",
            AvatarPath = "ms-appx:///Assets/Avatars/sakura.png",
            CustomStatus = "What the bobba",
            Status = UserStatus.InVoice,
            IsSpeaking = true,
            RoleName = "Storm Guard",
            RoleColorHex = "#FF6A88"
        };

        var localUser = new User
        {
            Id = "user_local",
            Username = "You",
            DisplayName = "You",
            Tag = "0001",
            AvatarPath = "ms-appx:///Assets/Avatars/you.png",
            CustomStatus = "Building STORM FELLOWSHIP",
            Status = UserStatus.InVoice,
            IsSpeaking = false,
            RoleName = "Storm Commander",
            RoleColorHex = "#00A3FF"
        };

        ActiveCall = new CallSession
        {
            Type = CallType.Direct1v1,
            State = CallState.Connected,
            Title = "1-1 DIRECT CALLS",
            RemoteUser = sakura,
            LocalUser = localUser,
            StartTime = DateTime.Now.AddMinutes(-13).AddSeconds(-37),
            Duration = TimeSpan.FromMinutes(13) + TimeSpan.FromSeconds(37),
            IsRemoteSpeaking = true,
            IsLocalSpeaking = false,
            PingMs = 18,
            PacketLossPercent = 0.0
        };

        ActiveCall.Participants.Add(sakura);
        ActiveCall.Participants.Add(localUser);

        _callTimer.Start();
        _waveformTimer.Start();
    }

    public void StartDirectCall(User remoteUser, bool isVideo = false)
    {
        AudioService.Instance.PlaySoundCue(SoundCueType.CallStart);

        var localUser = new User
        {
            Id = "user_local",
            Username = "You",
            DisplayName = "You",
            Tag = "0001",
            AvatarPath = "ms-appx:///Assets/Avatars/you.png",
            Status = UserStatus.InVoice,
            RoleName = "Storm Commander"
        };

        ActiveCall = new CallSession
        {
            Type = isVideo ? CallType.DirectVideo : CallType.Direct1v1,
            State = CallState.Connected,
            Title = isVideo ? "1-1 VIDEO CALL" : "1-1 DIRECT CALL",
            RemoteUser = remoteUser,
            LocalUser = localUser,
            StartTime = DateTime.Now,
            Duration = TimeSpan.Zero,
            IsRemoteSpeaking = true,
            IsVideoOn = isVideo
        };

        ActiveCall.Participants.Add(remoteUser);
        ActiveCall.Participants.Add(localUser);

        _callTimer.Start();
        _waveformTimer.Start();

        CallStateChanged?.Invoke(ActiveCall);
    }

    public void JoinVoiceChannel(Channel voiceChannel)
    {
        AudioService.Instance.PlaySoundCue(SoundCueType.UserJoin);

        var localUser = new User
        {
            Id = "user_local",
            Username = "You",
            DisplayName = "You",
            Tag = "0001",
            AvatarPath = "ms-appx:///Assets/Avatars/you.png",
            Status = UserStatus.InVoice,
            RoleName = "Storm Commander"
        };

        ActiveCall = new CallSession
        {
            Type = CallType.GroupVoiceChannel,
            State = CallState.Connected,
            Title = $"🔊 {voiceChannel.Name.ToUpper()}",
            LocalUser = localUser,
            StartTime = DateTime.Now,
            Duration = TimeSpan.Zero,
            Codec = $"Opus {voiceChannel.BitrateKbps} kbps Low-Latency VBR"
        };

        ActiveCall.Participants.Add(localUser);
        foreach (var u in voiceChannel.ConnectedVoiceUsers)
        {
            ActiveCall.Participants.Add(u);
        }

        _callTimer.Start();
        _waveformTimer.Start();

        CallStateChanged?.Invoke(ActiveCall);
    }

    public void EndCall()
    {
        if (ActiveCall != null)
        {
            AudioService.Instance.PlaySoundCue(SoundCueType.CallEnd);
            ActiveCall.State = CallState.Ended;
            _callTimer.Stop();
            _waveformTimer.Stop();
            ActiveCall = null;
            CallStateChanged?.Invoke(null);
        }
    }

    public void ToggleMute()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsMicMuted = !ActiveCall.IsMicMuted;
            AudioService.Instance.IsMuted = ActiveCall.IsMicMuted;
        }
    }

    public void ToggleDeafen()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsDeafened = !ActiveCall.IsDeafened;
            AudioService.Instance.IsDeafened = ActiveCall.IsDeafened;
        }
    }

    public void ToggleVideo()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsVideoOn = !ActiveCall.IsVideoOn;
        }
    }

    public void ToggleScreenShare()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsScreenSharing = !ActiveCall.IsScreenSharing;
        }
    }

    private void OnCallTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (ActiveCall != null)
        {
            ActiveCall.Duration = DateTime.Now - ActiveCall.StartTime;
            ActiveCall.IsRemoteSpeaking = (_random.NextDouble() > 0.3);
            ActiveCall.IsLocalSpeaking = !ActiveCall.IsMicMuted && (_random.NextDouble() > 0.7);
        }
    }

    private void OnWaveformTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (ActiveCall == null) return;

        // Generate 7-bar dynamic audio levels for voice indicator waveform (between Sakura and You)
        double[] bars = new double[7];
        bool speaking = ActiveCall.IsRemoteSpeaking || ActiveCall.IsLocalSpeaking;
        
        for (int i = 0; i < 7; i++)
        {
            if (speaking)
            {
                double centerDist = Math.Abs(3 - i);
                double baseHeight = (4.0 - centerDist) * 5.0;
                bars[i] = Math.Clamp(baseHeight + _random.Next(2, 18), 4.0, 32.0);
            }
            else
            {
                bars[i] = 4.0;
            }
        }

        WaveformUpdated?.Invoke(bars);
    }
}
