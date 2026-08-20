using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using StormFellowship.Services;
using StormFellowship.ViewModels;
using WinRT.Interop;

namespace StormFellowship;

public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        ViewModel = new MainViewModel();
        InitializeComponent();

        BindViewModels();
        ConfigureWindow();
    }

    private void BindViewModels()
    {
        AppTitleBar.ViewModel = ViewModel;
        FellowshipRail.ViewModel = ViewModel.RailVM;
        ChannelSidebar.ViewModel = ViewModel.SidebarVM;
        MainChatView.ViewModel = ViewModel.ChatVM;
        CallOverlay.ViewModel = ViewModel.CallVM;
        MemberList.ViewModel = ViewModel.MemberListVM;
        CreateFellowshipModal.ViewModel = ViewModel;
        FellowshipSettingsModal.ViewModel = ViewModel;
        UserSettingsModal.ViewModel = ViewModel.UserSettingsVM;

        ViewModel.PropertyChanged += (s, e) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.PropertyName == nameof(ViewModel.IsCallOverlayVisible))
                {
                    CallOverlay.Visibility = ViewModel.IsCallOverlayVisible ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (e.PropertyName == nameof(ViewModel.IsMemberListVisible))
                {
                    MemberList.Visibility = ViewModel.IsMemberListVisible ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (e.PropertyName == nameof(ViewModel.IsCreateFellowshipModalOpen))
                {
                    CreateFellowshipModal.Visibility = ViewModel.IsCreateFellowshipModalOpen ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (e.PropertyName == nameof(ViewModel.IsFellowshipSettingsModalOpen))
                {
                    FellowshipSettingsModal.Visibility = ViewModel.IsFellowshipSettingsModalOpen ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (e.PropertyName == nameof(ViewModel.IsUserSettingsModalOpen))
                {
                    UserSettingsModal.Visibility = ViewModel.IsUserSettingsModalOpen ? Visibility.Visible : Visibility.Collapsed;
                }
            });
        };

        CallOverlay.Visibility = ViewModel.IsCallOverlayVisible ? Visibility.Visible : Visibility.Collapsed;
        MemberList.Visibility = ViewModel.IsMemberListVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ConfigureWindow()
    {
        var hwnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow != null)
        {
            appWindow.Title = "STORM FELLOWSHIP v0.0.1";
            appWindow.Resize(new Windows.Graphics.SizeInt32(1280, 800));

            string iconPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "AppIcon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                appWindow.SetIcon(iconPath);
            }
        }

        TrayService.Instance.Initialize(hwnd, "STORM FELLOWSHIP v0.0.1");
    }

    private void OnToggleMemberListRequested(object sender, RoutedEventArgs e)
    {
        ViewModel.ToggleMemberList();
    }

    private void OnUserSettingsCloseRequested(object sender, RoutedEventArgs e)
    {
        ViewModel.CloseUserSettingsDialog();
    }
}
