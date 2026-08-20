using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace StormFellowship.Installer;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string installDir = Path.Combine(localAppData, "StormFellowship");
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string startMenuDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "STORM FELLOWSHIP");

            // Look for Assembling directory
            string appBase = AppDomain.CurrentDomain.BaseDirectory;
            string sourceAssembling = Path.Combine(@"E:\STORM FELLOWSHIP\Assembling");
            if (!Directory.Exists(sourceAssembling))
            {
                sourceAssembling = Path.GetFullPath(Path.Combine(appBase, "..", "Assembling"));
            }

            Directory.CreateDirectory(installDir);
            Directory.CreateDirectory(startMenuDir);

            // Copy files from Assembling to Install Directory
            if (Directory.Exists(sourceAssembling))
            {
                CopyDirectory(sourceAssembling, installDir);
            }

            string exePath = Path.Combine(installDir, "StormFellowship.exe");
            string iconPath = Path.Combine(installDir, "Assets", "AppIcon.ico");

            // Create Desktop Shortcut
            string desktopShortcut = Path.Combine(desktopDir, "STORM FELLOWSHIP.lnk");
            CreateShortcut(desktopShortcut, exePath, installDir, iconPath, "STORM FELLOWSHIP - Next-gen communication platform");

            // Create Start Menu Shortcut
            string startMenuShortcut = Path.Combine(startMenuDir, "STORM FELLOWSHIP.lnk");
            CreateShortcut(startMenuShortcut, exePath, installDir, iconPath, "STORM FELLOWSHIP");

            // Write Registry Entries for Add/Remove Programs
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\StormFellowship"))
            {
                key.SetValue("DisplayName", "STORM FELLOWSHIP");
                key.SetValue("DisplayVersion", "0.0.1");
                key.SetValue("Publisher", "ReiKatari");
                key.SetValue("DisplayIcon", iconPath);
                key.SetValue("InstallLocation", installDir);
                key.SetValue("UninstallString", $"cmd.exe /c \"{Path.Combine(installDir, "Uninstall.cmd")}\"");
                key.SetValue("QuietUninstallString", $"cmd.exe /c \"{Path.Combine(installDir, "Uninstall.cmd")} /quiet\"");
                key.SetValue("NoModify", 1, RegistryValueKind.DWord);
                key.SetValue("NoRepair", 1, RegistryValueKind.DWord);
            }

            // Register URL Protocol: storm://
            using (var protocolKey = Registry.CurrentUser.CreateSubKey(@"Software\Classes\storm"))
            {
                protocolKey.SetValue("", "URL:STORM FELLOWSHIP Protocol");
                protocolKey.SetValue("URL Protocol", "");
                using (var cmdKey = protocolKey.CreateSubKey(@"shell\open\command"))
                {
                    cmdKey.SetValue("", $"\"{exePath}\" \"%1\"");
                }
            }

            // Create Uninstaller Script
            string uninstallerCmd = Path.Combine(installDir, "Uninstall.cmd");
            string uninstallScript = $@"@echo off
taskkill /f /im StormFellowship.exe >nul 2>&1
reg delete ""HKCU\Software\Microsoft\Windows\CurrentVersion\Uninstall\StormFellowship"" /f >nul 2>&1
reg delete ""HKCU\Software\Classes\storm"" /f >nul 2>&1
del /f /q ""{desktopShortcut}"" >nul 2>&1
del /f /q ""{startMenuShortcut}"" >nul 2>&1
rmdir /s /q ""{startMenuDir}"" >nul 2>&1
echo STORM FELLOWSHIP uninstalled successfully.
timeout /t 2 >nul
";
            File.WriteAllText(uninstallerCmd, uninstallScript);

            // Notify user of completion
            MessageBox(nint.Zero, "STORM FELLOWSHIP v0.0.1 has been installed successfully!\n\n• Desktop shortcut created\n• Start Menu shortcut created\n• Registry & storm:// protocol registered\n\nClick OK to launch STORM FELLOWSHIP.", "STORM FELLOWSHIP Setup", 0x00000040);

            // Launch app
            if (File.Exists(exePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    WorkingDirectory = installDir,
                    UseShellExecute = true
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox(nint.Zero, $"Installation encountered an error:\n{ex.Message}", "STORM FELLOWSHIP Setup Error", 0x00000010);
        }
    }

    private static void CopyDirectory(string sourceDir, string targetDir)
    {
        foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(sourceDir, targetDir));
        }

        foreach (string file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            string dest = file.Replace(sourceDir, targetDir);
            File.Copy(file, dest, true);
        }
    }

    private static void CreateShortcut(string shortcutPath, string targetPath, string workingDir, string iconPath, string description)
    {
        try
        {
            Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
            if (shellType != null)
            {
                dynamic? shell = Activator.CreateInstance(shellType);
                if (shell != null)
                {
                    dynamic shortcut = shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = targetPath;
                    shortcut.WorkingDirectory = workingDir;
                    if (File.Exists(iconPath))
                    {
                        shortcut.IconLocation = $"{iconPath},0";
                    }
                    shortcut.Description = description;
                    shortcut.Save();
                }
            }
        }
        catch
        {
            // Fallback safe
        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int MessageBox(nint hWnd, string text, string caption, uint type);
}
