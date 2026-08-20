using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public interface IChatService
{
    ObservableCollection<EmojiItem> Emojis { get; }
    ObservableCollection<StickerItem> Stickers { get; }
    
    ChatMessage SendMessage(string channelId, string content, ChatMessage? replyTo = null, string attachmentUrl = "", string stickerUrl = "", bool isVoiceNote = false, int voiceDuration = 0);
    ChatMessage SendVoiceNote(string channelId, int durationSeconds);
    void ToggleReaction(ChatMessage message, string emojiSymbol, string emojiCode);
    void PinMessage(ChatMessage message);
    void DeleteMessage(Channel channel, ChatMessage message);
}
