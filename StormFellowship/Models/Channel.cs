using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public enum ChannelType
{
    Text,
    Voice,
    Announcements
}

public partial class Channel : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "general";

    [ObservableProperty]
    private string _topic = string.Empty;

    [ObservableProperty]
    private ChannelType _type = ChannelType.Text;

    [ObservableProperty]
    private int _bitrateKbps = 96;

    [ObservableProperty]
    private int _userLimit = 0; // 0 = unlimited

    [ObservableProperty]
    private int _unreadCount = 0;

    [ObservableProperty]
    private bool _isMuted = false;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public ObservableCollection<User> ConnectedVoiceUsers { get; } = new();

    public string IconGlyph => Type switch
    {
        ChannelType.Text => "#",
        ChannelType.Voice => "🔊",
        ChannelType.Announcements => "📢",
        _ => "#"
    };

    public bool IsVoice => Type == ChannelType.Voice;
    public bool IsText => Type == ChannelType.Text || Type == ChannelType.Announcements;
}

public partial class ChannelCategory : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "TEXT CHANNELS";

    [ObservableProperty]
    private bool _isCollapsed;

    public ObservableCollection<Channel> Channels { get; } = new();
}
