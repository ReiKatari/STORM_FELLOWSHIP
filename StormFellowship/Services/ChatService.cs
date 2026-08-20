using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class ChatService : IChatService
{
    private static ChatService? _instance;
    public static ChatService Instance => _instance ??= new ChatService();

    public ObservableCollection<EmojiItem> Emojis { get; } = new();
    public ObservableCollection<StickerItem> Stickers { get; } = new();

    public ChatService()
    {
        InitializeEmojis();
        InitializeStickers();
    }

    private void InitializeEmojis()
    {
        // Custom Storm Emojis
        Emojis.Add(new EmojiItem { Code = ":storm_bolt:", Symbol = "⚡", Name = "Storm Lightning", Category = "Storm" });
        Emojis.Add(new EmojiItem { Code = ":storm_shield:", Symbol = "🛡️", Name = "Storm Shield", Category = "Storm" });
        Emojis.Add(new EmojiItem { Code = ":storm_crown:", Symbol = "👑", Name = "Storm Crown", Category = "Storm" });
        Emojis.Add(new EmojiItem { Code = ":storm_fire:", Symbol = "🔥", Name = "Storm Fire", Category = "Storm" });
        Emojis.Add(new EmojiItem { Code = ":storm_sword:", Symbol = "⚔️", Name = "Storm Swords", Category = "Storm" });
        Emojis.Add(new EmojiItem { Code = ":storm_trophy:", Symbol = "🏆", Name = "Storm Trophy", Category = "Storm" });

        // Smileys & Emotion
        Emojis.Add(new EmojiItem { Code = ":smile:", Symbol = "😄", Name = "Grinning Face", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":heart_eyes:", Symbol = "😍", Name = "Heart Eyes", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":sunglasses:", Symbol = "😎", Name = "Cool Sunglasses", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":party:", Symbol = "🥳", Name = "Party Popper", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":joy:", Symbol = "😂", Name = "Tears of Joy", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":mind_blown:", Symbol = "🤯", Name = "Mind Blown", Category = "Smileys" });
        Emojis.Add(new EmojiItem { Code = ":thinking:", Symbol = "🤔", Name = "Thinking", Category = "Smileys" });

        // Gaming & Esports
        Emojis.Add(new EmojiItem { Code = ":gamepad:", Symbol = "🎮", Name = "Gamepad", Category = "Gaming" });
        Emojis.Add(new EmojiItem { Code = ":target:", Symbol = "🎯", Name = "Bullseye Target", Category = "Gaming" });
        Emojis.Add(new EmojiItem { Code = ":skull:", Symbol = "💀", Name = "Skull", Category = "Gaming" });
        Emojis.Add(new EmojiItem { Code = ":100:", Symbol = "💯", Name = "Hundred Points", Category = "Gaming" });
        Emojis.Add(new EmojiItem { Code = ":robot:", Symbol = "🤖", Name = "Robot Bot", Category = "Gaming" });

        // Tech & Audio
        Emojis.Add(new EmojiItem { Code = ":headphone:", Symbol = "🎧", Name = "Headphones", Category = "Tech" });
        Emojis.Add(new EmojiItem { Code = ":mic:", Symbol = "🎙️", Name = "Studio Mic", Category = "Tech" });
        Emojis.Add(new EmojiItem { Code = ":sound:", Symbol = "🔊", Name = "High Volume", Category = "Tech" });
        Emojis.Add(new EmojiItem { Code = ":radio:", Symbol = "📻", Name = "Radio Transceiver", Category = "Tech" });
        Emojis.Add(new EmojiItem { Code = ":rocket:", Symbol = "🚀", Name = "Rocket Speed", Category = "Tech" });
    }

    private void InitializeStickers()
    {
        Stickers.Add(new StickerItem { Id = "stk_gg", Name = "STORM GG", ImagePath = "ms-appx:///Assets/Stickers/storm_gg.png" });
        Stickers.Add(new StickerItem { Id = "stk_hype", Name = "HYPED UP!", ImagePath = "ms-appx:///Assets/Stickers/storm_hype.png" });
        Stickers.Add(new StickerItem { Id = "stk_rage", Name = "RAGE MODE", ImagePath = "ms-appx:///Assets/Stickers/storm_rage.png" });
        Stickers.Add(new StickerItem { Id = "stk_victory", Name = "VICTORY!", ImagePath = "ms-appx:///Assets/Stickers/storm_victory.png" });
        Stickers.Add(new StickerItem { Id = "stk_fellowship", Name = "FELLOWSHIP", ImagePath = "ms-appx:///Assets/Stickers/storm_fellowship.png" });
        Stickers.Add(new StickerItem { Id = "stk_clutch", Name = "CLUTCH GOD", ImagePath = "ms-appx:///Assets/Stickers/storm_clutch.png" });
    }

    public ChatMessage SendMessage(string channelId, string content, ChatMessage? replyTo = null, string attachmentUrl = "", string stickerUrl = "", bool isVoiceNote = false, int voiceDuration = 0)
    {
        var msg = new ChatMessage
        {
            ChannelId = channelId,
            Author = FellowshipService.Instance.CurrentUser,
            Content = content,
            Timestamp = DateTime.Now,
            AttachmentUrl = attachmentUrl,
            HasAttachment = !string.IsNullOrEmpty(attachmentUrl),
            StickerUrl = stickerUrl,
            HasSticker = !string.IsNullOrEmpty(stickerUrl),
            IsVoiceNote = isVoiceNote,
            VoiceNoteDurationSeconds = voiceDuration
        };

        if (replyTo != null)
        {
            msg.HasReply = true;
            msg.ReplyToAuthor = replyTo.Author.DisplayName;
            msg.ReplyToContent = replyTo.Content.Length > 50 ? replyTo.Content.Substring(0, 47) + "..." : replyTo.Content;
        }

        AudioService.Instance.PlaySoundCue(SoundCueType.MessageReceived);
        return msg;
    }

    public ChatMessage SendVoiceNote(string channelId, int durationSeconds)
    {
        return SendMessage(channelId, "🎤 Voice Note (" + durationSeconds + "s)", isVoiceNote: true, voiceDuration: durationSeconds);
    }

    public void ToggleReaction(ChatMessage message, string emojiSymbol, string emojiCode)
    {
        var existing = message.Reactions.FirstOrDefault(r => r.EmojiSymbol == emojiSymbol || r.EmojiCode == emojiCode);
        var currentUserId = FellowshipService.Instance.CurrentUser.Id;

        if (existing != null)
        {
            if (existing.HasReacted)
            {
                existing.Count--;
                existing.HasReacted = false;
                existing.UserIds.Remove(currentUserId);
                if (existing.Count <= 0)
                {
                    message.Reactions.Remove(existing);
                }
            }
            else
            {
                existing.Count++;
                existing.HasReacted = true;
                existing.UserIds.Add(currentUserId);
            }
        }
        else
        {
            var newReaction = new MessageReaction
            {
                EmojiCode = emojiCode,
                EmojiSymbol = emojiSymbol,
                Count = 1,
                HasReacted = true
            };
            newReaction.UserIds.Add(currentUserId);
            message.Reactions.Add(newReaction);
        }
    }

    public void PinMessage(ChatMessage message)
    {
        message.IsPinned = !message.IsPinned;
    }

    public void DeleteMessage(Channel channel, ChatMessage message)
    {
        channel.Messages.Remove(message);
    }
}
