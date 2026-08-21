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
            TrayService.Instance.Initialize(hwnd, "STORM FELLOWSHIP v0.2.2");

            var source = HwndSource.FromHwnd(hwnd);
            source?.AddHook(WndProc);

            if (DataContext is MainViewModel vm)
            {
                EmotePopup.EmoteSelected += (emote) => vm.SelectEmoteFromPicker(emote);
                EmotePopup.Closed += () => vm.CloseEmotePicker();
            }
        };

        Closing += (s, e) =>
        {
            e.Cancel = true;
            Hide();
            TrayService.Instance.ShowNotification("STORM FELLOWSHIP", "Приложение свернуто в системный трей.");
        };
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if (e.Key == Key.Escape && DataContext is MainViewModel vm)
        {
            if (vm.IsAuthModalOpen)
            {
                vm.CloseAuthDialog();
                e.Handled = true;
            }
            else if (vm.IsFolderManagerModalOpen)
            {
                vm.CloseFolderManager();
                e.Handled = true;
            }
            else if (vm.IsCreatePollModalOpen)
            {
                vm.CloseCreatePollDialog();
                e.Handled = true;
            }
            else if (vm.IsCreateFellowshipModalOpen)
            {
                vm.CloseCreateFellowshipDialog();
                e.Handled = true;
            }
            else if (vm.IsCreateChannelModalOpen)
            {
                vm.CloseCreateChannelDialog();
                e.Handled = true;
            }
            else if (vm.IsUserSettingsModalOpen)
            {
                vm.CloseUserSettingsDialog();
                e.Handled = true;
            }
            else if (vm.IsFellowshipSettingsModalOpen)
            {
                vm.CloseFellowshipSettingsDialog();
                e.Handled = true;
            }
            else if (vm.IsUserProfileQuickCardOpen)
            {
                vm.IsUserProfileQuickCardOpen = false;
                e.Handled = true;
            }
            else if (vm.IsEmotePickerOpen)
            {
                vm.CloseEmotePicker();
                e.Handled = true;
            }
            else if (vm.IsScreenShareModalOpen)
            {
                vm.CloseScreenShareDialog();
                e.Handled = true;
            }
            else if (vm.IsSoundboardModalOpen)
            {
                vm.IsSoundboardModalOpen = false;
                e.Handled = true;
            }
            else if (vm.IsQuickSwitcherModalOpen)
            {
                vm.IsQuickSwitcherModalOpen = false;
                e.Handled = true;
            }
        }
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WM_TRAYICON = 0x8000 + 100;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_LBUTTONDBLCLK = 0x0203;
        const int WM_RBUTTONUP = 0x0205;

        if (msg == App.ShowWindowMessage && App.ShowWindowMessage != 0)
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
            handled = true;
            return IntPtr.Zero;
        }

        if (msg == WM_TRAYICON)
        {
            int eventId = lParam.ToInt32();
            if (eventId == WM_LBUTTONUP || eventId == WM_LBUTTONDBLCLK)
            {
                Show();
                WindowState = WindowState.Normal;
                Activate();
                handled = true;
            }
            else if (eventId == WM_RBUTTONUP)
            {
                ShowTrayContextMenu();
                handled = true;
            }
        }
        return IntPtr.Zero;
    }

    private void ShowTrayContextMenu()
    {
        var menu = new System.Windows.Controls.ContextMenu();

        var itemOpen = new System.Windows.Controls.MenuItem { Header = "⚡ Открыть STORM FELLOWSHIP" };
        itemOpen.Click += (s, e) =>
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        };

        var itemMute = new System.Windows.Controls.MenuItem { Header = "🎙️ Переключить микрофон (Mute)" };
        itemMute.Click += (s, e) =>
        {
            AudioService.Instance.IsMuted = !AudioService.Instance.IsMuted;
        };

        var itemExit = new System.Windows.Controls.MenuItem { Header = "❌ Выход из приложения" };
        itemExit.Click += (s, e) =>
        {
            App.ForceExit();
        };

        menu.Items.Add(itemOpen);
        menu.Items.Add(itemMute);
        menu.Items.Add(new System.Windows.Controls.Separator());
        menu.Items.Add(itemExit);

        menu.IsOpen = true;
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
