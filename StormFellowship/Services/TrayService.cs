using System.Runtime.InteropServices;

namespace StormFellowship.Services;

public class TrayService : ITrayService, IDisposable
{
    private static TrayService? _instance;
    public static TrayService Instance => _instance ??= new TrayService();

    private nint _hwnd;
    private bool _isInitialized;
    private nint _hIcon;

    public bool IsInitialized => _isInitialized;

    public void Initialize(nint hwnd, string tooltip = "STORM FELLOWSHIP v0.0.1")
    {
        if (_isInitialized) return;
        _hwnd = hwnd;

        try
        {
            // Load custom app icon from Assets
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string iconPath = System.IO.Path.Combine(baseDir, "Assets", "AppIcon.ico");
            if (System.IO.File.Exists(iconPath))
            {
                _hIcon = LoadImage(nint.Zero, iconPath, IMAGE_ICON, 16, 16, LR_LOADFROMFILE);
            }

            if (_hIcon == nint.Zero)
            {
                _hIcon = LoadIcon(nint.Zero, IDI_APPLICATION);
            }

            var nid = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = _hwnd,
                uID = 1001,
                uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP,
                uCallbackMessage = WM_TRAYICON,
                hIcon = _hIcon,
                szTip = tooltip
            };

            Shell_NotifyIcon(NIM_ADD, ref nid);
            _isInitialized = true;
        }
        catch
        {
            // Safe fallback
        }
    }

    public void ShowNotification(string title, string message)
    {
        if (!_isInitialized) return;

        var nid = new NOTIFYICONDATA
        {
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
            hWnd = _hwnd,
            uID = 1001,
            uFlags = NIF_INFO,
            szInfo = message,
            szInfoTitle = title,
            dwInfoFlags = NIIF_INFO
        };

        Shell_NotifyIcon(NIM_MODIFY, ref nid);
    }

    public void MinimizeToTray()
    {
        if (_hwnd != nint.Zero)
        {
            ShowWindow(_hwnd, SW_HIDE);
        }
    }

    public void RestoreFromTray()
    {
        if (_hwnd != nint.Zero)
        {
            ShowWindow(_hwnd, SW_RESTORE);
            SetForegroundWindow(_hwnd);
        }
    }

    public void Dispose()
    {
        if (_isInitialized)
        {
            var nid = new NOTIFYICONDATA
            {
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATA>(),
                hWnd = _hwnd,
                uID = 1001
            };
            Shell_NotifyIcon(NIM_DELETE, ref nid);
            if (_hIcon != nint.Zero)
            {
                DestroyIcon(_hIcon);
            }
            _isInitialized = false;
        }
    }

    #region Win32 Interop
    private const int NIM_ADD = 0x00000000;
    private const int NIM_MODIFY = 0x00000001;
    private const int NIM_DELETE = 0x00000002;

    private const int NIF_MESSAGE = 0x00000001;
    private const int NIF_ICON = 0x00000002;
    private const int NIF_TIP = 0x00000004;
    private const int NIF_INFO = 0x00000010;

    private const int NIIF_INFO = 0x00000001;
    private const int WM_TRAYICON = 0x8000 + 100;

    private const int SW_HIDE = 0;
    private const int SW_RESTORE = 9;
    private const int IMAGE_ICON = 1;
    private const int LR_LOADFROMFILE = 0x00000010;
    private const nint IDI_APPLICATION = 32512;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct NOTIFYICONDATA
    {
        public uint cbSize;
        public nint hWnd;
        public uint uID;
        public uint uFlags;
        public uint uCallbackMessage;
        public nint hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
        public uint dwState;
        public uint dwStateMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string szInfo;
        public uint uTimeoutOrVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szInfoTitle;
        public uint dwInfoFlags;
        public Guid guidItem;
        public nint hBalloonIcon;
    }

    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern nint LoadImage(nint hInst, string name, uint type, int cx, int cy, uint fuLoad);

    [DllImport("user32.dll")]
    private static extern nint LoadIcon(nint hInstance, nint lpIconName);

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(nint hIcon);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(nint hWnd);
    #endregion
}
