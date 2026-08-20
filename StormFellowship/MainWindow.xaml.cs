using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        Loaded += (s, e) =>
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            TrayService.Instance.Initialize(hwnd, "STORM FELLOWSHIP v0.0.3");
        };

        Closing += (s, e) =>
        {
            TrayService.Instance.Dispose();
            AudioService.Instance.StopMicMonitoring();
        };
    }

    private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (DataContext is MainViewModel vm)
            {
                if (vm.IsCreateFellowshipModalOpen ||
                    vm.IsFellowshipSettingsModalOpen ||
                    vm.IsUserSettingsModalOpen ||
                    vm.IsCreateChannelModalOpen ||
                    vm.IsEditChannelModalOpen)
                {
                    vm.CloseAllModals();
                    e.Handled = true;
                }
            }
        }
    }

    private void OnToggleMemberListRequested()
    {
        if (DataContext is MainViewModel vm)
        {
            vm.ToggleMemberList();
        }
    }
}
