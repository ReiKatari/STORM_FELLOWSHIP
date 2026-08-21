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

    public double CallVolume { get; set; } = 100.0;

    public event Action<CallSession?>? CallStateChanged;
    public event Action<double[]>? WaveformUpdated;

    public CallService()
    {
        _callTimer = new System.Timers.Timer(1000);
        _callTimer.Elapsed += OnCallTimerElapsed;

        _waveformTimer = new System.Timers.Timer(40); // 25 FPS wave visualizer
        _waveformTimer.Elapsed += OnWaveformTimerElapsed;
    }

    public void StartDirectCall(User remoteUser, bool isVideo = false)
    {
        AudioService.Instance.PlaySoundCue(SoundCueType.CallStart);

        var localUser = FellowshipService.Instance.CurrentUser;

        ActiveCall = new CallSession
        {
            Type = isVideo ? CallType.DirectVideo : CallType.Direct1v1,
            State = CallState.Connected,
            Title = isVideo ? "ПРЯМОЙ ВИДЕОЗВОНОК" : "ПРЯМОЙ ВЫЗОВ",
            RemoteUser = remoteUser,
            LocalUser = localUser,
            StartTime = DateTime.Now,
            Duration = TimeSpan.Zero,
            IsRemoteSpeaking = true,
            IsVideoOn = isVideo,
            PingMs = 16,
            PacketLossPercent = 0.0,
            Codec = "Opus 128 kbps Low-Latency VBR"
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

        var localUser = FellowshipService.Instance.CurrentUser;

        ActiveCall = new CallSession
        {
            Type = CallType.GroupVoiceChannel,
            State = CallState.Connected,
            Title = voiceChannel.Name.ToUpper(),
            LocalUser = localUser,
            RemoteUser = new User
            {
                DisplayName = "Голосовая комната",
                CustomStatus = $"{voiceChannel.BitrateKbps} Кбит/с",
                AvatarGlyph = "🔊"
            },
            StartTime = DateTime.Now,
            Duration = TimeSpan.Zero,
            Codec = $"Opus {voiceChannel.BitrateKbps} Кбит/с Сверхнизкая задержка",
            PingMs = 14,
            PacketLossPercent = 0.0
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
            CameraService.Instance.StopCamera();
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
            FellowshipService.Instance.CurrentUser.IsMuted = ActiveCall.IsMicMuted;
        }
        else
        {
            AudioService.Instance.IsMuted = !AudioService.Instance.IsMuted;
            FellowshipService.Instance.CurrentUser.IsMuted = AudioService.Instance.IsMuted;
        }
    }

    public void ToggleDeafen()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsDeafened = !ActiveCall.IsDeafened;
            AudioService.Instance.IsDeafened = ActiveCall.IsDeafened;
            FellowshipService.Instance.CurrentUser.IsDeafened = ActiveCall.IsDeafened;
        }
        else
        {
            AudioService.Instance.IsDeafened = !AudioService.Instance.IsDeafened;
            FellowshipService.Instance.CurrentUser.IsDeafened = AudioService.Instance.IsDeafened;
        }
    }

    public void ToggleVideo()
    {
        if (ActiveCall != null)
        {
            ActiveCall.IsVideoOn = !ActiveCall.IsVideoOn;
            if (ActiveCall.IsVideoOn)
            {
                CameraService.Instance.StartCamera();
            }
            else
            {
                CameraService.Instance.StopCamera();
            }
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
            ActiveCall.IsRemoteSpeaking = (_random.NextDouble() > 0.35);
            ActiveCall.IsLocalSpeaking = !AudioService.Instance.IsMuted && (_random.NextDouble() > 0.65);
        }
    }

    private void OnWaveformTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (ActiveCall == null) return;

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
