using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _windowTitle = "STORM FELLOWSHIP v0.1.1";

    [ObservableProperty]
    private bool _isCreateFellowshipModalOpen;

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
    private bool _isEmotePickerOpen = false;

    [ObservableProperty]
    private bool _isSoundboardModalOpen = false;

    [ObservableProperty]
    private bool _isQuickSwitcherModalOpen = false;

    public ObservableCollection<SoundboardTrack> SoundboardTracks => SoundboardService.Instance.Tracks;

    public double SidebarWidth => IsSidebarCompact ? 56.0 : 240.0;

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
    private string _newPollOption1 = string.Empty;

    [ObservableProperty]
    private string _newPollOption2 = string.Empty;

    [ObservableProperty]
    private string _newPollOption3 = string.Empty;

    [ObservableProperty]
    private string _newPollOption4 = string.Empty;

    // Screen Share Settings
    [ObservableProperty]
    private string _screenShareSource = "Весь экран (1920x1080)";

    [ObservableProperty]
    private string _screenShareQuality = "1080p 60 FPS (Высокое качество)";

    [ObservableProperty]
    private bool _screenShareIncludeAudio = true;

    public ObservableCollection<string> ScreenShareSources { get; } = new()
    {
        "Весь экран (1920x1080)",
        "Окно игры (DirectX/Vulkan Fullscreen)",
        "Окно браузера / Приложения"
    };

    public ObservableCollection<string> ScreenShareQualities { get; } = new()
    {
        "1080p 60 FPS (Высокое качество)",
        "1440p 60 FPS (2K Ultra)",
        "4K 60 FPS (Ultra HD)",
        "1080p 120 FPS (Киберспортивный)"
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

    public MainViewModel CreateFellowshipModalViewModel => this;
    public MainViewModel FellowshipSettingsModalViewModel => this;
    public MainViewModel CreateChannelModalViewModel => this;
    public MainViewModel EditChannelModalViewModel => this;

    public string E2EEFingerprint => EncryptionService.Instance.Fingerprint;

    public MainViewModel()
    {
        RailViewModel = new FellowshipRailViewModel(this);
        SidebarViewModel = new ChannelSidebarViewModel(this);
        ChatViewModel = new ChatViewModel(this);
        CallViewModel = new CallViewModel(this);
        MemberListViewModel = new MemberListViewModel(this);
        UserSettingsViewModel = new UserSettingsViewModel(this);
        RoleManagementViewModel = new RoleManagementViewModel(this);

        CallService.Instance.CallStateChanged += (call) =>
        {
            IsInCallView = call != null;
        };
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
        NewPollOption1 = string.Empty;
        NewPollOption2 = string.Empty;
        NewPollOption3 = string.Empty;
        NewPollOption4 = string.Empty;
        IsCreatePollModalOpen = true;
    }

    [RelayCommand]
    public void SubmitCreatePoll()
    {
        if (string.IsNullOrWhiteSpace(NewPollQuestion) || string.IsNullOrWhiteSpace(NewPollOption1) || string.IsNullOrWhiteSpace(NewPollOption2))
        {
            ShowToastNotification("Введите вопрос и как минимум 2 варианта ответа");
            return;
        }

        var poll = new PollItem
        {
            Question = NewPollQuestion,
            AuthorName = FellowshipService.Instance.CurrentUser.DisplayName
        };
        poll.Options.Add(new PollOption { Text = NewPollOption1 });
        poll.Options.Add(new PollOption { Text = NewPollOption2 });
        if (!string.IsNullOrWhiteSpace(NewPollOption3)) poll.Options.Add(new PollOption { Text = NewPollOption3 });
        if (!string.IsNullOrWhiteSpace(NewPollOption4)) poll.Options.Add(new PollOption { Text = NewPollOption4 });
        poll.RecalculatePercentages();

        var channelId = FellowshipService.Instance.CurrentChannel?.Id ?? "general";
        var msg = new ChatMessage
        {
            ChannelId = channelId,
            Author = FellowshipService.Instance.CurrentUser,
            Content = "Опрос сообщества:",
            Poll = poll,
            Timestamp = DateTime.Now
        };

        FellowshipService.Instance.CurrentChannel?.Messages.Add(msg);
        IsCreatePollModalOpen = false;
        ShowToastNotification("📊 Опрос успешно опубликован в чате!");
    }

    [RelayCommand]
    public void CloseCreatePollDialog()
    {
        IsCreatePollModalOpen = false;
    }

    [RelayCommand]
    public void OpenScreenShareDialog()
    {
        IsScreenShareModalOpen = true;
    }

    [RelayCommand]
    public void SubmitStartScreenShare()
    {
        IsScreenShareModalOpen = false;
        CallService.Instance.ToggleScreenShare();
        ShowToastNotification($"🖥️ Трансляция экрана запущена ({ScreenShareQuality}, WASAPI: {(ScreenShareIncludeAudio ? "Вкл" : "Выкл")})");
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
    public void ToggleGlassBubbles()
    {
        IsGlassBubblesMode = !IsGlassBubblesMode;
        ShowToastNotification(IsGlassBubblesMode ? "Режим Floating Glass Bubbles включен" : "Классический режим сообщений");
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
        IsSoundboardModalOpen = false;
        IsQuickSwitcherModalOpen = false;
    }

    public void ToggleMemberList()
    {
        IsMemberListVisible = !IsMemberListVisible;
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
        ShowToastNotification("STORM FELLOWSHIP v0.1.0 — Установлена новейшая версия!");
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

    public void OpenUserSettingsDialog() => IsUserSettingsModalOpen = true;

    public void CloseUserSettingsDialog() => IsUserSettingsModalOpen = false;

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
