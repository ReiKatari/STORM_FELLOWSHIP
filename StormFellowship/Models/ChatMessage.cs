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
    private string _transcriptionText = string.Empty;

    [ObservableProperty]
    private bool _isTranscribed;

    [ObservableProperty]
    private bool _isTranscribing;

    [ObservableProperty]
    private bool _isE2EEEncrypted = true;

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

    [ObservableProperty]
    private bool _isTranslated;

    [ObservableProperty]
    private string _translatedText = string.Empty;

    [ObservableProperty]
    private string _targetLangCode = string.Empty;

    [ObservableProperty]
    private bool _hasCodeBlock;

    [ObservableProperty]
    private string _codeLanguage = "C#";

    [ObservableProperty]
    private string _codeContent = string.Empty;

    [ObservableProperty]
    private bool _hasSpoiler;

    [ObservableProperty]
    private string _spoilerText = string.Empty;

    [ObservableProperty]
    private bool _isSpoilerRevealed;

    [ObservableProperty]
    private bool _isVoicePlaying;

    [ObservableProperty]
    private double _voicePlaybackProgress;

    [ObservableProperty]
    private double _voicePlaybackSpeed = 1.0;

    public ObservableCollection<double> VoiceWaveformBands { get; } = new();

    [ObservableProperty]
    private bool _hasThread;

    [ObservableProperty]
    private int _threadReplyCount = 0;

    [ObservableProperty]
    private string _threadLastReplyTime = string.Empty;

    [ObservableProperty]
    private bool _hasFileAttachment;

    [ObservableProperty]
    private string _fileName = string.Empty;

    [ObservableProperty]
    private string _fileSizeFormatted = string.Empty;

    [ObservableProperty]
    private string _fileExtension = string.Empty;

    [ObservableProperty]
    private PollItem? _poll;

    public bool IsPoll => Poll != null;

    public ObservableCollection<MessageReaction> Reactions { get; } = new();

    public string FormattedTime => Timestamp.ToString("HH:mm");

    public string FormattedDate => Timestamp.Date == DateTime.Today
        ? $"Сегодня в {Timestamp:HH:mm}"
        : Timestamp.ToString("dd.MM.yyyy HH:mm");
}
