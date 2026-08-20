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
                // Return or create messages for DM
                return FellowshipService.Instance.Fellowships.FirstOrDefault()?.Categories.FirstOrDefault()?.Channels.FirstOrDefault()?.Messages
                    ?? new ObservableCollection<ChatMessage>();
            }
            return CurrentChannel?.Messages ?? new ObservableCollection<ChatMessage>();
        }
    }

    public ObservableCollection<EmojiItem> Emojis => ChatService.Instance.Emojis;
    public ObservableCollection<StickerItem> Stickers => ChatService.Instance.Stickers;

    public string HeaderTitle => IsDirectMessages
        ? (CurrentDmUser != null ? $"@{CurrentDmUser.DisplayName}" : "Direct Messages")
        : (CurrentChannel != null ? $"#{CurrentChannel.Name}" : "Select a Channel");

    public string HeaderTopic => IsDirectMessages
        ? (CurrentDmUser?.CustomStatus ?? "Direct conversation")
        : (CurrentChannel?.Topic ?? "Storm Fellowship Channel");

    public ChatViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        FellowshipService.Instance.CurrentChannelChanged += (c) =>
        {
            OnPropertyChanged(nameof(CurrentChannel));
            OnPropertyChanged(nameof(CurrentMessages));
            OnPropertyChanged(nameof(HeaderTitle));
            OnPropertyChanged(nameof(HeaderTopic));
        };

        FellowshipService.Instance.CurrentDmUserChanged += (u) =>
        {
            OnPropertyChanged(nameof(CurrentDmUser));
            OnPropertyChanged(nameof(CurrentMessages));
            OnPropertyChanged(nameof(HeaderTitle));
            OnPropertyChanged(nameof(HeaderTopic));
        };
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
        _mainVM.ShowToastNotification(message.IsPinned ? "Message pinned to channel" : "Message unpinned");
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
