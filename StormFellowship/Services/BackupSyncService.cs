using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using StormFellowship.Models;

namespace StormFellowship.Services;

public record StormBackupData(
    string Version,
    DateTime CreatedAt,
    string DisplayName,
    string CustomStatus,
    string AvatarGlyph,
    ThemeType Theme,
    string AccentColorHex,
    string PushToTalkKey,
    bool IsPushToTalkEnabled,
    double InputVolume,
    double OutputVolume,
    string SelectedVoiceFxPreset,
    double DuckingPercent,
    bool IsSpatialAudioEnabled
);

/// <summary>
/// Storm Backup & Cloud Sync Service.
/// Exports and imports encrypted backup packages (.stormbackup) containing user profiles, hotkeys, and preferences.
/// </summary>
public class BackupSyncService
{
    private static BackupSyncService? _instance;
    public static BackupSyncService Instance => _instance ??= new BackupSyncService();

    public async Task<string> ExportBackupAsync(string destinationPath)
    {
        var backup = new StormBackupData(
            Version: "0.2.2",
            CreatedAt: DateTime.UtcNow,
            DisplayName: FellowshipService.Instance.CurrentUser.DisplayName,
            CustomStatus: FellowshipService.Instance.CurrentUser.CustomStatus,
            AvatarGlyph: FellowshipService.Instance.CurrentUser.AvatarGlyph,
            Theme: ThemeService.Instance.CurrentTheme,
            AccentColorHex: "#00D2FF",
            PushToTalkKey: GlobalHotkeyService.Instance.ConfiguredPttKey,
            IsPushToTalkEnabled: AudioService.Instance.IsPushToTalkEnabled,
            InputVolume: AudioService.Instance.InputVolume,
            OutputVolume: AudioService.Instance.OutputVolume,
            SelectedVoiceFxPreset: AudioService.Instance.SelectedVoicePreset.Name,
            DuckingPercent: AudioDuckingService.Instance.DuckingPercent,
            IsSpatialAudioEnabled: SpatialAudioService.Instance.IsSpatialAudioEnabled
        );

        string json = JsonSerializer.Serialize(backup, new JsonSerializerOptions { WriteIndented = true });
        
        // Encrypt using AES-256 GCM from EncryptionService
        string encrypted = EncryptionService.Instance.EncryptString(json);
        await File.WriteAllTextAsync(destinationPath, encrypted);

        return destinationPath;
    }

    public async Task<bool> ImportBackupAsync(string sourcePath)
    {
        if (!File.Exists(sourcePath)) return false;

        string encrypted = await File.ReadAllTextAsync(sourcePath);
        string decrypted = EncryptionService.Instance.DecryptString(encrypted);

        var backup = JsonSerializer.Deserialize<StormBackupData>(decrypted);
        if (backup == null) return false;

        // Apply backup settings
        FellowshipService.Instance.CurrentUser.DisplayName = backup.DisplayName;
        FellowshipService.Instance.CurrentUser.CustomStatus = backup.CustomStatus;
        FellowshipService.Instance.CurrentUser.AvatarGlyph = backup.AvatarGlyph;
        ThemeService.Instance.SetTheme(backup.Theme);
        AudioService.Instance.InputVolume = backup.InputVolume;
        AudioService.Instance.OutputVolume = backup.OutputVolume;
        AudioDuckingService.Instance.DuckingPercent = backup.DuckingPercent;
        SpatialAudioService.Instance.IsSpatialAudioEnabled = backup.IsSpatialAudioEnabled;

        return true;
    }
}
