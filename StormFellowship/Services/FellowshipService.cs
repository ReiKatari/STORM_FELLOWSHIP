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
            Username = "You",
            DisplayName = "You",
            Tag = "0001",
            AvatarPath = "ms-appx:///Assets/Avatars/you.png",
            Status = UserStatus.Online,
            CustomStatus = "Exploring STORM FELLOWSHIP v0.0.1",
            RoleName = "Storm Commander",
            RoleColorHex = "#00A3FF"
        };

        SeedData();
    }

    private void SeedData()
    {
        // Seed Direct Message Users
        var sakura = new User
        {
            Id = "user_sakura",
            Username = "Sakura",
            DisplayName = "Sakura",
            Tag = "7721",
            AvatarPath = "ms-appx:///Assets/Avatars/sakura.png",
            CustomStatus = "What the bobba",
            Status = UserStatus.InVoice,
            RoleName = "Storm Guard",
            RoleColorHex = "#FF6A88"
        };

        var valkyrie = new User
        {
            Id = "user_valkyrie",
            Username = "Valkyrie",
            DisplayName = "Valkyrie",
            Tag = "1337",
            AvatarPath = "ms-appx:///Assets/Avatars/valkyrie.png",
            CustomStatus = "Clutching the tournament finals",
            Status = UserStatus.Streaming,
            RoleName = "Tournament Champion",
            RoleColorHex = "#A855F7"
        };

        var alex = new User
        {
            Id = "user_alex",
            Username = "Alex",
            DisplayName = "Alex",
            Tag = "4096",
            AvatarPath = "ms-appx:///Assets/Avatars/alex.png",
            CustomStatus = "Configuring 3D spatial audio nodes",
            Status = UserStatus.Online,
            RoleName = "Audio Engineer",
            RoleColorHex = "#38EF7D"
        };

        var elena = new User
        {
            Id = "user_elena",
            Username = "Elena",
            DisplayName = "Elena",
            Tag = "9920",
            AvatarPath = "ms-appx:///Assets/Avatars/elena.png",
            CustomStatus = "Streaming STORM Fellowship Session",
            Status = UserStatus.Idle,
            RoleName = "Moderator",
            RoleColorHex = "#F59E0B"
        };

        var stormBot = new User
        {
            Id = "user_stormbot",
            Username = "Storm Relay Bot",
            DisplayName = "Storm Relay",
            Tag = "0000",
            AvatarPath = "ms-appx:///Assets/Avatars/storm_bot.png",
            CustomStatus = "Storm Low-Latency Engine v0.0.1 Active",
            Status = UserStatus.Online,
            RoleName = "System Bot",
            RoleColorHex = "#22C55E"
        };

        DirectMessageUsers.Add(sakura);
        DirectMessageUsers.Add(valkyrie);
        DirectMessageUsers.Add(alex);
        DirectMessageUsers.Add(elena);
        DirectMessageUsers.Add(stormBot);

        // Fellowship 1: Storm Sanctuary
        var sanctuary = new Fellowship
        {
            Id = "guild_sanctuary",
            Name = "Storm Sanctuary",
            Tag = "STORM",
            Description = "Official headquarters for STORM FELLOWSHIP voice ops and gaming sessions.",
            IconUrl = "ms-appx:///Assets/Logo.png",
            OwnerId = CurrentUser.Id,
            IsSelected = true
        };

        sanctuary.Members.Add(CurrentUser);
        sanctuary.Members.Add(sakura);
        sanctuary.Members.Add(valkyrie);
        sanctuary.Members.Add(alex);
        sanctuary.Members.Add(elena);
        sanctuary.Members.Add(stormBot);

        var textCat = new ChannelCategory { Id = "cat_text", Name = "TEXT CHANNELS" };
        var genChan = new Channel { Id = "chan_general", Name = "general", Topic = "Main fellowship lobby for chats, gaming, and banter.", Type = ChannelType.Text };
        var annChan = new Channel { Id = "chan_announcements", Name = "announcements", Topic = "STORM FELLOWSHIP updates, patch notes & releases.", Type = ChannelType.Announcements };
        var buildsChan = new Channel { Id = "chan_builds", Name = "builds-and-strats", Topic = "Share loadouts, tactical strats, and configs.", Type = ChannelType.Text };
        var clipsChan = new Channel { Id = "chan_clips", Name = "media-clips", Topic = "Top plays, clutches, screenshots and clips.", Type = ChannelType.Text };

        textCat.Channels.Add(genChan);
        textCat.Channels.Add(annChan);
        textCat.Channels.Add(buildsChan);
        textCat.Channels.Add(clipsChan);

        var voiceCat = new ChannelCategory { Id = "cat_voice", Name = "VOICE CHANNELS" };
        var opsVoice = new Channel { Id = "voice_ops", Name = "Ops Command 1", Topic = "Low-latency 128kbps Opus audio channel", Type = ChannelType.Voice, BitrateKbps = 128 };
        var duoVoice = new Channel { Id = "voice_duo", Name = "Duo Queue A", Topic = "2-player tactical channel", Type = ChannelType.Voice, BitrateKbps = 96, UserLimit = 2 };
        var spatialVoice = new Channel { Id = "voice_3d", Name = "3D Positional Stage", Topic = "3D spatial audio demonstration channel", Type = ChannelType.Voice, BitrateKbps = 160 };
        var afkVoice = new Channel { Id = "voice_afk", Name = "AFK Lounge", Topic = "Muted standby area", Type = ChannelType.Voice, BitrateKbps = 32 };

        opsVoice.ConnectedVoiceUsers.Add(sakura);
        opsVoice.ConnectedVoiceUsers.Add(CurrentUser);
        duoVoice.ConnectedVoiceUsers.Add(valkyrie);
        duoVoice.ConnectedVoiceUsers.Add(alex);

        voiceCat.Channels.Add(opsVoice);
        voiceCat.Channels.Add(duoVoice);
        voiceCat.Channels.Add(spatialVoice);
        voiceCat.Channels.Add(afkVoice);

        sanctuary.Categories.Add(textCat);
        sanctuary.Categories.Add(voiceCat);

        // Seed Sample Chat Messages in #general
        SeedGeneralMessages(genChan, sakura, valkyrie, alex, stormBot);

        // Fellowship 2: Esports Pro League
        var esports = new Fellowship
        {
            Id = "guild_esports",
            Name = "Esports Pro League",
            Tag = "ESPORTS",
            Description = "Competitive tournament brackets, scrims, and low-latency voice channels.",
            IconUrl = "ms-appx:///Assets/Stickers/storm_victory.png",
            OwnerId = valkyrie.Id
        };
        var esTextCat = new ChannelCategory { Id = "es_cat_text", Name = "TOURNAMENT" };
        esTextCat.Channels.Add(new Channel { Id = "es_lobby", Name = "tournament-lobby", Type = ChannelType.Text });
        esTextCat.Channels.Add(new Channel { Id = "es_scrims", Name = "scrim-schedules", Type = ChannelType.Text });
        var esVoiceCat = new ChannelCategory { Id = "es_cat_voice", Name = "MATCH CHANNELS" };
        esVoiceCat.Channels.Add(new Channel { Id = "es_team_a", Name = "Team Alpha", Type = ChannelType.Voice, BitrateKbps = 128 });
        esVoiceCat.Channels.Add(new Channel { Id = "es_team_b", Name = "Team Bravo", Type = ChannelType.Voice, BitrateKbps = 128 });
        esports.Categories.Add(esTextCat);
        esports.Categories.Add(esVoiceCat);

        Fellowships.Add(sanctuary);
        Fellowships.Add(esports);

        CurrentFellowship = sanctuary;
        CurrentChannel = genChan;
    }

    private void SeedGeneralMessages(Channel chan, User sakura, User valkyrie, User alex, User stormBot)
    {
        var msg1 = new ChatMessage
        {
            Id = "m1",
            ChannelId = chan.Id,
            Author = stormBot,
            Content = "⚡ **Welcome to STORM FELLOWSHIP v0.0.1!**\nExperience ultra-low latency voice channels combined with rich chat, animated stickers, customizable themes, and 1-1 direct calling.",
            Timestamp = DateTime.Now.AddMinutes(-45),
            IsPinned = true
        };
        var r1 = new MessageReaction { EmojiCode = ":storm_bolt:", EmojiSymbol = "⚡", Count = 5, HasReacted = true };
        var r2 = new MessageReaction { EmojiCode = ":fire:", EmojiSymbol = "🔥", Count = 4 };
        msg1.Reactions.Add(r1);
        msg1.Reactions.Add(r2);

        var msg2 = new ChatMessage
        {
            Id = "m2",
            ChannelId = chan.Id,
            Author = sakura,
            Content = "Hey everyone! The new 1-1 direct call UI looks incredible! Look at the waveform and avatar speaking rings 😍",
            Timestamp = DateTime.Now.AddMinutes(-30)
        };
        msg2.Reactions.Add(new MessageReaction { EmojiCode = ":heart:", EmojiSymbol = "💖", Count = 3, HasReacted = true });

        var msg3 = new ChatMessage
        {
            Id = "m3",
            ChannelId = chan.Id,
            Author = alex,
            Content = "The 3D positional audio and Opus 128kbps codec latency is less than 15ms. Studio-grade clarity achieved!",
            Timestamp = DateTime.Now.AddMinutes(-20)
        };

        var msg4 = new ChatMessage
        {
            Id = "m4",
            ChannelId = chan.Id,
            Author = valkyrie,
            Content = "Dropping our championship victory sticker into the chat! GG!",
            Timestamp = DateTime.Now.AddMinutes(-10),
            HasSticker = true,
            StickerUrl = "ms-appx:///Assets/Stickers/storm_victory.png"
        };
        msg4.Reactions.Add(new MessageReaction { EmojiCode = ":trophy:", EmojiSymbol = "🏆", Count = 7, HasReacted = true });

        chan.Messages.Add(msg1);
        chan.Messages.Add(msg2);
        chan.Messages.Add(msg3);
        chan.Messages.Add(msg4);
    }

    public Fellowship CreateFellowship(string name)
    {
        return CreateFellowship(name, "Fellowship created by " + CurrentUser.DisplayName);
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

        var textCat = new ChannelCategory { Name = "TEXT CHANNELS" };
        var genChan = new Channel { Name = "general", Topic = "General discussion", Type = ChannelType.Text };
        textCat.Channels.Add(genChan);

        var voiceCat = new ChannelCategory { Name = "VOICE CHANNELS" };
        var genVoice = new Channel { Name = "General Voice", Topic = "Main voice room", Type = ChannelType.Voice };
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

    public Channel AddChannel(string fellowshipId, string categoryId, string name, ChannelType type, int bitrateKbps = 96)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
        if (f == null) throw new InvalidOperationException("Fellowship not found");

        var cat = f.Categories.FirstOrDefault(c => c.Id == categoryId) ?? f.Categories.FirstOrDefault();
        if (cat == null)
        {
            cat = new ChannelCategory { Name = type == ChannelType.Voice ? "VOICE CHANNELS" : "TEXT CHANNELS" };
            f.Categories.Add(cat);
        }

        var chan = new Channel
        {
            Name = name.ToLower().Replace(" ", "-"),
            Type = type,
            BitrateKbps = bitrateKbps
        };

        cat.Channels.Add(chan);
        return chan;
    }

    public void DeleteChannel(string fellowshipId, string channelId)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
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
