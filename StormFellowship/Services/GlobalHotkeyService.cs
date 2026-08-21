using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace StormFellowship.Services;

/// <summary>
/// Low-level Windows Global Input Hook (WH_KEYBOARD_LL and WH_MOUSE_LL).
/// Intercepts Push-to-Talk, Mouse 4, Mouse 5, and custom combo hotkeys even inside full-screen games.
/// </summary>
public class GlobalHotkeyService : IDisposable
{
    private static GlobalHotkeyService? _instance;
    public static GlobalHotkeyService Instance => _instance ??= new GlobalHotkeyService();

    private const int WH_KEYBOARD_LL = 13;
    private const int WH_MOUSE_LL = 14;
    private const int WM_KEYDOWN = 0x0100;
    private const int WM_KEYUP = 0x0101;
    private const int WM_SYSKEYDOWN = 0x0104;
    private const int WM_SYSKEYUP = 0x0105;
    private const int WM_XBUTTONDOWN = 0x020B;
    private const int WM_XBUTTONUP = 0x020C;
    private const int XBUTTON1 = 0x0001; // Mouse 4
    private const int XBUTTON2 = 0x0002; // Mouse 5

    private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

    private readonly LowLevelProc _keyboardProc;
    private readonly LowLevelProc _mouseProc;
    private IntPtr _keyboardHook = IntPtr.Zero;
    private IntPtr _mouseHook = IntPtr.Zero;

    public event Action<string, bool>? HotkeyStateChanged; // (hotkeyName, isPressed)
    public event Action? InstantClipTriggered;
    public event Action? ToggleOverlayTriggered;

    public bool IsPushToTalkPressed { get; private set; }
    public string ConfiguredPttKey { get; set; } = "Mouse4"; // "Mouse4", "Mouse5", "Caps", "LAlt", "V"

    public GlobalHotkeyService()
    {
        _keyboardProc = KeyboardHookCallback;
        _mouseProc = MouseHookCallback;
        InstallHooks();
    }

    private void InstallHooks()
    {
        try
        {
            using var curProcess = Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            if (curModule != null)
            {
                IntPtr moduleHandle = GetModuleHandle(curModule.ModuleName);
                _keyboardHook = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, moduleHandle, 0);
                _mouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);
            }
        }
        catch { }
    }

    private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            bool isDown = (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN);
            bool isUp = (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP);

            // Shift + ~ (Tilde/VK 192) for Overlay
            if (isDown && vkCode == 192 && (GetKeyState(0x10) & 0x8000) != 0) // VK_SHIFT = 0x10
            {
                ToggleOverlayTriggered?.Invoke();
            }

            // Alt + F10 (VK_F10 = 0x79) for Instant Clip
            if (isDown && vkCode == 0x79 && (GetKeyState(0x12) & 0x8000) != 0)
            {
                InstantClipTriggered?.Invoke();
            }

            // Push to Talk matching
            if (ConfiguredPttKey == "Caps" && vkCode == 0x14) // VK_CAPITAL
            {
                SetPttState(isDown);
            }
            else if (ConfiguredPttKey == "LAlt" && vkCode == 0x12) // VK_MENU
            {
                SetPttState(isDown);
            }
            else if (ConfiguredPttKey == "V" && vkCode == 0x56)
            {
                SetPttState(isDown);
            }
        }
        return CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
    }

    private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0)
        {
            int msg = (int)wParam;
            if (msg == WM_XBUTTONDOWN || msg == WM_XBUTTONUP)
            {
                int mouseData = (int)((Marshal.ReadInt64(lParam + 8) >> 16) & 0xFFFF);
                bool isDown = (msg == WM_XBUTTONDOWN);

                if (mouseData == XBUTTON1 && (ConfiguredPttKey == "Mouse4" || ConfiguredPttKey == "Боковая кнопка 4"))
                {
                    SetPttState(isDown);
                }
                else if (mouseData == XBUTTON2 && (ConfiguredPttKey == "Mouse5" || ConfiguredPttKey == "Боковая кнопка 5"))
                {
                    SetPttState(isDown);
                }
            }
        }
        return CallNextHookEx(_mouseHook, nCode, wParam, lParam);
    }

    private void SetPttState(bool isPressed)
    {
        if (IsPushToTalkPressed != isPressed)
        {
            IsPushToTalkPressed = isPressed;
            HotkeyStateChanged?.Invoke(ConfiguredPttKey, isPressed);
            if (AudioService.Instance.IsPushToTalkEnabled)
            {
                AudioService.Instance.IsMuted = !isPressed;
            }
        }
    }

    public void Dispose()
    {
        if (_keyboardHook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_keyboardHook);
            _keyboardHook = IntPtr.Zero;
        }
        if (_mouseHook != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_mouseHook);
            _mouseHook = IntPtr.Zero;
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern short GetKeyState(int nVirtKey);
}
