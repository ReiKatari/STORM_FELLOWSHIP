using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public enum CallType
{
    Direct1v1,
    DirectVideo,
    GroupVoiceChannel,
    VideoConference
}

public enum CallState
{
    Idle,
    Ringing,
    Connecting,
    Connected,
    Ended
}

public partial class CallSession : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private CallType _type = CallType.Direct1v1;

    [ObservableProperty]
    private CallState _state = CallState.Connected;

    [ObservableProperty]
    private string _title = "1-1 DIRECT CALL";

    [ObservableProperty]
    private User _remoteUser = new();

    [ObservableProperty]
    private User _localUser = new();

    [ObservableProperty]
    private DateTime _startTime = DateTime.Now;

    [ObservableProperty]
    private TimeSpan _duration = TimeSpan.FromMinutes(13) + TimeSpan.FromSeconds(37);

    [ObservableProperty]
    private bool _isMicMuted;

    [ObservableProperty]
    private bool _isDeafened;

    [ObservableProperty]
    private bool _isVideoOn;

    [ObservableProperty]
    private bool _isScreenSharing;

    [ObservableProperty]
    private string _codec = "Opus 48kHz Stereo 128 kbps (Low Latency)";

    [ObservableProperty]
    private int _pingMs = 18;

    [ObservableProperty]
    private double _packetLossPercent = 0.0;

    [ObservableProperty]
    private bool _isRemoteSpeaking = true;

    [ObservableProperty]
    private bool _isLocalSpeaking = false;

    public ObservableCollection<User> Participants { get; } = new();

    public string DurationFormatted => $"{Duration.Minutes:D2}:{Duration.Seconds:D2}";
    public string BottomCallStatus => $"Call ongoing • {DurationFormatted}";
}
