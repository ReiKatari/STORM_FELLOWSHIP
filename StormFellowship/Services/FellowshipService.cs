using System.Collections.ObjectModel;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class FellowshipService : IFellowshipService
{
    private static FellowshipService? _instance;
    public static FellowshipService Instance => _instance ??= new FellowshipService();

    public ObservableCollection<Fellowship> Fellowships { get; } = new();
    public ObservableCollection<FellowshipFolder> Folders { get; } = new();
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

        // Seed Roles
        mainFellowship.Roles.Add(new Role { Name = "Создатель", ColorHex = "#00A3FF", Permissions = (RolePermissions)0x3FFF, Priority = 100 });
        mainFellowship.Roles.Add(new Role { Name = "Модератор", ColorHex = "#22C55E", Permissions = RolePermissions.SendMessages | RolePermissions.AttachFiles | RolePermissions.ConnectVoice | RolePermissions.Speak | RolePermissions.PrioritySpeaker | RolePermissions.MuteMembers, Priority = 80 });
        mainFellowship.Roles.Add(new Role { Name = "Оратор", ColorHex = "#F59E0B", Permissions = RolePermissions.SendMessages | RolePermissions.ConnectVoice | RolePermissions.Speak | RolePermissions.PrioritySpeaker, Priority = 60 });
        mainFellowship.Roles.Add(new Role { Name = "Участник", ColorHex = "#94A3B8", Permissions = RolePermissions.SendMessages | RolePermissions.AttachFiles | RolePermissions.ConnectVoice | RolePermissions.Speak, Priority = 10 });

        // Add Categories
        var textCategory = new ChannelCategory { Id = "cat_text", Name = "ТЕКСТОВЫЕ КАНАЛЫ" };
        var voiceCategory = new ChannelCategory { Id = "cat_voice", Name = "ГОЛОСОВЫЕ КАНАЛЫ" };

        var generalChannel = new Channel
        {
            Id = "chan_general",
            Name = "общий",
            Topic = "Основной чат содружества",
            Type = ChannelType.Text
        };

        var newsChannel = new Channel
        {
            Id = "chan_news",
            Name = "новости",
            Topic = "Официальные объявления",
            Type = ChannelType.Announcements
        };

        var voiceChannel1 = new Channel
        {
            Id = "chan_voice_1",
            Name = "Голосовой 1",
            Topic = "Основная комната",
            Type = ChannelType.Voice,
            BitrateKbps = 128
        };

        var voiceHub = new Channel
        {
            Id = "chan_voice_hub",
            Name = "⚡ Создать комнату",
            Topic = "Автоматическое создание персональной комнаты",
            Type = ChannelType.VoiceHub,
            BitrateKbps = 128
        };

        textCategory.Channels.Add(generalChannel);
        textCategory.Channels.Add(newsChannel);
        voiceCategory.Channels.Add(voiceChannel1);
        voiceCategory.Channels.Add(voiceHub);

        mainFellowship.Categories.Add(textCategory);
        mainFellowship.Categories.Add(voiceCategory);

        mainFellowship.Members.Add(CurrentUser);
        Fellowships.Add(mainFellowship);

        // Seed Sample Folder
        var gamingFolder = new FellowshipFolder { Name = "Игры и Сообщества", ColorHex = "#A855F7" };
        Folders.Add(gamingFolder);

        // Seed Sample Welcome Message with Poll
        var welcomeMsg = new ChatMessage
        {
            Author = CurrentUser,
            Content = "Добро пожаловать в STORM FELLOWSHIP v0.0.6! ⚡ Все модули активны: игровой оверлей, E2EE, Whisper AI, 3D звук и опрос.",
            Timestamp = DateTime.Now
        };
        generalChannel.Messages.Add(welcomeMsg);

        // Sample Interactive Poll
        var samplePoll = new PollItem
        {
            Question = "Какой режим связи вы используете чаще всего?",
            AuthorName = CurrentUser.DisplayName
        };
        samplePoll.Options.Add(new PollOption { Text = "🎙️ Голосовые каналы со сверхнизкой задержкой", VotesCount = 4, Percentage = 57.0 });
        samplePoll.Options.Add(new PollOption { Text = "💬 Текстовые чаты с опросами и стикерами", VotesCount = 2, Percentage = 29.0 });
        samplePoll.Options.Add(new PollOption { Text = "📹 HD видеосозвоны и стриминг экрана", VotesCount = 1, Percentage = 14.0 });
        samplePoll.RecalculatePercentages();

        var pollMsg = new ChatMessage
        {
            Author = CurrentUser,
            Content = "Опрос сообщества:",
            Poll = samplePoll,
            Timestamp = DateTime.Now
        };
        generalChannel.Messages.Add(pollMsg);

        SelectFellowship(mainFellowship);
        SelectChannel(generalChannel);
    }

    public Fellowship CreateFellowship(string name)
    {
        return CreateFellowship(name, "Новое содружество", "ms-appx:///Assets/Logo.png");
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
        var voiceCat = new ChannelCategory { Name = "ГОЛОСОВЫЕ КАНАЛЫ" };

        textCat.Channels.Add(new Channel { Name = "общий", Type = ChannelType.Text });
        voiceCat.Channels.Add(new Channel { Name = "Голосовой 1", Type = ChannelType.Voice, BitrateKbps = 128 });
        voiceCat.Channels.Add(new Channel { Name = "⚡ Создать комнату", Type = ChannelType.VoiceHub, BitrateKbps = 128 });

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
                chan.Name = newName;
                chan.Topic = newTopic;
                chan.BitrateKbps = bitrateKbps;
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
                    SelectChannel(f.Categories.FirstOrDefault()?.Channels.FirstOrDefault());
                }
                break;
            }
        }
    }

    public void AddCategory(string fellowshipId, string name)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        if (f == null) return;

        var cat = new ChannelCategory { Name = name.ToUpperInvariant() };
        f.Categories.Add(cat);
    }

    public Role AddRole(string fellowshipId, string name, string colorHex, RolePermissions perms)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        var role = new Role { Name = name, ColorHex = colorHex, Permissions = perms };
        f?.Roles.Add(role);
        return role;
    }

    public void UpdateRole(string fellowshipId, string roleId, string name, string colorHex, RolePermissions perms)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        var role = f?.Roles.FirstOrDefault(r => r.Id == roleId);
        if (role != null)
        {
            role.Name = name;
            role.ColorHex = colorHex;
            role.Permissions = perms;
        }
    }

    public void DeleteRole(string fellowshipId, string roleId)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId) ?? CurrentFellowship;
        var role = f?.Roles.FirstOrDefault(r => r.Id == roleId);
        if (role != null)
        {
            f?.Roles.Remove(role);
        }
    }

    public FellowshipFolder CreateFolder(string name, string colorHex)
    {
        var folder = new FellowshipFolder { Name = name, ColorHex = colorHex };
        Folders.Add(folder);
        return folder;
    }

    public void SelectFellowship(Fellowship? fellowship)
    {
        IsDirectMessagesSelected = false;
        foreach (var f in Fellowships) f.IsSelected = false;

        CurrentFellowship = fellowship;
        if (CurrentFellowship != null)
        {
            CurrentFellowship.IsSelected = true;
            CurrentChannel = CurrentFellowship.Categories.FirstOrDefault()?.Channels.FirstOrDefault();
        }

        CurrentFellowshipChanged?.Invoke(CurrentFellowship);
        CurrentChannelChanged?.Invoke(CurrentChannel);
    }

    public void SelectChannel(Channel? channel)
    {
        if (channel == null) return;

        // Dynamic Voice Hub Logic
        if (channel.IsVoiceHub && CurrentFellowship != null)
        {
            var voiceCat = CurrentFellowship.Categories.FirstOrDefault(c => c.Channels.Contains(channel));
            if (voiceCat != null)
            {
                var tempChan = new Channel
                {
                    Name = $"🔊 Комната {CurrentUser.DisplayName}",
                    Topic = "Временная комната (авто-удаление)",
                    Type = ChannelType.TemporaryVoice,
                    IsTemporary = true,
                    OwnerUserId = CurrentUser.Id,
                    BitrateKbps = 128
                };
                tempChan.ConnectedVoiceUsers.Add(CurrentUser);
                voiceCat.Channels.Insert(voiceCat.Channels.IndexOf(channel) + 1, tempChan);
                CurrentChannel = tempChan;
                CurrentChannelChanged?.Invoke(CurrentChannel);
                CallService.Instance.JoinVoiceChannel(tempChan);
                return;
            }
        }

        CurrentChannel = channel;
        CurrentChannelChanged?.Invoke(CurrentChannel);

        if (channel.IsVoice)
        {
            CallService.Instance.JoinVoiceChannel(channel);
        }
    }

    public void SelectDirectMessage(User? user)
    {
        IsDirectMessagesSelected = true;
        foreach (var f in Fellowships) f.IsSelected = false;

        CurrentDmUser = user;
        CurrentFellowship = null;
        CurrentChannel = null;

        CurrentDmUserChanged?.Invoke(CurrentDmUser);
        CurrentFellowshipChanged?.Invoke(null);
        CurrentChannelChanged?.Invoke(null);
    }
}
