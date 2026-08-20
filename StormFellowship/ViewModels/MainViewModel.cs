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

    public FellowshipRailViewModel RailViewModel { get; }
    public ChannelSidebarViewModel SidebarViewModel { get; }
    public ChatViewModel ChatViewModel { get; }
    public CallViewModel CallViewModel { get; }
    public MemberListViewModel MemberListViewModel { get; }
    public UserSettingsViewModel UserSettingsViewModel { get; }
    public MainViewModel CreateFellowshipModalViewModel => this;
    public MainViewModel FellowshipSettingsModalViewModel => this;

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

    [RelayCommand]
    public void Close()
    {
        IsCreateFellowshipModalOpen = false;
        IsFellowshipSettingsModalOpen = false;
        IsUserSettingsModalOpen = false;
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
