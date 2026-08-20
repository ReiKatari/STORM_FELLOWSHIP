using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class FellowshipService : IFellowshipService
{
    private static FellowshipService? _instance;
    public static FellowshipService Instance => _instance ??= new FellowshipService();

    public ObservableCollection<Fellowship> Fellowships { get; } = new();
    public ObservableCollection<User> DirectMessageUsers { get; } = new();

    public Fellowship? CurrentFellowship { get; set; }
    public Channel? CurrentChannel { get; set; }
    public User? CurrentDmUser { get; set; }
    public bool IsDirectMessagesSelected { get; set; } = false;

    public User CurrentUser { get; private set; }

    public event Action<Fellowship?>? CurrentFellowshipChanged;
    public event Action<Channel?>? CurrentChannelChanged;
    public event Action<User?>? CurrentDmUserChanged;

    public FellowshipService()
    {
        CurrentUser = new User
        {
            Id = "user_local",
            Username = "user",
            DisplayName = "Пользователь",
            Tag = "0001",
            AvatarGlyph = "⚡",
            Status = UserStatus.Online,
            CustomStatus = "В сети",
            RoleName = "Создатель",
            RoleColorHex = "#00A3FF"
        };

        SeedData();
    }

    private void SeedData()
    {
        var mainFellowship = new Fellowship
        {
            Id = "guild_main",
            Name = "Основное содружество",
            Tag = "STORM",
            Description = "Пространство для голосового и текстового общения.",
            IconUrl = "ms-appx:///Assets/Logo.png",
            OwnerId = CurrentUser.Id,
            IsSelected = true
        };

        mainFellowship.Members.Add(CurrentUser);

        // Text Channels Category
        var textCat = new ChannelCategory { Id = "cat_text", Name = "ТЕКСТОВЫЕ КАНАЛЫ" };
        var genChan = new Channel
        {
            Id = "chan_general",
            Name = "общий",
            Topic = "Основной текстовый чат для общения.",
            Type = ChannelType.Text
        };
        var newsChan = new Channel
        {
            Id = "chan_news",
            Name = "новости",
            Topic = "Объявления и важная информация.",
            Type = ChannelType.Announcements
        };

        textCat.Channels.Add(genChan);
        textCat.Channels.Add(newsChan);

        // Voice Channels Category
        var voiceCat = new ChannelCategory { Id = "cat_voice", Name = "ГОЛОСОВЫЕ КАНАЛЫ" };
        var voice1 = new Channel
        {
            Id = "voice_1",
            Name = "Голосовой 1",
            Topic = "Основная голосовая комната 128 Кбит/с",
            Type = ChannelType.Voice,
            BitrateKbps = 128
        };
        var voice2 = new Channel
        {
            Id = "voice_2",
            Name = "Голосовой 2",
            Topic = "Дополнительная голосовая комната 128 Кбит/с",
            Type = ChannelType.Voice,
            BitrateKbps = 128
        };

        voiceCat.Channels.Add(voice1);
        voiceCat.Channels.Add(voice2);

        mainFellowship.Categories.Add(textCat);
        mainFellowship.Categories.Add(voiceCat);

        // Clean initial welcome message
        var welcomeMsg = new ChatMessage
        {
            Id = "m1",
            ChannelId = genChan.Id,
            Author = CurrentUser,
            Content = "⚡ **Добро пожаловать в STORM FELLOWSHIP v0.0.3!**\nСоздавайте каналы, настраивайте содружество и пользуйтесь голосовой связью с высоким качеством звука.",
            Timestamp = DateTime.Now,
            IsPinned = true
        };
        welcomeMsg.Reactions.Add(new MessageReaction { EmojiCode = ":storm_bolt:", EmojiSymbol = "⚡", Count = 1, HasReacted = true });
        genChan.Messages.Add(welcomeMsg);

        Fellowships.Add(mainFellowship);
        CurrentFellowship = mainFellowship;
        CurrentChannel = genChan;
    }

    public Fellowship CreateFellowship(string name)
    {
        return CreateFellowship(name, "Содружество, созданное пользователем " + CurrentUser.DisplayName);
    }

    public Fellowship CreateFellowship(string name, string description, string iconUrl = "")
    {
        var f = new Fellowship
        {
            Name = name,
            Description = description,
            IconUrl = string.IsNullOrWhiteSpace(iconUrl) ? "ms-appx:///Assets/Logo.png" : iconUrl,
            OwnerId = CurrentUser.Id
        };

        var textCat = new ChannelCategory { Name = "ТЕКСТОВЫЕ КАНАЛЫ" };
        var genChan = new Channel { Name = "общий", Topic = "Основной текстовый чат", Type = ChannelType.Text };
        textCat.Channels.Add(genChan);

        var voiceCat = new ChannelCategory { Name = "ГОЛОСОВЫЕ КАНАЛЫ" };
        var genVoice = new Channel { Name = "Голосовой 1", Topic = "Основная комната", Type = ChannelType.Voice, BitrateKbps = 128 };
        voiceCat.Channels.Add(genVoice);

        f.Categories.Add(textCat);
        f.Categories.Add(voiceCat);
        f.Members.Add(CurrentUser);

        Fellowships.Add(f);
        SelectFellowship(f);
        return f;
    }

    public Fellowship? JoinFellowship(string inviteCode)
    {
        var existing = Fellowships.FirstOrDefault();
        if (existing != null)
        {
            SelectFellowship(existing);
        }
        return existing;
    }

    public void DeleteFellowship(string fellowshipId)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
        if (f != null)
        {
            Fellowships.Remove(f);
            if (CurrentFellowship?.Id == fellowshipId)
            {
                SelectFellowship(Fellowships.FirstOrDefault());
            }
        }
    }

    public void RenameFellowship(string fellowshipId, string newName, string newDescription)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
        if (f != null)
        {
            f.Name = newName;
            f.Description = newDescription;
        }
    }

    public Channel AddChannel(string fellowshipId, string? categoryId, string name, ChannelType type, int bitrateKbps = 128)
    {
        return AddChannel(fellowshipId, categoryId, name, string.Empty, type, bitrateKbps);
    }

    public Channel AddChannel(string fellowshipId, string? categoryId, string name, string topic, ChannelType type, int bitrateKbps = 128)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        if (f == null) throw new InvalidOperationException("Содружество не найдено");

        ChannelCategory? cat = null;
        if (!string.IsNullOrWhiteSpace(categoryId))
        {
            cat = f.Categories.FirstOrDefault(c => c.Id == categoryId);
        }

        if (cat == null)
        {
            cat = f.Categories.FirstOrDefault(c => (type == ChannelType.Voice && c.Name.Contains("ГОЛОСОВЫЕ", StringComparison.OrdinalIgnoreCase))
                                                || (type != ChannelType.Voice && c.Name.Contains("ТЕКСТОВЫЕ", StringComparison.OrdinalIgnoreCase)))
                  ?? f.Categories.FirstOrDefault();
        }

        if (cat == null)
        {
            cat = new ChannelCategory { Name = type == ChannelType.Voice ? "ГОЛОСОВЫЕ КАНАЛЫ" : "ТЕКСТОВЫЕ КАНАЛЫ" };
            f.Categories.Add(cat);
        }

        var cleanName = name.Trim().ToLower().Replace(" ", "-");
        if (string.IsNullOrWhiteSpace(cleanName)) cleanName = "новый-канал";

        var chan = new Channel
        {
            Name = cleanName,
            Topic = topic,
            Type = type,
            BitrateKbps = bitrateKbps
        };

        cat.Channels.Add(chan);
        SelectChannel(chan);
        return chan;
    }

    public void UpdateChannel(string fellowshipId, string channelId, string newName, string newTopic, int bitrateKbps)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        if (f == null) return;

        foreach (var cat in f.Categories)
        {
            var chan = cat.Channels.FirstOrDefault(c => c.Id == channelId);
            if (chan != null)
            {
                chan.Name = newName.Trim().ToLower().Replace(" ", "-");
                chan.Topic = newTopic;
                chan.BitrateKbps = bitrateKbps;
                if (CurrentChannel?.Id == channelId)
                {
                    CurrentChannelChanged?.Invoke(chan);
                }
                break;
            }
        }
    }

    public void DeleteChannel(string fellowshipId, string channelId)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        if (f == null) return;

        foreach (var cat in f.Categories)
        {
            var chan = cat.Channels.FirstOrDefault(c => c.Id == channelId);
            if (chan != null)
            {
                cat.Channels.Remove(chan);
                if (CurrentChannel?.Id == channelId)
                {
                    CurrentChannel = f.Categories.SelectMany(c => c.Channels).FirstOrDefault();
                    CurrentChannelChanged?.Invoke(CurrentChannel);
                }
                break;
            }
        }
    }

    public void AddCategory(string fellowshipId, string name)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        if (f == null) return;

        var cat = new ChannelCategory { Name = name.ToUpper() };
        f.Categories.Add(cat);
    }

    public void SelectFellowship(Fellowship? fellowship)
    {
        foreach (var f in Fellowships) f.IsSelected = (f == fellowship);
        IsDirectMessagesSelected = (fellowship == null);

        CurrentFellowship = fellowship;
        CurrentDmUser = null;
        if (fellowship != null)
        {
            CurrentChannel = fellowship.Categories.SelectMany(c => c.Channels).FirstOrDefault(c => c.IsText);
        }
        else
        {
            CurrentChannel = null;
        }

        CurrentFellowshipChanged?.Invoke(CurrentFellowship);
        CurrentChannelChanged?.Invoke(CurrentChannel);
    }

    public void SelectChannel(Channel? channel)
    {
        CurrentChannel = channel;
        if (channel != null && channel.UnreadCount > 0)
        {
            channel.UnreadCount = 0;
        }
        CurrentChannelChanged?.Invoke(channel);
    }

    public void SelectDirectMessage(User? user)
    {
        foreach (var f in Fellowships) f.IsSelected = false;
        IsDirectMessagesSelected = true;
        CurrentFellowship = null;
        CurrentDmUser = user;
        CurrentChannel = null;

        CurrentFellowshipChanged?.Invoke(null);
        CurrentDmUserChanged?.Invoke(user);
    }
}
