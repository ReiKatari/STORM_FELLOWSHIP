using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public enum ChannelType
{
    Text,
    Voice,
    Announcements,
    VoiceHub,
    TemporaryVoice
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
    private int _bitrateKbps = 128;

    [ObservableProperty]
    private int _userLimit = 0; // 0 = unlimited

    [ObservableProperty]
    private int _unreadCount = 0;

    [ObservableProperty]
    private bool _isMuted = false;

    [ObservableProperty]
    private bool _isTemporary = false;

    [ObservableProperty]
    private string _ownerUserId = string.Empty;

    public ObservableCollection<ChatMessage> Messages { get; } = new();

    public ObservableCollection<User> ConnectedVoiceUsers { get; } = new();

    public string IconGlyph => Type switch
    {
        ChannelType.Text => "#",
        ChannelType.Voice => "🔊",
        ChannelType.Announcements => "📢",
        ChannelType.VoiceHub => "⚡",
        ChannelType.TemporaryVoice => "🎙️",
        _ => "#"
    };

    public string BitrateDisplay => Type == ChannelType.Voice || Type == ChannelType.VoiceHub || Type == ChannelType.TemporaryVoice
        ? $"{BitrateKbps} Кбит/с"
        : string.Empty;

    public bool IsVoice => Type == ChannelType.Voice || Type == ChannelType.VoiceHub || Type == ChannelType.TemporaryVoice;
    public bool IsText => Type == ChannelType.Text || Type == ChannelType.Announcements;
    public bool IsVoiceHub => Type == ChannelType.VoiceHub;
}

public partial class ChannelCategory : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "ТЕКСТОВЫЕ КАНАЛЫ";

    [ObservableProperty]
    private bool _isCollapsed;

    public ObservableCollection<Channel> Channels { get; } = new();
}
