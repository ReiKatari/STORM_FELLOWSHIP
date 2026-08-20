using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public partial class MessageReaction : ObservableObject
{
    [ObservableProperty]
    private string _emojiCode = string.Empty;

    [ObservableProperty]
    private string _emojiSymbol = string.Empty;

    [ObservableProperty]
    private int _count = 1;

    [ObservableProperty]
    private bool _hasReacted;

    public ObservableCollection<string> UserIds { get; } = new();
}

public partial class ChatMessage : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _channelId = string.Empty;

    [ObservableProperty]
    private User _author = new();

    [ObservableProperty]
    private string _content = string.Empty;

    [ObservableProperty]
    private DateTime _timestamp = DateTime.Now;

    [ObservableProperty]
    private bool _isEdited;

    [ObservableProperty]
    private bool _isPinned;

    [ObservableProperty]
    private bool _isVoiceNote;

    [ObservableProperty]
    private int _voiceNoteDurationSeconds;

    [ObservableProperty]
    private string _replyToAuthor = string.Empty;

    [ObservableProperty]
    private string _replyToContent = string.Empty;

    [ObservableProperty]
    private bool _hasReply;

    [ObservableProperty]
    private string _attachmentUrl = string.Empty;

    [ObservableProperty]
    private bool _hasAttachment;

    [ObservableProperty]
    private string _stickerUrl = string.Empty;

    [ObservableProperty]
    private bool _hasSticker;

    public ObservableCollection<MessageReaction> Reactions { get; } = new();

    public string FormattedTime => Timestamp.ToString("HH:mm");

    public string FormattedDate => Timestamp.Date == DateTime.Today
        ? $"Today at {Timestamp:HH:mm}"
        : Timestamp.ToString("dd.MM.yyyy HH:mm");
}
