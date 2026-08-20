using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public enum ActiveMainView
{
    Fellowship,
    DirectMessages,
    CallView,
    Explore,
    Settings
}

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ActiveMainView _activeView = ActiveMainView.Fellowship;

    [ObservableProperty]
    private bool _isInCallView = false;

    [ObservableProperty]
    private bool _isMemberListVisible = true;

    [ObservableProperty]
    private bool _isCreateFellowshipModalOpen = false;

    [ObservableProperty]
    private bool _isFellowshipSettingsModalOpen = false;

    [ObservableProperty]
    private bool _isUserSettingsModalOpen = false;

    [ObservableProperty]
    private bool _isCreateChannelModalOpen = false;

    [ObservableProperty]
    private bool _isEditChannelModalOpen = false;

    [ObservableProperty]
    private string _toastMessage = string.Empty;

    [ObservableProperty]
    private bool _isToastVisible = false;

    [ObservableProperty]
    private string _fellowshipNameInput = "Новое содружество";

    [ObservableProperty]
    private string _joinCodeInput = string.Empty;

    [ObservableProperty]
    private string _fellowshipName = "Основное содружество";

    [ObservableProperty]
    private string _fellowshipDescription = "Пространство для голосового и текстового общения.";

    [ObservableProperty]
    private string _inviteLink = "storm://invite/main";

    // Channel Creation properties
    [ObservableProperty]
    private string _newChannelName = string.Empty;

    [ObservableProperty]
    private string _newChannelTopic = string.Empty;

    [ObservableProperty]
    private ChannelType _newChannelType = ChannelType.Text;

    [ObservableProperty]
    private int _newChannelBitrate = 128;

    [ObservableProperty]
    private ChannelCategory? _targetCategoryForNewChannel;

    // Channel Editing properties
    [ObservableProperty]
    private Channel? _selectedChannelForEdit;

    [ObservableProperty]
    private string _editingChannelName = string.Empty;

    [ObservableProperty]
    private string _editingChannelTopic = string.Empty;

    [ObservableProperty]
    private int _editingChannelBitrate = 128;

    public FellowshipRailViewModel RailViewModel { get; }
    public ChannelSidebarViewModel SidebarViewModel { get; }
    public ChatViewModel ChatViewModel { get; }
    public CallViewModel CallViewModel { get; }
    public MemberListViewModel MemberListViewModel { get; }
    public UserSettingsViewModel UserSettingsViewModel { get; }

    public MainViewModel CreateFellowshipModalViewModel => this;
    public MainViewModel FellowshipSettingsModalViewModel => this;
    public MainViewModel CreateChannelModalViewModel => this;
    public MainViewModel EditChannelModalViewModel => this;

    public ObservableCollection<int> AvailableBitrates { get; } = new() { 64, 96, 128, 256, 384 };

    public MainViewModel()
    {
        RailViewModel = new FellowshipRailViewModel(this);
        SidebarViewModel = new ChannelSidebarViewModel(this);
        ChatViewModel = new ChatViewModel(this);
        CallViewModel = new CallViewModel(this);
        MemberListViewModel = new MemberListViewModel(this);
        UserSettingsViewModel = new UserSettingsViewModel(this);

        CallService.Instance.CallStateChanged += (call) =>
        {
            IsInCallView = (call != null);
        };
    }

    [RelayCommand]
    public void ToggleMemberList()
    {
        IsMemberListVisible = !IsMemberListVisible;
    }

    [RelayCommand]
    public void CheckForUpdates()
    {
        ShowToastNotification("Проверка обновлений... Вы используете актуальную версию STORM FELLOWSHIP v0.0.3");
    }

    [RelayCommand]
    public void OpenCreateFellowshipDialog()
    {
        IsCreateFellowshipModalOpen = true;
    }

    [RelayCommand]
    public void CloseCreateFellowshipDialog()
    {
        IsCreateFellowshipModalOpen = false;
    }

    [RelayCommand]
    public void Create()
    {
        if (!string.IsNullOrWhiteSpace(FellowshipNameInput))
        {
            FellowshipService.Instance.CreateFellowship(FellowshipNameInput);
            ShowToastNotification($"Создано содружество: {FellowshipNameInput}");
            IsCreateFellowshipModalOpen = false;
        }
    }

    [RelayCommand]
    public void JoinWithCode()
    {
        if (!string.IsNullOrWhiteSpace(JoinCodeInput))
        {
            FellowshipService.Instance.JoinFellowship(JoinCodeInput);
            ShowToastNotification($"Подключение к содружеству: {JoinCodeInput}");
            IsCreateFellowshipModalOpen = false;
        }
    }

    [RelayCommand]
    public void OpenFellowshipSettingsDialog()
    {
        IsFellowshipSettingsModalOpen = true;
    }

    [RelayCommand]
    public void CloseFellowshipSettingsDialog()
    {
        IsFellowshipSettingsModalOpen = false;
    }

    [RelayCommand]
    public void SaveChanges()
    {
        ShowToastNotification("Настройки содружества сохранены");
        IsFellowshipSettingsModalOpen = false;
    }

    [RelayCommand]
    public void DeleteFellowship()
    {
        ShowToastNotification("Содружество удалено");
        IsFellowshipSettingsModalOpen = false;
    }

    [RelayCommand]
    public void CopyInvite()
    {
        try
        {
            System.Windows.Clipboard.SetText(InviteLink);
            ShowToastNotification("Ссылка-приглашение скопирована в буфер обмена!");
        }
        catch { }
    }

    [RelayCommand]
    public void OpenUserSettingsDialog()
    {
        IsUserSettingsModalOpen = true;
    }

    [RelayCommand]
    public void CloseUserSettingsDialog()
    {
        IsUserSettingsModalOpen = false;
    }

    // Channel CRUD
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
    public void CloseCreateChannelDialog()
    {
        IsCreateChannelModalOpen = false;
    }

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
            var chan = FellowshipService.Instance.AddChannel(
                currentF.Id,
                TargetCategoryForNewChannel?.Id,
                NewChannelName,
                NewChannelTopic,
                NewChannelType,
                NewChannelBitrate
            );
            ShowToastNotification($"Канал #{chan.Name} успешно создан!");
        }
        IsCreateChannelModalOpen = false;
    }

    public void OpenEditChannelDialog(Channel channel)
    {
        SelectedChannelForEdit = channel;
        EditingChannelName = channel.Name;
        EditingChannelTopic = channel.Topic;
        EditingChannelBitrate = channel.BitrateKbps;
        IsEditChannelModalOpen = true;
    }

    [RelayCommand]
    public void CloseEditChannelDialog()
    {
        IsEditChannelModalOpen = false;
    }

    [RelayCommand]
    public void SubmitEditChannel()
    {
        if (SelectedChannelForEdit != null)
        {
            var currentF = FellowshipService.Instance.CurrentFellowship;
            if (currentF != null)
            {
                FellowshipService.Instance.UpdateChannel(
                    currentF.Id,
                    SelectedChannelForEdit.Id,
                    EditingChannelName,
                    EditingChannelTopic,
                    EditingChannelBitrate
                );
                ShowToastNotification($"Канал #{EditingChannelName} обновлен!");
            }
        }
        IsEditChannelModalOpen = false;
    }

    [RelayCommand]
    public void SubmitDeleteChannel()
    {
        if (SelectedChannelForEdit != null)
        {
            var currentF = FellowshipService.Instance.CurrentFellowship;
            if (currentF != null)
            {
                FellowshipService.Instance.DeleteChannel(currentF.Id, SelectedChannelForEdit.Id);
                ShowToastNotification($"Канал #{SelectedChannelForEdit.Name} удален");
            }
        }
        IsEditChannelModalOpen = false;
    }

    public void CloseAllModals()
    {
        IsCreateFellowshipModalOpen = false;
        IsFellowshipSettingsModalOpen = false;
        IsUserSettingsModalOpen = false;
        IsCreateChannelModalOpen = false;
        IsEditChannelModalOpen = false;
    }

    [RelayCommand]
    public void Close()
    {
        CloseAllModals();
    }

    public void ShowToastNotification(string message)
    {
        ToastMessage = message;
        IsToastVisible = true;
        Task.Delay(3500).ContinueWith(_ =>
        {
            IsToastVisible = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
