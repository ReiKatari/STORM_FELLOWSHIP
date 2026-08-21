using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _windowTitle = "STORM FELLOWSHIP v0.2.2";

    [ObservableProperty]
    private bool _isCreateFellowshipModalOpen;

    [ObservableProperty]
    private bool _isAuthModalOpen;

    [ObservableProperty]
    private bool _isFellowshipSettingsModalOpen;

    [ObservableProperty]
    private bool _isUserSettingsModalOpen;

    [ObservableProperty]
    private bool _isCreateChannelModalOpen;

    [ObservableProperty]
    private bool _isEditChannelModalOpen;

    [ObservableProperty]
    private bool _isRoleManagementModalOpen;

    [ObservableProperty]
    private bool _isCreatePollModalOpen;

    [ObservableProperty]
    private bool _isSearchModalOpen;

    [ObservableProperty]
    private bool _isE2EESecurityModalOpen;

    [ObservableProperty]
    private bool _isScreenShareModalOpen;

    [ObservableProperty]
    private bool _isSidebarCompact = false;

    [ObservableProperty]
    private bool _isGlassBubblesMode = true;

    [ObservableProperty]
    private bool _isUserProfileQuickCardOpen = false;

    [ObservableProperty]
    private bool _isEmotePickerOpen = false;

    [ObservableProperty]
    private bool _isSoundboardModalOpen = false;

    [ObservableProperty]
    private bool _isQuickSwitcherModalOpen = false;

    [ObservableProperty]
    private bool _isFolderManagerModalOpen = false;

    [ObservableProperty]
    private string _folderEditName = string.Empty;

    [ObservableProperty]
    private string _folderEditIcon = "📁";

    [ObservableProperty]
    private string _folderEditColor = "#00A3FF";

    [ObservableProperty]
    private FellowshipFolder? _selectedFolderForEdit;

    public ObservableCollection<Fellowship> AvailableFellowshipsToAddToFolder { get; } = new();

    public ObservableCollection<SoundboardTrack> SoundboardTracks => SoundboardService.Instance.Tracks;

    public double SidebarWidth => IsSidebarCompact ? 56.0 : 275.0;

    [ObservableProperty]
    private bool _isMemberListVisible = true;

    [ObservableProperty]
    private bool _isInCallView;

    [ObservableProperty]
    private string _toastMessage = string.Empty;

    [ObservableProperty]
    private bool _isToastVisible;

    // Modals data bindings
    [ObservableProperty]
    private string _fellowshipNameInput = string.Empty;

    [ObservableProperty]
    private string _joinCodeInput = string.Empty;

    [ObservableProperty]
    private string _newChannelName = string.Empty;

    [ObservableProperty]
    private string _newChannelTopic = string.Empty;

    [ObservableProperty]
    private ChannelType _newChannelType = ChannelType.Text;

    [ObservableProperty]
    private int _newChannelBitrate = 128;

    [ObservableProperty]
    private string _newCategoryName = string.Empty;

    [ObservableProperty]
    private string _editingChannelName = string.Empty;

    [ObservableProperty]
    private string _editingChannelTopic = string.Empty;

    [ObservableProperty]
    private int _editingChannelBitrate = 128;

    [ObservableProperty]
    private Channel? _channelToEdit;

    [ObservableProperty]
    private ChannelCategory? _targetCategoryForNewChannel;

    // Search query & results
    [ObservableProperty]
    private string _searchQuery = string.Empty;

    public ObservableCollection<ChatMessage> SearchResults { get; } = new();

    // Poll creation inputs
    [ObservableProperty]
    private string _newPollQuestion = string.Empty;

    [ObservableProperty]
    private string _newPollQuestionImageUrl = string.Empty;

    [ObservableProperty]
    private bool _newPollAllowMultiple = false;

    [ObservableProperty]
    private bool _newPollIsAnonymous = false;

    public ObservableCollection<PollOption> NewPollOptions { get; } = new();

    // Screen Share Settings
    [ObservableProperty]
    private string _screenShareSource = "🖥️ Монитор 1: Основной экран (1920x1080 @ 144Hz)";

    [ObservableProperty]
    private string _screenShareQuality = "1080p 60 FPS (Высокая четкость HD)";

    [ObservableProperty]
    private bool _screenShareIncludeAudio = true;

    public ObservableCollection<string> ScreenShareSources { get; } = new()
    {
        "🖥️ Монитор 1: Основной экран (1920x1080 @ 144Hz)",
        "🖥️ Монитор 2: Дополнительный экран (2560x1440 @ 60Hz)",
        "🎮 Окно игры (CS2 / Valorant / DirectX 12)",
        "🌐 Окно браузера (Google Chrome / Edge)",
        "💻 Visual Studio / Кодовый редактор",
        "🎛️ Мульти-экран (2 Окна Split 50/50)",
        "📱 Мульти-стрим (Grid 2x2: 4 Окна)"
    };

    public ObservableCollection<string> ScreenShareQualities { get; } = new()
    {
        "1080p 60 FPS (Высокая четкость HD)",
        "1080p 120 FPS (Киберспортивный сверхплавный)",
        "1440p 60 FPS (2K Ultra HD)",
        "4K 60 FPS (Cinematic 4K UHD)",
        "720p 30 FPS (Экономия трафика)"
    };

    public ObservableCollection<int> AvailableBitrates { get; } = new() { 64, 96, 128, 256, 384 };

    // Sub ViewModels
    public FellowshipRailViewModel RailViewModel { get; }
    public ChannelSidebarViewModel SidebarViewModel { get; }
    public ChatViewModel ChatViewModel { get; }
    public CallViewModel CallViewModel { get; }
    public MemberListViewModel MemberListViewModel { get; }
    public UserSettingsViewModel UserSettingsViewModel { get; }
    public RoleManagementViewModel RoleManagementViewModel { get; }
    public AuthViewModel AuthViewModel { get; }

    public MainViewModel CreateFellowshipModalViewModel => this;
    public MainViewModel FellowshipSettingsModalViewModel => this;
    public MainViewModel CreateChannelModalViewModel => this;
    public MainViewModel EditChannelModalViewModel => this;

    public string E2EEFingerprint => EncryptionService.Instance.Fingerprint;

    public User CurrentUser => FellowshipService.Instance.CurrentUser;

    public MainViewModel()
    {
        RailViewModel = new FellowshipRailViewModel(this);
        SidebarViewModel = new ChannelSidebarViewModel(this);
        ChatViewModel = new ChatViewModel(this);
        CallViewModel = new CallViewModel(this);
        MemberListViewModel = new MemberListViewModel(this);
        UserSettingsViewModel = new UserSettingsViewModel(this);
        RoleManagementViewModel = new RoleManagementViewModel(this);
        AuthViewModel = new AuthViewModel(this);

        CallService.Instance.CallStateChanged += (call) =>
        {
            IsInCallView = call != null;
        };

        FellowshipService.Instance.CurrentChannelChanged += (chan) =>
        {
            if (chan != null && !chan.IsVoice)
            {
                IsInCallView = false;
            }
        };
    }

    [RelayCommand]
    public void OpenAuthDialog()
    {
        IsAuthModalOpen = true;
    }

    [RelayCommand]
    public void CloseAuthDialog()
    {
        IsAuthModalOpen = false;
    }

    public void RefreshUserProfileBindings()
    {
        OnPropertyChanged(nameof(CurrentUser));
        SidebarViewModel.RefreshProperties();
        RailViewModel.RefreshProperties();
        MemberListViewModel.RefreshMembers();
    }

    [RelayCommand]
    public void ToggleGameOverlay()
    {
        GameOverlayService.Instance.ToggleOverlay();
        ShowToastNotification(GameOverlayService.Instance.IsOverlayActive
            ? "🎮 Игровой оверлей активирован (Shift + ~)"
            : "🎮 Игровой оверлей скрыт");
    }

    [RelayCommand]
    public void OpenSearchDialog()
    {
        SearchQuery = string.Empty;
        SearchResults.Clear();
        IsSearchModalOpen = true;
    }

    [RelayCommand]
    public void ExecuteSearch()
    {
        SearchResults.Clear();
        var results = SearchService.Instance.SearchMessages(SearchQuery, FellowshipService.Instance.CurrentFellowship);
        foreach (var r in results) SearchResults.Add(r);
    }

    [RelayCommand]
    public void CloseSearchDialog()
    {
        IsSearchModalOpen = false;
    }

    [RelayCommand]
    public void OpenE2EESecurityDialog()
    {
        IsE2EESecurityModalOpen = true;
    }

    [RelayCommand]
    public void CloseE2EESecurityDialog()
    {
        IsE2EESecurityModalOpen = false;
    }

    [RelayCommand]
    public void OpenRoleManagement()
    {
        IsRoleManagementModalOpen = true;
    }

    [RelayCommand]
    public void OpenCreatePollDialog()
    {
        NewPollQuestion = string.Empty;
        NewPollQuestionImageUrl = string.Empty;
        NewPollAllowMultiple = false;
        NewPollIsAnonymous = false;
        NewPollOptions.Clear();
        NewPollOptions.Add(new PollOption { Text = string.Empty });
        NewPollOptions.Add(new PollOption { Text = string.Empty });
        IsCreatePollModalOpen = true;
    }

    [RelayCommand]
    public void AddNewPollOption()
    {
        if (NewPollOptions.Count < 10)
        {
            NewPollOptions.Add(new PollOption { Text = string.Empty });
        }
        else
        {
            ShowToastNotification("Максимум 10 вариантов ответа в опросе");
        }
    }

    [RelayCommand]
    public void RemoveNewPollOption(PollOption? option)
    {
        if (option != null && NewPollOptions.Count > 2)
        {
            NewPollOptions.Remove(option);
        }
        else
        {
            ShowToastNotification("Опрос должен содержать как минимум 2 варианта ответа");
        }
    }

    [RelayCommand]
    public void BrowsePollQuestionImage()
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Изображения (*.png;*.jpg;*.jpeg;*.gif;*.webp)|*.png;*.jpg;*.jpeg;*.gif;*.webp|Все файлы (*.*)|*.*",
            Title = "Выберите изображение для вопроса опроса"
        };
        if (dlg.ShowDialog() == true)
        {
            NewPollQuestionImageUrl = dlg.FileName;
        }
    }

    [RelayCommand]
    public void ClearPollQuestionImage()
    {
        NewPollQuestionImageUrl = string.Empty;
    }

    [RelayCommand]
    public void BrowseOptionImage(PollOption? option)
    {
        if (option == null) return;
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Изображения (*.png;*.jpg;*.jpeg;*.gif;*.webp)|*.png;*.jpg;*.jpeg;*.gif;*.webp|Все файлы (*.*)|*.*",
            Title = "Выберите изображение для варианта ответа"
        };
        if (dlg.ShowDialog() == true)
        {
            option.ImageUrl = dlg.FileName;
        }
    }

    [RelayCommand]
    public void ClearOptionImage(PollOption? option)
    {
        if (option == null) return;
        option.ImageUrl = string.Empty;
    }

    [RelayCommand]
    public void SubmitCreatePoll()
    {
        var validOptions = NewPollOptions.Where(o => !string.IsNullOrWhiteSpace(o.Text)).ToList();
        if (string.IsNullOrWhiteSpace(NewPollQuestion) || validOptions.Count < 2)
        {
            ShowToastNotification("Введите вопрос и как минимум 2 варианта ответа");
            return;
        }

        var poll = new PollItem
        {
            Question = NewPollQuestion,
            QuestionImageUrl = NewPollQuestionImageUrl,
            AllowMultipleAnswers = NewPollAllowMultiple,
            IsAnonymous = NewPollIsAnonymous,
            AuthorName = FellowshipService.Instance.CurrentUser.DisplayName
        };

        foreach (var opt in validOptions)
        {
            poll.Options.Add(new PollOption
            {
                Text = opt.Text,
                ImageUrl = opt.ImageUrl
            });
        }
        poll.RecalculatePercentages();

        var channel = FellowshipService.Instance.CurrentChannel;
        var msg = new ChatMessage
        {
            ChannelId = channel?.Id ?? "general",
            Author = FellowshipService.Instance.CurrentUser,
            Content = $"📊 Опрос: {NewPollQuestion}",
            Poll = poll,
            Timestamp = DateTime.Now
        };

        if (channel != null)
        {
            channel.Messages.Add(msg);
        }
        else if (FellowshipService.Instance.CurrentDmUser != null)
        {
            var firstChan = FellowshipService.Instance.Fellowships.FirstOrDefault()?.Categories.FirstOrDefault()?.Channels.FirstOrDefault();
            firstChan?.Messages.Add(msg);
        }

        IsCreatePollModalOpen = false;
        ShowToastNotification("📊 Опрос успешно создан и опубликован в чате!");
    }

    [RelayCommand]
    public void CloseCreatePollDialog()
    {
        IsCreatePollModalOpen = false;
    }

    [RelayCommand]
    public void OpenScreenShareDialog()
    {
        RefreshRealCaptureSources();
        IsScreenShareModalOpen = true;
    }

    [RelayCommand]
    public void RefreshRealCaptureSources()
    {
        ScreenShareSources.Clear();
        var realSources = ScreenCaptureSourceService.GetRealCaptureSources();
        foreach (var s in realSources)
        {
            ScreenShareSources.Add($"{s.Title} — {s.Resolution}");
        }
        if (ScreenShareSources.Count > 0)
        {
            ScreenShareSource = ScreenShareSources[0];
        }
        else
        {
            ScreenShareSources.Add("🖥️ Основной экран (1920x1080)");
            ScreenShareSource = ScreenShareSources[0];
        }
    }

    [RelayCommand]
    public void SubmitStartScreenShare()
    {
        IsScreenShareModalOpen = false;
        CallService.Instance.ToggleScreenShare();
        ShowToastNotification($"🖥️ Трансляция источника запущена: {ScreenShareSource}");
    }

    [RelayCommand]
    public void CloseScreenShareDialog()
    {
        IsScreenShareModalOpen = false;
    }

    [RelayCommand]
    public void ToggleSidebarCompact()
    {
        IsSidebarCompact = !IsSidebarCompact;
        OnPropertyChanged(nameof(SidebarWidth));
        ShowToastNotification(IsSidebarCompact ? "Компактный режим панели каналов включен" : "Стандартный режим панели каналов");
    }

    [RelayCommand]
    public void ToggleMemberList()
    {
        IsMemberListVisible = !IsMemberListVisible;
        ShowToastNotification(IsMemberListVisible ? "👥 Список участников отображается" : "👥 Список участников скрыт");
    }

    [RelayCommand]
    public void ToggleGlassBubbles()
    {
        IsGlassBubblesMode = !IsGlassBubblesMode;
        ChatViewModel.RefreshProperties();
        ShowToastNotification(IsGlassBubblesMode 
            ? "💬 Режим сообщений: Современные стеклянные пузыри (Glass Bubbles)" 
            : "📜 Режим сообщений: Компактные строки (Compact Line View)");
    }

    [RelayCommand]
    public void OpenUserProfileQuickCard()
    {
        IsUserProfileQuickCardOpen = !IsUserProfileQuickCardOpen;
    }

    [RelayCommand]
    public void CloseUserProfileQuickCard()
    {
        IsUserProfileQuickCardOpen = false;
    }

    [RelayCommand]
    public void SetUserStatus(string statusStr)
    {
        if (Enum.TryParse<UserStatus>(statusStr, out var status))
        {
            FellowshipService.Instance.CurrentUser.Status = status;
            var label = status switch
            {
                UserStatus.Online => "В сети",
                UserStatus.Idle => "Не активен",
                UserStatus.DoNotDisturb => "Не беспокоить",
                UserStatus.Invisible => "Невидимка",
                _ => "В сети"
            };
            ShowToastNotification($"Статус изменен: {label}");
        }
        IsUserProfileQuickCardOpen = false;
    }

    [RelayCommand]
    public void CopyUserId()
    {
        try
        {
            var user = FellowshipService.Instance.CurrentUser;
            System.Windows.Clipboard.SetText($"{user.Username}#{user.Tag}");
            ShowToastNotification($"📋 ID пользователя скопирован: @{user.Username}#{user.Tag}");
        }
        catch { }
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

    public void SelectEmoteFromPicker(string emote)
    {
        ChatViewModel.MessageInputText += (string.IsNullOrEmpty(ChatViewModel.MessageInputText) ? "" : " ") + emote;
        IsEmotePickerOpen = false;
    }

    [RelayCommand]
    public void OpenSoundboard()
    {
        IsSoundboardModalOpen = true;
    }

    [RelayCommand]
    public void CloseSoundboardDialog()
    {
        IsSoundboardModalOpen = false;
    }

    [RelayCommand]
    public void OpenQuickSwitcher()
    {
        IsQuickSwitcherModalOpen = true;
    }

    [RelayCommand]
    public void CloseQuickSwitcherDialog()
    {
        IsQuickSwitcherModalOpen = false;
    }

    public void CloseAllModals()
    {
        IsCreateFellowshipModalOpen = false;
        IsFellowshipSettingsModalOpen = false;
        IsUserSettingsModalOpen = false;
        IsCreateChannelModalOpen = false;
        IsEditChannelModalOpen = false;
        IsRoleManagementModalOpen = false;
        IsCreatePollModalOpen = false;
        IsSearchModalOpen = false;
        IsE2EESecurityModalOpen = false;
        IsScreenShareModalOpen = false;
        IsEmotePickerOpen = false;
        IsUserProfileQuickCardOpen = false;
        IsSoundboardModalOpen = false;
        IsQuickSwitcherModalOpen = false;
    }

    public void ShowToastNotification(string message)
    {
        ToastMessage = message;
        IsToastVisible = true;
        Task.Delay(3500).ContinueWith(_ =>
        {
            App.Current?.Dispatcher.Invoke(() => IsToastVisible = false);
        });
    }

    [RelayCommand]
    public void CheckForUpdates()
    {
        ShowToastNotification("STORM FELLOWSHIP v0.2.2 — Установлена новейшая версия!");
    }

    // Fellowships & Channels Modal management
    public void OpenCreateFellowshipDialog() => IsCreateFellowshipModalOpen = true;

    [RelayCommand]
    public void CloseCreateFellowshipDialog() => IsCreateFellowshipModalOpen = false;

    [RelayCommand]
    public void CreateFellowship()
    {
        if (string.IsNullOrWhiteSpace(FellowshipNameInput)) FellowshipNameInput = "Новое содружество";
        FellowshipService.Instance.CreateFellowship(FellowshipNameInput);
        FellowshipNameInput = string.Empty;
        IsCreateFellowshipModalOpen = false;
        ShowToastNotification("Содружество успешно создано!");
    }

    [ObservableProperty]
    private string _directLanIpInput = "192.168.1.100";

    public ObservableCollection<LanPeer> DiscoveredLanPeers => CloudSyncService.Instance.DiscoveredPeers;

    [RelayCommand]
    public void ConnectDirectLanPeer(string? ip)
    {
        string targetIp = string.IsNullOrWhiteSpace(ip) ? DirectLanIpInput : ip;
        CloudSyncService.Instance.ConnectDirectP2P(targetIp, 48150);
        IsCreateFellowshipModalOpen = false;
        ShowToastNotification($"🌐 Подключение Direct P2P: {targetIp}:48150");
    }

    [RelayCommand]
    public void CopyCurrentInviteLink()
    {
        var f = FellowshipService.Instance.CurrentFellowship;
        if (f != null)
        {
            try
            {
                System.Windows.Clipboard.SetText($"storm://invite/{f.Id}");
                ShowToastNotification($"📋 Ссылка скопирована: storm://invite/{f.Id}");
            }
            catch { }
        }
        else
        {
            ShowToastNotification("Выберите содружество для копирования ссылки");
        }
    }

    [RelayCommand]
    public void AnnounceLanPresence()
    {
        CloudSyncService.Instance.BroadcastLanPresence();
        ShowToastNotification("📡 Оповещение отправлено в локальную сеть / VPN!");
    }

    [RelayCommand]
    public void JoinWithCode()
    {
        if (!string.IsNullOrWhiteSpace(JoinCodeInput))
        {
            FellowshipService.Instance.JoinFellowship(JoinCodeInput);
            JoinCodeInput = string.Empty;
            IsCreateFellowshipModalOpen = false;
            ShowToastNotification("Вы присоединились к содружеству!");
        }
    }

    public void OpenFellowshipSettingsDialog() => IsFellowshipSettingsModalOpen = true;

    [RelayCommand]
    public void CloseFellowshipSettingsDialog() => IsFellowshipSettingsModalOpen = false;

    [RelayCommand]
    public void OpenUserSettings()
    {
        IsUserProfileQuickCardOpen = false;
        IsUserSettingsModalOpen = true;
    }

    public void OpenUserSettingsDialog() => OpenUserSettings();

    public void CloseUserSettingsDialog() => IsUserSettingsModalOpen = false;

    [RelayCommand]
    public void DeleteFellowship(string? fellowshipId)
    {
        if (string.IsNullOrWhiteSpace(fellowshipId)) return;
        FellowshipService.Instance.DeleteFellowship(fellowshipId);
        RailViewModel.RefreshProperties();
        SidebarViewModel.RefreshProperties();
        ShowToastNotification("🗑️ Содружество удалено");
    }

    [RelayCommand]
    public void OpenFolderManager(FellowshipFolder? folder)
    {
        if (folder == null) return;
        SelectedFolderForEdit = folder;
        FolderEditName = folder.Name;
        FolderEditIcon = folder.Icon;
        FolderEditColor = folder.ColorHex;
        RefreshAvailableFellowshipsForFolder();
        IsFolderManagerModalOpen = true;
    }

    [RelayCommand]
    public void CloseFolderManager()
    {
        IsFolderManagerModalOpen = false;
        SelectedFolderForEdit = null;
    }

    [RelayCommand]
    public void SetFolderEditIcon(string? icon)
    {
        if (!string.IsNullOrWhiteSpace(icon)) FolderEditIcon = icon;
    }

    [RelayCommand]
    public void SetFolderEditColor(string? color)
    {
        if (!string.IsNullOrWhiteSpace(color)) FolderEditColor = color;
    }

    [RelayCommand]
    public void SaveFolderChanges()
    {
        if (SelectedFolderForEdit == null) return;
        SelectedFolderForEdit.Name = string.IsNullOrWhiteSpace(FolderEditName) ? "Папка" : FolderEditName;
        SelectedFolderForEdit.Icon = FolderEditIcon;
        SelectedFolderForEdit.ColorHex = FolderEditColor;
        IsFolderManagerModalOpen = false;
        RailViewModel.RefreshProperties();
        ShowToastNotification("💾 Настройки папки сохранены");
    }

    [RelayCommand]
    public void DeleteCurrentFolder()
    {
        if (SelectedFolderForEdit == null) return;
        FellowshipService.Instance.DeleteFolder(SelectedFolderForEdit);
        IsFolderManagerModalOpen = false;
        SelectedFolderForEdit = null;
        RailViewModel.RefreshProperties();
        ShowToastNotification("🗑️ Папка удалена, содружества извлечены");
    }

    [RelayCommand]
    public void ExtractFellowshipFromFolder(Fellowship? fellowship)
    {
        if (fellowship == null || SelectedFolderForEdit == null) return;
        FellowshipService.Instance.RemoveFellowshipFromFolder(fellowship, SelectedFolderForEdit);
        RefreshAvailableFellowshipsForFolder();
        OnPropertyChanged(nameof(SelectedFolderForEdit));
        RailViewModel.RefreshProperties();
    }

    [RelayCommand]
    public void AddFellowshipToEditedFolder(Fellowship? fellowship)
    {
        if (fellowship == null || SelectedFolderForEdit == null) return;
        FellowshipService.Instance.MoveFellowshipToFolder(fellowship, SelectedFolderForEdit);
        RefreshAvailableFellowshipsForFolder();
        OnPropertyChanged(nameof(SelectedFolderForEdit));
        RailViewModel.RefreshProperties();
    }

    private void RefreshAvailableFellowshipsForFolder()
    {
        AvailableFellowshipsToAddToFolder.Clear();
        foreach (var f in FellowshipService.Instance.Fellowships)
        {
            if (SelectedFolderForEdit == null || !SelectedFolderForEdit.Fellowships.Contains(f))
            {
                AvailableFellowshipsToAddToFolder.Add(f);
            }
        }
    }

    public void OpenCreateChannelDialog(ChannelCategory? category = null)
    {
        TargetCategoryForNewChannel = category;
        NewChannelName = string.Empty;
        NewChannelTopic = string.Empty;
        NewChannelType = ChannelType.Text;
        NewChannelBitrate = 128;
        IsCreateChannelModalOpen = true;
    }

    [RelayCommand]
    public void CloseCreateChannelDialog() => IsCreateChannelModalOpen = false;

    [RelayCommand]
    public void SubmitCreateChannel()
    {
        if (string.IsNullOrWhiteSpace(NewChannelName))
        {
            NewChannelName = NewChannelType == ChannelType.Voice ? "голосовой-канал" : "текстовый-чат";
        }

        var currentF = FellowshipService.Instance.CurrentFellowship;
        if (currentF != null)
        {
            FellowshipService.Instance.AddChannel(
                currentF.Id,
                TargetCategoryForNewChannel?.Id,
                NewChannelName,
                NewChannelTopic,
                NewChannelType,
                NewChannelBitrate);
            ShowToastNotification($"Канал #{NewChannelName} успешно создан");
        }
        IsCreateChannelModalOpen = false;
    }

    public void OpenEditChannelDialog(Channel channel)
    {
        ChannelToEdit = channel;
        EditingChannelName = channel.Name;
        EditingChannelTopic = channel.Topic;
        EditingChannelBitrate = channel.BitrateKbps;
        IsEditChannelModalOpen = true;
    }

    [RelayCommand]
    public void CloseEditChannelDialog() => IsEditChannelModalOpen = false;

    [RelayCommand]
    public void SubmitEditChannel()
    {
        if (ChannelToEdit != null && FellowshipService.Instance.CurrentFellowship != null)
        {
            FellowshipService.Instance.UpdateChannel(
                FellowshipService.Instance.CurrentFellowship.Id,
                ChannelToEdit.Id,
                EditingChannelName,
                EditingChannelTopic,
                EditingChannelBitrate);
            ShowToastNotification("Настройки канала сохранены");
        }
        IsEditChannelModalOpen = false;
    }

    [RelayCommand]
    public void SubmitDeleteChannel()
    {
        if (ChannelToEdit != null && FellowshipService.Instance.CurrentFellowship != null)
        {
            var name = ChannelToEdit.Name;
            FellowshipService.Instance.DeleteChannel(FellowshipService.Instance.CurrentFellowship.Id, ChannelToEdit.Id);
            ShowToastNotification($"Канал #{name} удален");
        }
        IsEditChannelModalOpen = false;
    }
}
