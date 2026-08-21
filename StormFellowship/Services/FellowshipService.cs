using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
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
            AvatarGlyph = "GeoLightning",
            Status = UserStatus.Online,
            CustomStatus = "В сети",
            RoleName = "Создатель",
            RoleColorHex = "#3B82F6"
        };

        LoadUserProfile();
        SeedData();
    }

    private static string GetUserProfileFilePath()
    {
        string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StormFellowship");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, "user_profile.json");
    }

    public void SaveUserProfile()
    {
        try
        {
            var data = new
            {
                DisplayName = CurrentUser.DisplayName,
                CustomStatus = CurrentUser.CustomStatus,
                AvatarPath = CurrentUser.AvatarPath,
                AvatarGlyph = CurrentUser.AvatarGlyph,
                RoleColorHex = CurrentUser.RoleColorHex,
                Username = CurrentUser.Username,
                Tag = CurrentUser.Tag
            };
            string json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(GetUserProfileFilePath(), json);
        }
        catch { }
    }

    public void LoadUserProfile()
    {
        try
        {
            string path = GetUserProfileFilePath();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var root = doc.RootElement;
                if (root.TryGetProperty("DisplayName", out var dn) && !string.IsNullOrWhiteSpace(dn.GetString()))
                    CurrentUser.DisplayName = dn.GetString()!;
                if (root.TryGetProperty("CustomStatus", out var cs))
                    CurrentUser.CustomStatus = cs.GetString() ?? "В сети";
                if (root.TryGetProperty("AvatarPath", out var ap))
                    CurrentUser.AvatarPath = ap.GetString() ?? string.Empty;
                if (root.TryGetProperty("AvatarGlyph", out var ag) && !string.IsNullOrWhiteSpace(ag.GetString()))
                    CurrentUser.AvatarGlyph = ag.GetString()!;
                if (root.TryGetProperty("RoleColorHex", out var rc) && !string.IsNullOrWhiteSpace(rc.GetString()))
                    CurrentUser.RoleColorHex = rc.GetString()!;
            }
        }
        catch { }
    }

    private readonly Dictionary<string, ObservableCollection<ChatMessage>> _dmMessageStore = new();

    public ObservableCollection<ChatMessage> GetDirectMessages(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) userId = "user_bot";
        if (!_dmMessageStore.TryGetValue(userId, out var list))
        {
            list = new ObservableCollection<ChatMessage>();
            _dmMessageStore[userId] = list;
        }
        return list;
    }

    private void SeedData()
    {
        Fellowships.Clear();
        Folders.Clear();
        DirectMessageUsers.Clear();

        var botUser = new User
        {
            Id = "user_bot",
            Username = "storm_ai",
            DisplayName = "STORM Bot",
            Tag = "0000",
            AvatarGlyph = "🤖",
            Status = UserStatus.Online,
            CustomStatus = "⚡ AI Ассистент и Саундборд",
            RoleName = "Бот",
            RoleColorHex = "#00D2FF"
        };

        DirectMessageUsers.Add(botUser);

        var botChat = GetDirectMessages(botUser.Id);
        if (botChat.Count == 0)
        {
            botChat.Add(new ChatMessage
            {
                Author = botUser,
                Content = "Привет! Я STORM Bot — твой AI-ассистент. Ты можешь общаться со мной здесь, создавать содружества, настраивать папки и использовать саундборд!",
                Timestamp = DateTime.Now
            });
        }

        SelectDirectMessage(botUser);
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

        textCat.Channels.Add(new Channel { Name = "Общий", Type = ChannelType.Text });
        voiceCat.Channels.Add(new Channel { Name = "Голосовой 1", Type = ChannelType.Voice, BitrateKbps = 128 });
        voiceCat.Channels.Add(new Channel { Name = "Создать комнату", Type = ChannelType.VoiceHub, BitrateKbps = 128 });

        f.Categories.Add(textCat);
        f.Categories.Add(voiceCat);
        f.Members.Add(CurrentUser);

        Fellowships.Add(f);
        SelectFellowship(f);
        return f;
    }

    public Fellowship? JoinFellowship(string inviteCode)
    {
        if (string.IsNullOrWhiteSpace(inviteCode)) return null;

        string cleanCode = inviteCode.Replace("storm://invite/", "").Replace("storm://join/", "").Trim();

        var existing = Fellowships.FirstOrDefault(f => f.Id.Equals(cleanCode, StringComparison.OrdinalIgnoreCase) || f.Name.Contains(cleanCode, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            if (!existing.Members.Any(m => m.Id == CurrentUser.Id))
            {
                existing.Members.Add(CurrentUser);
            }
            SelectFellowship(existing);
            return existing;
        }

        // Create joined fellowship from invite code
        string fellowshipName = cleanCode.Length > 8 ? $"Содружество #{cleanCode[..6].ToUpper()}" : $"Содружество {cleanCode}";
        var newFellowship = new Fellowship
        {
            Id = cleanCode,
            Name = fellowshipName,
            Description = $"Подключено по ссылке-приглашению: {inviteCode}",
            IconUrl = "pack://application:,,,/Assets/AppIcon.png",
            OwnerId = "remote_host"
        };

        var textCat = new ChannelCategory { Name = "ТЕКСТОВЫЕ КАНАЛЫ" };
        var voiceCat = new ChannelCategory { Name = "ГОЛОСОВЫЕ КАНАЛЫ (OPUS HD)" };

        var generalChan = new Channel { Name = "общий", Type = ChannelType.Text, Topic = "Канал связи участников содружества" };
        generalChan.Messages.Add(new ChatMessage
        {
            Author = new User { DisplayName = "STORM System", Username = "system", RoleName = "Система", RoleColorHex = "#3B82F6", AvatarGlyph = "GeoShield" },
            Content = $"👋 Добро пожаловать! Вы успешно присоединились к «{fellowshipName}» по ссылке-приглашению.",
            Timestamp = DateTime.Now
        });

        textCat.Channels.Add(generalChan);
        voiceCat.Channels.Add(new Channel { Name = "Голосовой 1", Type = ChannelType.Voice, BitrateKbps = 256 });
        voiceCat.Channels.Add(new Channel { Name = "Игровая комната", Type = ChannelType.Voice, BitrateKbps = 384 });

        newFellowship.Categories.Add(textCat);
        newFellowship.Categories.Add(voiceCat);
        newFellowship.Members.Add(CurrentUser);

        Fellowships.Add(newFellowship);
        SelectFellowship(newFellowship);
        return newFellowship;
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

    public FellowshipFolder CreateFolder(string name, string icon = "📁", string colorHex = "#00A3FF")
    {
        var folder = new FellowshipFolder
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Новая папка" : name,
            Icon = string.IsNullOrWhiteSpace(icon) ? "📁" : icon,
            ColorHex = string.IsNullOrWhiteSpace(colorHex) ? "#00A3FF" : colorHex,
            IsExpanded = true
        };
        Folders.Add(folder);
        return folder;
    }

    public void DeleteFolder(FellowshipFolder folder)
    {
        foreach (var f in folder.Fellowships.ToList())
        {
            if (!Fellowships.Contains(f))
            {
                Fellowships.Add(f);
            }
        }
        folder.Fellowships.Clear();
        Folders.Remove(folder);
    }

    public void DeleteFellowship(string fellowshipId)
    {
        var f = Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
        if (f != null)
        {
            Fellowships.Remove(f);
        }

        foreach (var folder in Folders)
        {
            var nested = folder.Fellowships.FirstOrDefault(x => x.Id == fellowshipId);
            if (nested != null)
            {
                folder.Fellowships.Remove(nested);
            }
        }

        if (CurrentFellowship?.Id == fellowshipId)
        {
            var nextF = Fellowships.FirstOrDefault() ?? Folders.SelectMany(x => x.Fellowships).FirstOrDefault();
            SelectFellowship(nextF);
        }
    }

    public void MoveFellowshipToFolder(Fellowship fellowship, FellowshipFolder targetFolder)
    {
        if (fellowship == null || targetFolder == null) return;

        if (Fellowships.Contains(fellowship))
        {
            Fellowships.Remove(fellowship);
        }

        foreach (var fld in Folders)
        {
            if (fld != targetFolder && fld.Fellowships.Contains(fellowship))
            {
                fld.Fellowships.Remove(fellowship);
            }
        }

        if (!targetFolder.Fellowships.Contains(fellowship))
        {
            targetFolder.Fellowships.Add(fellowship);
        }
        targetFolder.IsExpanded = true;
    }

    public void RemoveFellowshipFromFolder(Fellowship fellowship, FellowshipFolder folder)
    {
        if (fellowship == null || folder == null) return;
        if (folder.Fellowships.Contains(fellowship))
        {
            folder.Fellowships.Remove(fellowship);
        }
        if (!Fellowships.Contains(fellowship))
        {
            Fellowships.Add(fellowship);
        }
    }
}
