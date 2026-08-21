using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

[Flags]
public enum RolePermissions
{
    None = 0,
    SendMessages = 1 << 0,
    AttachFiles = 1 << 1,
    ConnectVoice = 1 << 2,
    Speak = 1 << 3,
    Video = 1 << 4,
    ScreenShare = 1 << 5,
    PrioritySpeaker = 1 << 6,
    MuteMembers = 1 << 7,
    DeafenMembers = 1 << 8,
    MoveMembers = 1 << 9,
    ManageChannels = 1 << 10,
    ManageRoles = 1 << 11,
    ManageFellowship = 1 << 12,
    Administrator = 1 << 13
}

public partial class Role : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "Участник";

    [ObservableProperty]
    private string _colorHex = "#94A3B8";

    [ObservableProperty]
    private int _priority = 0;

    [ObservableProperty]
    private RolePermissions _permissions = RolePermissions.SendMessages | RolePermissions.AttachFiles | RolePermissions.ConnectVoice | RolePermissions.Speak | RolePermissions.Video;

    [ObservableProperty]
    private bool _isHoisted = true;

    [ObservableProperty]
    private bool _isMentionable = true;

    public bool HasPermission(RolePermissions perm) => (Permissions & perm) == perm;
}
