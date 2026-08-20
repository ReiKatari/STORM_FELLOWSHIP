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
    private string _username = "user";

    [ObservableProperty]
    private string _tag = "0001";

    [ObservableProperty]
    private string _displayName = "Пользователь";

    [ObservableProperty]
    private string _avatarPath = "ms-appx:///Assets/Avatars/you.png";

    [ObservableProperty]
    private UserStatus _status = UserStatus.Online;

    [ObservableProperty]
    private string _customStatus = "В сети";

    [ObservableProperty]
    private string _roleName = "Участник";

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
        UserStatus.Offline => "#94A3B8",
        UserStatus.InVoice => "#00A3FF",
        UserStatus.Streaming => "#A855F7",
        _ => "#94A3B8"
    };

    public string StatusText => Status switch
    {
        UserStatus.Online => "В сети",
        UserStatus.Idle => "Отошел",
        UserStatus.DoNotDisturb => "Не беспокоить",
        UserStatus.Offline => "Не в сети",
        UserStatus.InVoice => "В голосовом канале",
        UserStatus.Streaming => "В эфире",
        _ => "Не в сети"
    };
}
