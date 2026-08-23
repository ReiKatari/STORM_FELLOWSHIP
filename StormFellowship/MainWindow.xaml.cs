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
    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr CreatePopupMenu();

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern int TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y, int nReserved, IntPtr hWnd, IntPtr prcRect);

    [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
    private static extern bool DestroyMenu(IntPtr hMenu);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    private const int MF_STRING = 0x00000000;
    private const int MF_SEPARATOR = 0x00000800;
    private const uint TPM_RETURNCMD = 0x0100;
    private const uint TPM_NONOTIFY = 0x0080;
    private const uint TPM_LEFTALIGN = 0x0000;
    private const uint TPM_RIGHTBUTTON = 0x0002;

    private const int CMD_OPEN = 1;
    private const int CMD_MUTE = 2;
    private const int CMD_CLEAN_RAM = 4;
    private const int CMD_GAME_BOOST = 5;
    private const int CMD_EXIT = 3;
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
            TrayService.Instance.Initialize(hwnd, "STORM FELLOWSHIP 0.2.3");

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
        try
        {
            IntPtr hMenu = CreatePopupMenu();
            if (hMenu == IntPtr.Zero) return;

            AppendMenu(hMenu, MF_STRING, CMD_OPEN, "Открыть STORM FELLOWSHIP 0.2.3");
            
            string muteText = AudioService.Instance.IsMuted ? "Включить микрофон (Unmute)" : "Выключить микрофон (Mute)";
            AppendMenu(hMenu, MF_STRING, CMD_MUTE, muteText);
            AppendMenu(hMenu, MF_SEPARATOR, 0, string.Empty);
            AppendMenu(hMenu, MF_STRING, CMD_CLEAN_RAM, "Очистить оперативную память (RAM)");
            AppendMenu(hMenu, MF_STRING, CMD_GAME_BOOST, "Активировать игровой режим (Game Boost)");
            
            AppendMenu(hMenu, MF_SEPARATOR, 0, string.Empty);
            AppendMenu(hMenu, MF_STRING, CMD_EXIT, "Закрыть STORM FELLOWSHIP");

            GetCursorPos(out POINT pt);
            
            var hwnd = new WindowInteropHelper(this).Handle;
            SetForegroundWindow(hwnd);
            
            int cmd = TrackPopupMenu(hMenu, TPM_RETURNCMD | TPM_NONOTIFY | TPM_LEFTALIGN | TPM_RIGHTBUTTON,
                pt.X, pt.Y, 0, hwnd, IntPtr.Zero);

            DestroyMenu(hMenu);

            switch (cmd)
            {
                case CMD_OPEN:
                    Show();
                    WindowState = WindowState.Normal;
                    Activate();
                    break;
                case CMD_MUTE:
                    AudioService.Instance.IsMuted = !AudioService.Instance.IsMuted;
                    break;
                case CMD_CLEAN_RAM:
                    _ = System.Threading.Tasks.Task.Run(() => { MemoryOptimizerService.Instance.PurgeStandbyList(); MemoryOptimizerService.Instance.PurgeWorkingSets(); });
                    TrayService.Instance.ShowNotification("Очистка RAM", "Рабочие наборы и Standby List успешно очищены.");
                    break;
                case CMD_GAME_BOOST:
                    _ = System.Threading.Tasks.Task.Run(() => { GameBoostService.Instance.ActivateGameBoost(); });
                    TrayService.Instance.ShowNotification("Game Boost", "Игровой профиль наивысшего приоритета активирован.");
                    break;
                case CMD_EXIT:
                    App.ForceExit();
                    break;
            }
        }
        catch { }
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


