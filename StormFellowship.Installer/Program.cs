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

            // Install bundled prerequisites if present
            InstallPrerequisites(sourceAssembling, appBase);

            // Copy files from Assembling to Install Directory
            if (Directory.Exists(sourceAssembling))
            {
                CopyDirectory(sourceAssembling, installDir);
            }

            string exePath = Path.Combine(installDir, "StormFellowship.exe");
            string iconPath = Path.Combine(installDir, "Assets", "AppIcon.ico");

            // Create Desktop Shortcut
            string desktopShortcut = Path.Combine(desktopDir, "STORM FELLOWSHIP.lnk");
            CreateShortcut(desktopShortcut, exePath, installDir, iconPath, "STORM FELLOWSHIP — Платформа для общения и голосовых созвонов");

            // Create Start Menu Shortcut
            string startMenuShortcut = Path.Combine(startMenuDir, "STORM FELLOWSHIP.lnk");
            CreateShortcut(startMenuShortcut, exePath, installDir, iconPath, "STORM FELLOWSHIP");

            // Write Registry Entries for Add/Remove Programs
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\StormFellowship"))
            {
                key.SetValue("DisplayName", "STORM FELLOWSHIP");
                key.SetValue("DisplayVersion", "0.0.3");
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
echo STORM FELLOWSHIP успешно удален.
timeout /t 2 >nul
";
            File.WriteAllText(uninstallerCmd, uninstallScript);

            // Notify user of completion
            MessageBox(nint.Zero, "STORM FELLOWSHIP v0.0.3 успешно установлена!\n\n• Все необходимые компоненты встроены и настроены\n• Создан ярлык на Рабочем столе\n• Программа добавлена в меню «Пуск»\n• Зарегистрирован протокол storm://\n\nНажмите OK для запуска STORM FELLOWSHIP.", "Установка STORM FELLOWSHIP", 0x00000040);

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
            MessageBox(nint.Zero, $"Ошибка при установке:\n{ex.Message}", "Ошибка установки STORM FELLOWSHIP", 0x00000010);
        }
    }

    private static void InstallPrerequisites(string sourceAssembling, string appBase)
    {
        try
        {
            string[] redistSearchPaths = new[]
            {
                Path.Combine(sourceAssembling, "Redist"),
                Path.Combine(appBase, "Redist"),
                Path.Combine(@"E:\STORM FELLOWSHIP\Files\Redist")
            };

            foreach (var rPath in redistSearchPaths)
            {
                if (!Directory.Exists(rPath)) continue;

                // VC++ Redistributable
                string vcInstaller = Path.Combine(rPath, "vc_redist.x64.exe");
                if (File.Exists(vcInstaller))
                {
                    var p = Process.Start(new ProcessStartInfo
                    {
                        FileName = vcInstaller,
                        Arguments = "/install /quiet /norestart",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                    p?.WaitForExit(30000);
                }

                // WebView2 Bootstrapper
                string wvInstaller = Path.Combine(rPath, "MicrosoftEdgeWebview2Setup.exe");
                if (File.Exists(wvInstaller))
                {
                    var p = Process.Start(new ProcessStartInfo
                    {
                        FileName = wvInstaller,
                        Arguments = "/silent /install",
                        UseShellExecute = true,
                        CreateNoWindow = true
                    });
                    p?.WaitForExit(30000);
                }

                break;
            }
        }
        catch
        {
            // Ignore optional prerequisites installation errors if already installed
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
