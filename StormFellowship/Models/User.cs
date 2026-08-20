using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public enum UserStatus
{
    Online,
    Idle,
    DoNotDisturb,
    Offline,
    InVoice,
    Streaming
}

public partial class User : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _tag = "0001";

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _avatarPath = "ms-appx:///Assets/Avatars/you.png";

    [ObservableProperty]
    private UserStatus _status = UserStatus.Online;

    [ObservableProperty]
    private string _customStatus = string.Empty;

    [ObservableProperty]
    private string _roleName = "Member";

    [ObservableProperty]
    private string _roleColorHex = "#00A3FF";

    [ObservableProperty]
    private bool _isMuted;

    [ObservableProperty]
    private bool _isDeafened;

    [ObservableProperty]
    private bool _isSpeaking;

    [ObservableProperty]
    private bool _isVideoOn;

    [ObservableProperty]
    private bool _isScreenSharing;

    [ObservableProperty]
    private double _volume = 100.0;

    [ObservableProperty]
    private int _pingMs = 18;

    public string FullUsername => $"@{Username}#{Tag}";

    public string StatusBadgeColor => Status switch
    {
        UserStatus.Online => "#22C55E",
        UserStatus.Idle => "#F59E0B",
        UserStatus.DoNotDisturb => "#EF4444",
        UserStatus.Offline => "#64748B",
        UserStatus.InVoice => "#00A3FF",
        UserStatus.Streaming => "#A855F7",
        _ => "#64748B"
    };

    public string StatusText => Status switch
    {
        UserStatus.Online => "Online",
        UserStatus.Idle => "Away",
        UserStatus.DoNotDisturb => "Do Not Disturb",
        UserStatus.Offline => "Offline",
        UserStatus.InVoice => "In Voice Channel",
        UserStatus.Streaming => "Streaming",
        _ => "Offline"
    };
}
