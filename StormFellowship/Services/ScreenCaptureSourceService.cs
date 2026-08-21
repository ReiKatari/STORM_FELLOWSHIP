using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace StormFellowship.Services;

public record RealCaptureSource(
    string Id,
    string Title,
    string ProcessName,
    string Resolution,
    bool IsScreen,
    IntPtr Hwnd
);

/// <summary>
/// Hardware Screen & Window Capture Enumerator Service.
/// Uses native Win32 APIs (EnumWindows, EnumDisplayMonitors) to dynamically discover
/// actual physical monitors and open application windows on the user''s system.
/// </summary>
public static class ScreenCaptureSourceService
{
    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    private const uint MONITORINFOF_PRIMARY = 0x00000001;

    public static List<RealCaptureSource> GetRealCaptureSources()
    {
        var list = new List<RealCaptureSource>();

        // 1. Enumerate Real Physical Monitors using Win32 API
        int monitorIndex = 1;
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMon, IntPtr hdc, ref RECT rc, IntPtr data) =>
        {
            var mi = new MONITORINFO();
            mi.cbSize = Marshal.SizeOf(mi);
            if (GetMonitorInfo(hMon, ref mi))
            {
                int width = mi.rcMonitor.Right - mi.rcMonitor.Left;
                int height = mi.rcMonitor.Bottom - mi.rcMonitor.Top;
                bool isPrimary = (mi.dwFlags & MONITORINFOF_PRIMARY) != 0;
                string primaryBadge = isPrimary ? " [Основной]" : "";

                list.Add(new RealCaptureSource(
                    Id: $"screen_{monitorIndex}",
                    Title: $"🖥️ Дисплей {monitorIndex}{primaryBadge} ({width}x{height})",
                    ProcessName: "Физический экран",
                    Resolution: $"{width}x{height} @ 60/144 Hz",
                    IsScreen: true,
                    Hwnd: IntPtr.Zero
                ));
                monitorIndex++;
            }
            return true;
        }, IntPtr.Zero);

        // 2. Enumerate Real Open Application Windows
        var currentPid = Process.GetCurrentProcess().Id;

        EnumWindows((hWnd, lParam) =>
        {
            if (!IsWindowVisible(hWnd)) return true;

            int length = GetWindowTextLength(hWnd);
            if (length == 0) return true;

            GetWindowRect(hWnd, out var rect);
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;
            if (width < 150 || height < 150) return true;

            var sb = new StringBuilder(length + 1);
            GetWindowText(hWnd, sb, sb.Capacity);
            string title = sb.ToString().Trim();

            if (string.IsNullOrWhiteSpace(title)) return true;

            // Filter out internal and desktop system windows
            if (title == "Program Manager" || title == "Windows Input Experience" || title == "Settings") return true;

            GetWindowThreadProcessId(hWnd, out uint pid);
            if (pid == currentPid) return true;

            string procName = "Приложение";
            try
            {
                using var proc = Process.GetProcessById((int)pid);
                procName = proc.ProcessName;
            }
            catch { }

            list.Add(new RealCaptureSource(
                Id: $"win_{hWnd.ToInt64()}",
                Title: $"🪟 {title}",
                ProcessName: procName,
                Resolution: $"{width}x{height} (Окно)",
                IsScreen: false,
                Hwnd: hWnd
            ));

            return true;
        }, IntPtr.Zero);

        return list;
    }
}
