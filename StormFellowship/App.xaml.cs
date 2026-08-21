using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using StormFellowship.Services;

namespace StormFellowship;

public partial class App : Application
{
    public const string ShowWindowMessageName = "STORM_FELLOWSHIP_SHOW_WINDOW_MSG_V013";

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int RegisterWindowMessage(string lpString);

    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    public static int ShowWindowMessage { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        // Ensure no hidden orphan background instances block the current launch
        try
        {
            int currentPid = Process.GetCurrentProcess().Id;
            var existingProcesses = Process.GetProcessesByName("StormFellowship");
            foreach (var proc in existingProcesses)
            {
                if (proc.Id != currentPid)
                {
                    try { proc.Kill(); } catch { }
                }
            }
        }
        catch { }

        base.OnStartup(e);

        this.DispatcherUnhandledException += (s, args) =>
        {
            try
            {
                string log = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
                var sb = new StringBuilder();
                Exception? ex = args.Exception;
                while (ex != null)
                {
                    sb.AppendLine($"[ERROR] {ex.GetType().FullName}: {ex.Message}");
                    sb.AppendLine($"Stack:\n{ex.StackTrace}\n");
                    ex = ex.InnerException;
                }
                File.WriteAllText(log, sb.ToString());
                MessageBox.Show(sb.ToString(), "STORM FELLOWSHIP Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        };
    }

    public static void ForceExit()
    {
        try
        {
            TrayService.Instance.Dispose();
            AudioService.Instance.Dispose();
        }
        catch { }
        finally
        {
            Environment.Exit(0);
        }
    }
}
