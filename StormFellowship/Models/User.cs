using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public enum UserStatus
{
    Online,
    Idle,
    DoNotDisturb,
    Invisible,
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
    private string _avatarGlyph = "⚡";

    [ObservableProperty]
    private string _avatarPath = string.Empty;

    [ObservableProperty]
    private UserStatus _status = UserStatus.Online;

    [ObservableProperty]
    private string _customStatus = "В сети";

    [ObservableProperty]
    private string _roleName = "Создатель";

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
    private bool _isMutedForMe = false;

    [ObservableProperty]
    private bool _isPrioritySpeaker = false;

    // Spatial 3D Audio Virtual Coordinates
    [ObservableProperty]
    private double _spatialX = 0.0; // -100 to +100 (Left / Right)

    [ObservableProperty]
    private double _spatialY = 0.0; // -100 to +100 (Behind / In Front)

    [ObservableProperty]
    private double _spatialZ = 0.0; // Height

    [ObservableProperty]
    private int _pingMs = 18;

    [ObservableProperty]
    private string _e2eeSafetyNumber = "7829-4912-3391-8842-1940-5521";

    public bool HasCustomAvatarImage => !string.IsNullOrWhiteSpace(AvatarPath);

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

    partial void OnAvatarPathChanged(string value)
    {
        OnPropertyChanged(nameof(HasCustomAvatarImage));
    }
}
