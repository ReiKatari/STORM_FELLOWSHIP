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
    private bool _isRecordingVoice = false;

    [ObservableProperty]
    private string _voiceRecordingDuration = "00:00";

    [ObservableProperty]
    private bool _isEmotePickerOpen = false;

    private System.Windows.Threading.DispatcherTimer? _voiceTimer;
    private int _voiceSeconds = 0;

    public bool IsGlassBubblesMode => _mainVM.IsGlassBubblesMode;

    public Channel? CurrentChannel => FellowshipService.Instance.CurrentChannel;
    public User? CurrentDmUser => FellowshipService.Instance.CurrentDmUser;
    public bool IsDirectMessages => FellowshipService.Instance.IsDirectMessagesSelected;

    public ObservableCollection<ChatMessage> CurrentMessages
    {
        get
        {
            if (IsDirectMessages)
            {
                var dmUserId = CurrentDmUser?.Id ?? "user_bot";
                return FellowshipService.Instance.GetDirectMessages(dmUserId);
            }
            return CurrentChannel?.Messages ?? FellowshipService.Instance.GetDirectMessages("user_bot");
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
        OnPropertyChanged(nameof(IsGlassBubblesMode));
    }

    [RelayCommand]
    public void OpenEmotePicker()
    {
        IsEmotePickerOpen = !IsEmotePickerOpen;
    }

    [RelayCommand]
    public void CloseEmotePicker()
    {
        IsEmotePickerOpen = false;
    }

    [RelayCommand]
    public void InsertRawEmoji(string symbol)
    {
        MessageInputText += (string.IsNullOrEmpty(MessageInputText) ? "" : " ") + symbol;
    }

    [RelayCommand]
    public void ToggleMemberList()
    {
        _mainVM.ToggleMemberList();
    }

    [RelayCommand]
    public void ToggleGlassBubbles()
    {
        _mainVM.ToggleGlassBubbles();
        OnPropertyChanged(nameof(IsGlassBubblesMode));
    }

    [RelayCommand]
    public void StartVoiceNoteRecording()
    {
        IsRecordingVoice = true;
        _voiceSeconds = 0;
        VoiceRecordingDuration = "00:00";
        _mainVM.ShowToastNotification("🎙️ Началась запись голосового сообщения...");

        _voiceTimer?.Stop();
        _voiceTimer = new System.Windows.Threading.DispatcherTimer();
        _voiceTimer.Interval = TimeSpan.FromSeconds(1);
        _voiceTimer.Tick += (s, e) =>
        {
            _voiceSeconds++;
            var mins = _voiceSeconds / 60;
            var secs = _voiceSeconds % 60;
            VoiceRecordingDuration = $"{mins:D2}:{secs:D2}";
        };
        _voiceTimer.Start();
    }

    [RelayCommand]
    public void CancelVoiceNoteRecording()
    {
        _voiceTimer?.Stop();
        IsRecordingVoice = false;
        _voiceSeconds = 0;
        _mainVM.ShowToastNotification("❌ Запись аудиосообщения отменена");
    }

    [RelayCommand]
    public void FinishAndSendVoiceNote()
    {
        _voiceTimer?.Stop();
        IsRecordingVoice = false;

        var duration = Math.Max(1, _voiceSeconds);
        var channelId = CurrentChannel?.Id ?? "dm_channel";
        var msg = ChatService.Instance.SendVoiceNote(channelId, duration);
        CurrentMessages.Add(msg);
        _voiceSeconds = 0;
        _mainVM.ShowToastNotification($"🎙️ Голосовое сообщение отправлено ({duration} сек)");
        RefreshProperties();
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
    public async Task TranslateMessage(ChatMessage message)
    {
        if (message == null) return;
        if (message.IsTranslated)
        {
            message.IsTranslated = false;
            return;
        }

        var targetLang = TranslationService.Instance.TargetLanguage.Code;
        _mainVM.ShowToastNotification($"🌍 Перевод сообщения на {TranslationService.Instance.TargetLanguage.Name}...");
        message.TranslatedText = await TranslationService.Instance.TranslateTextAsync(message.Content, targetLang);
        message.TargetLangCode = targetLang.ToUpper();
        message.IsTranslated = true;
    }

    [RelayCommand]
    public void RevealSpoiler(ChatMessage message)
    {
        if (message != null)
        {
            message.IsSpoilerRevealed = !message.IsSpoilerRevealed;
        }
    }

    [RelayCommand]
    public void TogglePlayVoiceNote(ChatMessage message)
    {
        if (message == null || !message.IsVoiceNote) return;

        message.IsVoicePlaying = !message.IsVoicePlaying;
        if (message.IsVoicePlaying)
        {
            AudioService.Instance.PlaySoundCue(SoundCueType.UserJoin);
            _mainVM.ShowToastNotification($"▶️ Воспроизведение аудиосообщения ({message.VoicePlaybackSpeed:F1}x)");
        }
    }

    [RelayCommand]
    public void SetVoiceSpeed(object[] parameters)
    {
        if (parameters.Length >= 2 && parameters[0] is ChatMessage msg && parameters[1] is double speed)
        {
            msg.VoicePlaybackSpeed = speed;
            _mainVM.ShowToastNotification($"⚡ Скорость аудио: {speed:F2}x");
        }
    }

    [RelayCommand]
    public void OpenThread(ChatMessage message)
    {
        if (message == null) return;
        message.HasThread = true;
        message.ThreadReplyCount++;
        message.ThreadLastReplyTime = DateTime.Now.ToString("HH:mm");
        _mainVM.ShowToastNotification($"🧵 Ветка обсуждения открыта для сообщения от {message.Author.DisplayName}");
    }

    [RelayCommand]
    public void AttachFile()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Прикрепить файл к сообщению",
                Filter = "Все файлы (*.*)|*.*|Изображения (*.png;*.jpg;*.gif)|*.png;*.jpg;*.gif|Архивы (*.zip;*.rar;*.7z)|*.zip;*.rar;*.7z",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                ProcessFileAttachment(dialog.FileName);
            }
        }
        catch { }
    }

    public void ProcessFileAttachment(string filePath)
    {
        if (!System.IO.File.Exists(filePath)) return;

        var fileInfo = new System.IO.FileInfo(filePath);
        var channelId = CurrentChannel?.Id ?? "dm_channel";

        var msg = new ChatMessage
        {
            ChannelId = channelId,
            Author = FellowshipService.Instance.CurrentUser,
            Content = $"📎 Прикреплен файл: {fileInfo.Name}",
            HasFileAttachment = true,
            FileName = fileInfo.Name,
            FileSizeFormatted = $"{fileInfo.Length / 1024.0 / 1024.0:F2} МБ",
            FileExtension = fileInfo.Extension.ToUpperInvariant(),
            Timestamp = DateTime.Now
        };

        CurrentMessages.Add(msg);
        _mainVM.ShowToastNotification($"📤 Файл {fileInfo.Name} загружен в чат");
    }

    [RelayCommand]
    public void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(MessageInputText)) return;

        var text = MessageInputText;
        var channelId = CurrentChannel?.Id ?? (CurrentDmUser?.Id ?? "user_bot");
        var msg = ChatService.Instance.SendMessage(channelId, text, ReplyingToMessage);

        // Parse Code Blocks ```csharp ... ```
        if (text.Contains("```"))
        {
            var parts = text.Split("```", StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
            {
                msg.HasCodeBlock = true;
                msg.CodeLanguage = "C# / Code";
                msg.CodeContent = parts[0].Trim();
            }
        }

        // Parse Spoilers ||hidden text||
        if (text.Contains("||"))
        {
            var spoilerParts = text.Split("||", StringSplitOptions.RemoveEmptyEntries);
            if (spoilerParts.Length >= 1)
            {
                msg.HasSpoiler = true;
                msg.SpoilerText = spoilerParts[0].Trim();
            }
        }
        
        CurrentMessages.Add(msg);

        MessageInputText = string.Empty;
        ReplyingToMessage = null;
        RefreshProperties();

        // If in DM with bot, auto-reply
        if (IsDirectMessages && (CurrentDmUser == null || CurrentDmUser.Id == "user_bot"))
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                var botReply = new ChatMessage
                {
                    Author = new User
                    {
                        Id = "user_bot",
                        Username = "storm_ai",
                        DisplayName = "STORM Bot",
                        Tag = "0000",
                        AvatarGlyph = "🤖",
                        RoleName = "Бот",
                        RoleColorHex = "#00D2FF"
                    },
                    Content = $"⚡ [STORM AI]: Получено сообщение «{text}». E2EE канал защищен AES-256.",
                    Timestamp = DateTime.Now
                };
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    CurrentMessages.Add(botReply);
                    RefreshProperties();
                });
            });
        }
    }

    [RelayCommand]
    public void InsertEmoji(EmojiItem emoji)
    {
        MessageInputText += (string.IsNullOrEmpty(MessageInputText) ? "" : " ") + emoji.Symbol;
        IsEmotePickerOpen = false;
    }

    [RelayCommand]
    public void SendSticker(StickerItem sticker)
    {
        var channelId = CurrentChannel?.Id ?? "dm_channel";
        var msg = ChatService.Instance.SendMessage(channelId, "", ReplyingToMessage, stickerUrl: sticker.ImagePath);
        CurrentMessages.Add(msg);
        IsEmotePickerOpen = false;
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
        if (message == null) return;
        ReplyingToMessage = message;
        _mainVM.ShowToastNotification($"↩️ Ответ на сообщение от {message.Author.DisplayName}");
    }

    [RelayCommand]
    public void CancelReply()
    {
        ReplyingToMessage = null;
    }

    [RelayCommand]
    public void PinMessage(ChatMessage message)
    {
        if (message == null) return;
        ChatService.Instance.PinMessage(message);
        _mainVM.ShowToastNotification(message.IsPinned ? "📌 Сообщение закреплено" : "📌 Сообщение откреплено");
    }

    [RelayCommand]
    public void DeleteMessage(ChatMessage message)
    {
        if (message == null) return;
        CurrentMessages.Remove(message);
        if (CurrentChannel != null && CurrentChannel.Messages.Contains(message))
        {
            CurrentChannel.Messages.Remove(message);
        }
        _mainVM.ShowToastNotification("🗑️ Сообщение удалено");
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
