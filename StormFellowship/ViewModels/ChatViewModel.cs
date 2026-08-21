using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    [ObservableProperty]
    private string _messageInputText = string.Empty;

    [ObservableProperty]
    private ChatMessage? _replyingToMessage;

    [ObservableProperty]
    private bool _isEmojiPickerOpen;

    [ObservableProperty]
    private bool _isStickerPickerOpen;

    [ObservableProperty]
    private string _searchFilter = string.Empty;

    public Channel? CurrentChannel => FellowshipService.Instance.CurrentChannel;
    public User? CurrentDmUser => FellowshipService.Instance.CurrentDmUser;
    public bool IsDirectMessages => FellowshipService.Instance.IsDirectMessagesSelected;

    public ObservableCollection<ChatMessage> CurrentMessages
    {
        get
        {
            if (IsDirectMessages && CurrentDmUser != null)
            {
                return FellowshipService.Instance.Fellowships.FirstOrDefault()?.Categories.FirstOrDefault()?.Channels.FirstOrDefault()?.Messages
                    ?? new ObservableCollection<ChatMessage>();
            }
            return CurrentChannel?.Messages ?? new ObservableCollection<ChatMessage>();
        }
    }

    public ObservableCollection<ChatMessage> Messages => CurrentMessages;
    public string ChannelName => HeaderTitle;
    public string ChannelTopic => HeaderTopic;

    public ObservableCollection<EmojiItem> Emojis => ChatService.Instance.Emojis;
    public ObservableCollection<StickerItem> Stickers => ChatService.Instance.Stickers;

    public string HeaderTitle => IsDirectMessages
        ? (CurrentDmUser != null ? $"@{CurrentDmUser.DisplayName}" : "Личные сообщения")
        : (CurrentChannel != null ? $"{CurrentChannel.Name}" : "общий");

    public string HeaderTopic => IsDirectMessages
        ? (CurrentDmUser?.CustomStatus ?? "Личная переписка • E2EE Защищено")
        : (CurrentChannel?.Topic ?? "Основной чат содружества • E2EE Защищено");

    public ChatViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        FellowshipService.Instance.CurrentChannelChanged += (c) =>
        {
            RefreshProperties();
        };

        FellowshipService.Instance.CurrentDmUserChanged += (u) =>
        {
            RefreshProperties();
        };
    }

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(CurrentChannel));
        OnPropertyChanged(nameof(CurrentDmUser));
        OnPropertyChanged(nameof(CurrentMessages));
        OnPropertyChanged(nameof(Messages));
        OnPropertyChanged(nameof(HeaderTitle));
        OnPropertyChanged(nameof(HeaderTopic));
        OnPropertyChanged(nameof(ChannelName));
        OnPropertyChanged(nameof(ChannelTopic));
    }

    [RelayCommand]
    public void ToggleMemberList()
    {
        _mainVM.ToggleMemberList();
    }

    [RelayCommand]
    public void OpenSearch()
    {
        _mainVM.OpenSearchDialog();
    }

    [RelayCommand]
    public void OpenCreatePoll()
    {
        _mainVM.OpenCreatePollDialog();
    }

    [RelayCommand]
    public void OpenE2EEDialog()
    {
        _mainVM.OpenE2EESecurityDialog();
    }

    [RelayCommand]
    public async Task TranscribeVoiceNote(ChatMessage message)
    {
        if (message == null || message.IsTranscribing) return;
        message.IsTranscribing = true;
        _mainVM.ShowToastNotification("🤖 Whisper AI: Обработка нейросетью...");
        var text = await WhisperTranscriptionService.Instance.TranscribeAudioAsync(message.Id, message.VoiceNoteDurationSeconds);
        message.TranscriptionText = text;
        message.IsTranscribed = true;
        message.IsTranscribing = false;
        _mainVM.ShowToastNotification("🤖 Whisper AI: Текст успешно расшифрован!");
    }

    [RelayCommand]
    public void SpeakMessage(ChatMessage message)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Content)) return;
        AudioService.Instance.SpeakText(message.Content);
        _mainVM.ShowToastNotification($"🔊 Озвучивание: {message.Author.DisplayName}");
    }

    [RelayCommand]
    public void CopyMessageText(ChatMessage message)
    {
        if (message == null || string.IsNullOrWhiteSpace(message.Content)) return;
        try
        {
            System.Windows.Clipboard.SetText(message.Content);
            _mainVM.ShowToastNotification("📋 Текст сообщения скопирован в буфер обмена");
        }
        catch { }
    }

    [RelayCommand]
    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(MessageInputText)) return;

        var channelId = CurrentChannel?.Id ?? "dm_channel";
        var msg = ChatService.Instance.SendMessage(channelId, MessageInputText, ReplyingToMessage);
        
        CurrentMessages.Add(msg);

        MessageInputText = string.Empty;
        ReplyingToMessage = null;
        RefreshProperties();
    }

    [RelayCommand]
    public void InsertEmoji(EmojiItem emoji)
    {
        MessageInputText += $" {emoji.Symbol} ";
        IsEmojiPickerOpen = false;
    }

    [RelayCommand]
    public void SendSticker(StickerItem sticker)
    {
        var channelId = CurrentChannel?.Id ?? "dm_channel";
        var msg = ChatService.Instance.SendMessage(channelId, "", ReplyingToMessage, stickerUrl: sticker.ImagePath);
        CurrentMessages.Add(msg);
        IsStickerPickerOpen = false;
        ReplyingToMessage = null;
        RefreshProperties();
    }

    [RelayCommand]
    public void ToggleReaction(object[] parameters)
    {
        if (parameters.Length >= 3 && parameters[0] is ChatMessage msg && parameters[1] is string symbol && parameters[2] is string code)
        {
            ChatService.Instance.ToggleReaction(msg, symbol, code);
        }
    }

    [RelayCommand]
    public void ReplyTo(ChatMessage message)
    {
        ReplyingToMessage = message;
    }

    [RelayCommand]
    public void CancelReply()
    {
        ReplyingToMessage = null;
    }

    [RelayCommand]
    public void PinMessage(ChatMessage message)
    {
        ChatService.Instance.PinMessage(message);
        _mainVM.ShowToastNotification(message.IsPinned ? "Сообщение закреплено" : "Сообщение откреплено");
    }

    [RelayCommand]
    public void DeleteMessage(ChatMessage message)
    {
        if (CurrentChannel != null)
        {
            ChatService.Instance.DeleteMessage(CurrentChannel, message);
        }
        else
        {
            CurrentMessages.Remove(message);
        }
    }

    [RelayCommand]
    public void StartDirectAudioCall()
    {
        var targetUser = CurrentDmUser ?? FellowshipService.Instance.DirectMessageUsers.FirstOrDefault();
        if (targetUser != null)
        {
            CallService.Instance.StartDirectCall(targetUser, isVideo: false);
        }
    }

    [RelayCommand]
    public void StartDirectVideoCall()
    {
        var targetUser = CurrentDmUser ?? FellowshipService.Instance.DirectMessageUsers.FirstOrDefault();
        if (targetUser != null)
        {
            CallService.Instance.StartDirectCall(targetUser, isVideo: true);
        }
    }
}
