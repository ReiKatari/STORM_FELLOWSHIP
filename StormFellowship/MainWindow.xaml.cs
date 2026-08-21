using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using StormFellowship.Helpers;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainViewModel();

        try
        {
            var iconUri = new Uri("pack://application:,,,/Assets/AppIcon.png", UriKind.RelativeOrAbsolute);
            Icon = new BitmapImage(iconUri);
        }
        catch { }

        Loaded += (s, e) =>
        {
            WindowBackdropHelper.EnableMicaBackdrop(this);
            var hwnd = new WindowInteropHelper(this).Handle;
            TrayService.Instance.Initialize(hwnd, "STORM FELLOWSHIP v0.1.1");

            if (DataContext is MainViewModel vm)
            {
                EmotePopup.EmoteSelected += (emote) => vm.SelectEmoteFromPicker(emote);
                EmotePopup.Closed += () => vm.CloseEmotePicker();
            }
        };

        Closing += (s, e) =>
        {
            TrayService.Instance.Dispose();
            AudioService.Instance.StopMicMonitoring();
        };
    }

    private void OnWindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        if (e.Key == Key.Escape)
        {
            vm.CloseAllModals();
            e.Handled = true;
        }
        else if (e.Key == Key.K && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            vm.OpenQuickSwitcher();
            e.Handled = true;
        }
        else if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            vm.OpenSearchDialog();
            e.Handled = true;
        }
        else if (e.Key == Key.OemTilde && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
        {
            vm.ToggleGameOverlay();
            e.Handled = true;
        }
    }
}
