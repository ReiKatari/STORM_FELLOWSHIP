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
    private bool _isCallOverlayVisible = false;

    [ObservableProperty]
    private bool _isMemberListVisible = true;

    [ObservableProperty]
    private bool _isCreateFellowshipModalOpen = false;

    [ObservableProperty]
    private bool _isFellowshipSettingsModalOpen = false;

    [ObservableProperty]
    private bool _isUserSettingsModalOpen = false;

    [ObservableProperty]
    private bool _isImageViewerModalOpen = false;

    [ObservableProperty]
    private string _modalImageSource = string.Empty;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasStatusNotification = false;

    public FellowshipRailViewModel RailVM { get; }
    public ChannelSidebarViewModel SidebarVM { get; }
    public ChatViewModel ChatVM { get; }
    public CallViewModel CallVM { get; }
    public MemberListViewModel MemberListVM { get; }
    public UserSettingsViewModel UserSettingsVM { get; }

    public MainViewModel()
    {
        RailVM = new FellowshipRailViewModel(this);
        SidebarVM = new ChannelSidebarViewModel(this);
        ChatVM = new ChatViewModel(this);
        CallVM = new CallViewModel(this);
        MemberListVM = new MemberListViewModel(this);
        UserSettingsVM = new UserSettingsViewModel(this);

        CallService.Instance.CallStateChanged += (call) =>
        {
            IsCallOverlayVisible = (call != null);
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
    public void OpenImageModal(string url)
    {
        ModalImageSource = url;
        IsImageViewerModalOpen = true;
    }

    [RelayCommand]
    public void CloseImageModal()
    {
        IsImageViewerModalOpen = false;
    }

    public void ShowToastNotification(string message)
    {
        StatusMessage = message;
        HasStatusNotification = true;
        Task.Delay(3500).ContinueWith(_ =>
        {
            HasStatusNotification = false;
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }
}
