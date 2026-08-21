using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public record StatusPresetItem(string IconGeo, string Title, string ColorHex);
public record AvatarPresetItem(string IconGeo, string Name, string ColorHex);
public record AccentColorItem(string Name, string Hex);

public partial class UserSettingsViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public User CurrentUser => FellowshipService.Instance.CurrentUser;

    public string DisplayName
    {
        get => CurrentUser.DisplayName;
        set
        {
            CurrentUser.DisplayName = value;
            OnPropertyChanged(nameof(DisplayName));
            FellowshipService.Instance.SaveUserProfile();
        }
    }

    public string CustomStatus
    {
        get => CurrentUser.CustomStatus;
        set
        {
            CurrentUser.CustomStatus = value;
            OnPropertyChanged(nameof(CustomStatus));
            FellowshipService.Instance.SaveUserProfile();
        }
    }

    public string AvatarGlyph
    {
        get => CurrentUser.AvatarGlyph;
        set
        {
            CurrentUser.AvatarGlyph = value;
            OnPropertyChanged(nameof(AvatarGlyph));
            FellowshipService.Instance.SaveUserProfile();
        }
    }

    public string AvatarPath
    {
        get => CurrentUser.AvatarPath;
        set
        {
            CurrentUser.AvatarPath = value;
            OnPropertyChanged(nameof(AvatarPath));
            OnPropertyChanged(nameof(HasCustomAvatar));
            FellowshipService.Instance.SaveUserProfile();
        }
    }

    public bool HasCustomAvatar => !string.IsNullOrWhiteSpace(CurrentUser.AvatarPath);

    public bool IsMicMonitoringLoopbackEnabled
    {
        get => AudioService.Instance.IsMicMonitoringLoopbackEnabled;
        set
        {
            AudioService.Instance.IsMicMonitoringLoopbackEnabled = value;
            OnPropertyChanged(nameof(IsMicMonitoringLoopbackEnabled));
            _mainVM.ShowToastNotification(value 
                ? "🎙️ Мониторинг голоса включен: говорите в микрофон!" 
                : "🔇 Мониторинг голоса отключен");
        }
    }

    public bool IsVadMode
    {
        get => !IsPushToTalk;
        set => IsPushToTalk = !value;
    }

    public bool IsPttMode
    {
        get => IsPushToTalk;
        set => IsPushToTalk = value;
    }

    public double LiveMicLevel
    {
        get => MicLiveLevel;
        set => MicLiveLevel = value;
    }

    public string PttKeyName => PttKey;

    [ObservableProperty]
    private ThemeType _selectedTheme = ThemeType.StormDark;

    [ObservableProperty]
    private double _micLiveLevel = 0.0;

    [ObservableProperty]
    private double _vadThreshold = 25.0;

    [ObservableProperty]
    private bool _isPushToTalk = false;

    [ObservableProperty]
    private string _pttKey = "Боковая кнопка 4";

    [ObservableProperty]
    private double _inputVolume = 100.0;

    [ObservableProperty]
    private double _outputVolume = 100.0;

    [ObservableProperty]
    private bool _soundCuesEnabled = true;

    [ObservableProperty]
    private bool _spatialAudioEnabled = true;

    [ObservableProperty]
    private bool _noiseSuppressionEnabled = true;

    [ObservableProperty]
    private bool _isNoiseSuppression = true;

    [ObservableProperty]
    private bool _isEchoCancellation = true;

    [ObservableProperty]
    private bool _is3DPositionalAudio = true;

    [ObservableProperty]
    private bool _isLiteMode = false;

    [ObservableProperty]
    private string _selectedDirectionMode = "Кардиоидная (Фронтальный фокус на голосе)";

    [ObservableProperty]
    private string _selectedNoiseMode = "RNNoise AI (Нейросетевое глубокое подавление)";

    [ObservableProperty]
    private string _selectedAudioInput = string.Empty;

    [ObservableProperty]
    private string _selectedAudioOutput = string.Empty;

    [ObservableProperty]
    private string _profileBannerHex = "#00A3FF";

    [ObservableProperty]
    private string _selectedAccentColor = "#00A3FF";

    public ObservableCollection<AccentColorItem> AccentColorPalette { get; } = new()
    {
        new("Cyber Cyan", "#00E5FF"),
        new("Electric Blue", "#00A3FF"),
        new("Neon Purple", "#A855F7"),
        new("Emerald Green", "#10B981"),
        new("Sunset Amber", "#F59E0B"),
        new("Crimson Rose", "#F43F5E"),
        new("Platinum White", "#E2E8F0")
    };

    public ObservableCollection<string> DirectionModes { get; } = new()
    {
        "Кардиоидная (Фронтальный фокус на голосе)",
        "Суперкардиоидная (Узконаправленная изоляция)",
        "Круговая 360° (Всенаправленная)",
        "Студийный AI фильтр (Глубокое подавление шумов)"
    };

    public ObservableCollection<string> NoiseModes { get; } = new()
    {
        "RNNoise AI (Нейросетевое глубокое подавление)",
        "DeepFilterNet Studio (Студийная нейросеть)",
        "Стандартный спектральный фильтр",
        "Отключено"
    };

    public ObservableCollection<string> FxPresets { get; } = new()
    {
        "Студийный баланс (Нейтральный чистый голос)",
        "Глубокий бас (Radio Broadcast / Deep Voice)",
        "Кристальный голос (Treble Boost / Максимальная четкость)",
        "Киберспортивный (Фокус на тиммейтах, отсечение фона)",
        "Теплый ламповый звук"
    };

    public List<VoiceChangerPreset> VoiceChangerPresets => AudioService.Instance.VoiceChangerPresets;

    public VoiceChangerPreset SelectedVoiceChangerPreset
    {
        get => AudioService.Instance.SelectedVoicePreset;
        set
        {
            AudioService.Instance.SelectedVoicePreset = value;
            OnPropertyChanged(nameof(SelectedVoiceChangerPreset));
            _mainVM.ShowToastNotification($"🎙️ Эффект изменения голоса: {value.Icon} {value.Name}");
        }
    }

    [RelayCommand]
    public void PreviewVoiceChanger(VoiceChangerPreset? preset)
    {
        var target = preset ?? SelectedVoiceChangerPreset;
        if (target != null)
        {
            AudioService.Instance.PreviewVoicePreset(target);
            _mainVM.ShowToastNotification($"🔊 Тест пресета: {target.Icon} {target.Name}");
        }
    }

    [ObservableProperty]
    private string _selectedFxPreset = "Студийный баланс (Нейтральный чистый голос)";

    public ObservableCollection<string> AudioInputDevices { get; } = new();
    public ObservableCollection<string> AudioOutputDevices { get; } = new();

    public ObservableCollection<AvatarPresetItem> AvatarPresets { get; } = new()
    {
        new("GeoLightning", "Молния", "#3B82F6"),
        new("GeoShield", "Щит", "#10B981"),
        new("GeoCrown", "Корона", "#F59E0B"),
        new("GeoGamepad", "Гейминг", "#8B5CF6"),
        new("GeoRocket", "Ракета", "#EC4899"),
        new("GeoDiamond", "Алмаз", "#06B6D4"),
        new("GeoFire", "Огонь", "#EF4444"),
        new("GeoStar", "Звезда", "#FBBF24"),
        new("GeoBot", "Киборг", "#6366F1"),
        new("GeoSwords", "Битва", "#14B8A6"),
        new("GeoHeadphones", "Аудио", "#A855F7"),
        new("GeoTrophy", "Трофей", "#EAB308")
    };

    public ObservableCollection<StatusPresetItem> StatusPresets { get; } = new()
    {
        new("GeoGamepad", "Играет в игру", "#3B82F6"),
        new("GeoHeadphones", "Слушает музыку", "#8B5CF6"),
        new("GeoLightning", "В фокусе / Кодит", "#F59E0B"),
        new("GeoMic", "На созвоне", "#10B981"),
        new("GeoSwords", "В рейде / Турнир", "#EF4444"),
        new("GeoTrophy", "Побеждает", "#EAB308"),
        new("GeoShield", "На страже содружества", "#06B6D4"),
        new("GeoStar", "Создает контент", "#EC4899"),
        new("GeoBot", "Автоответчик", "#6366F1"),
        new("GeoEye", "Смотрит трансляцию", "#14B8A6"),
        new("GeoSearch", "В поисках тимейтов", "#38BDF8"),
        new("GeoSave", "Работает над проектом", "#84CC16")
    };

    public bool IsAvxDspActive => AvxDspService.IsAvx2Supported;
    public string DspInstructionSet => AvxDspService.CpuInstructionSet;

    [ObservableProperty]
    private bool _isAudioDuckingEnabled = true;

    [ObservableProperty]
    private double _duckingPercent = 40.0;

    [ObservableProperty]
    private double _smartKeySuppression = 85.0;

    [ObservableProperty]
    private double _smartBreathSuppression = 90.0;

    [ObservableProperty]
    private bool _isAdaptiveMeshEnabled = true;

    [ObservableProperty]
    private int _selectedMaxBitrate = 384;

    public ObservableCollection<int> BitrateOptions { get; } = new() { 64, 128, 192, 256, 384, 510 };

    public ObservableCollection<SupportedLanguage> AvailableLanguages => TranslationService.Instance.AvailableLanguages;

    public SupportedLanguage SelectedLanguage
    {
        get => TranslationService.Instance.TargetLanguage;
        set
        {
            TranslationService.Instance.TargetLanguage = value;
            OnPropertyChanged(nameof(SelectedLanguage));
            _mainVM.ShowToastNotification($"🌍 Язык перевода: {value.FlagEmoji} {value.Name}");
        }
    }

    [ObservableProperty]
    private bool _isAutoTranslateChat = false;

    [RelayCommand]
    public async Task ExportBackup()
    {
        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Title = "Экспорт резервной копии STORM FELLOWSHIP",
                Filter = "STORM Backup (*.stormbackup)|*.stormbackup",
                FileName = $"StormBackup_{DateTime.Now:yyyyMMdd_HHmm}.stormbackup"
            };

            if (dialog.ShowDialog() == true)
            {
                await BackupSyncService.Instance.ExportBackupAsync(dialog.FileName);
                _mainVM.ShowToastNotification("💾 Резервная копия .stormbackup успешно создана!");
            }
        }
        catch (Exception ex)
        {
            _mainVM.ShowToastNotification($"Ошибка экспорта: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task ImportBackup()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Восстановление из резервной копии STORM FELLOWSHIP",
                Filter = "STORM Backup (*.stormbackup)|*.stormbackup"
            };

            if (dialog.ShowDialog() == true)
            {
                bool ok = await BackupSyncService.Instance.ImportBackupAsync(dialog.FileName);
                if (ok)
                {
                    _mainVM.ShowToastNotification("✅ Настройки и профиль успешно восстановлены!");
                }
            }
        }
        catch (Exception ex)
        {
            _mainVM.ShowToastNotification($"Ошибка восстановления: {ex.Message}");
        }
    }

    public UserSettingsViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;
        SelectedTheme = ThemeService.Instance.CurrentTheme;
        VadThreshold = AudioService.Instance.VadSensitivityThreshold;
        IsPushToTalk = AudioService.Instance.IsPushToTalkEnabled;
        PttKey = AudioService.Instance.PushToTalkKey;
        InputVolume = AudioService.Instance.InputVolume;
        OutputVolume = AudioService.Instance.OutputVolume;
        IsNoiseSuppression = AudioService.Instance.IsNoiseSuppressionEnabled;
        IsEchoCancellation = AudioService.Instance.IsEchoCancellationEnabled;
        Is3DPositionalAudio = AudioService.Instance.Is3DPositionalAudioEnabled;
        IsLiteMode = AudioService.Instance.IsLiteMode;

        // Load devices
        foreach (var dev in AudioService.GetAvailableInputDevices()) AudioInputDevices.Add(dev);
        foreach (var dev in AudioService.GetAvailableOutputDevices()) AudioOutputDevices.Add(dev);
        SelectedAudioInput = AudioInputDevices.FirstOrDefault() ?? "Микрофон по умолчанию";
        SelectedAudioOutput = AudioOutputDevices.FirstOrDefault() ?? "Динамики по умолчанию";

        AudioService.Instance.MicLevelChanged += (level) =>
        {
            MicLiveLevel = level;
            OnPropertyChanged(nameof(LiveMicLevel));
        };
    }

    [RelayCommand]
    public void SelectAccentColor(AccentColorItem item)
    {
        if (item != null)
        {
            SelectedAccentColor = item.Hex;
            ProfileBannerHex = item.Hex;
            ThemeService.Instance.SetAccentColor(item.Hex);
            _mainVM.ShowToastNotification($"🎨 Акцентный цвет изменен: {item.Name} ({item.Hex})");
        }
    }

    [RelayCommand]
    public void SelectAvatarPreset(AvatarPresetItem preset)
    {
        if (preset == null) return;
        AvatarGlyph = preset.IconGeo;
        AvatarPath = string.Empty;
        CurrentUser.AvatarGlyph = preset.IconGeo;
        CurrentUser.AvatarPath = string.Empty;
        OnPropertyChanged(nameof(AvatarGlyph));
        OnPropertyChanged(nameof(AvatarPath));
        OnPropertyChanged(nameof(HasCustomAvatar));
        FellowshipService.Instance.SaveUserProfile();
        _mainVM.RefreshUserProfileBindings();
        _mainVM.ShowToastNotification($"Аватар изменен на векторный символ «{preset.Name}»");
    }

    [RelayCommand]
    public void UploadCustomAvatar()
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите изображение аватара",
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.ico;*.bmp)|*.png;*.jpg;*.jpeg;*.ico;*.bmp|Все файлы (*.*)|*.*",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                AvatarPath = dialog.FileName;
                CurrentUser.AvatarPath = dialog.FileName;
                OnPropertyChanged(nameof(AvatarPath));
                OnPropertyChanged(nameof(HasCustomAvatar));
                FellowshipService.Instance.SaveUserProfile();
                _mainVM.RefreshUserProfileBindings();
                _mainVM.ShowToastNotification("Пользовательский аватар успешно сохранен и применен!");
            }
        }
        catch (Exception ex)
        {
            _mainVM.ShowToastNotification($"Ошибка при загрузке аватара: {ex.Message}");
        }
    }

    [RelayCommand]
    public void ResetCustomAvatar()
    {
        AvatarPath = string.Empty;
        CurrentUser.AvatarPath = string.Empty;
        OnPropertyChanged(nameof(AvatarPath));
        OnPropertyChanged(nameof(HasCustomAvatar));
        FellowshipService.Instance.SaveUserProfile();
        _mainVM.RefreshUserProfileBindings();
        _mainVM.ShowToastNotification("Аватар сброшен на иконку по умолчанию");
    }

    [RelayCommand]
    public void SelectStatusPreset(StatusPresetItem preset)
    {
        if (preset == null) return;
        CustomStatus = preset.Title;
        CurrentUser.CustomStatus = CustomStatus;
        OnPropertyChanged(nameof(CustomStatus));
        FellowshipService.Instance.SaveUserProfile();
        _mainVM.RefreshUserProfileBindings();
        _mainVM.ShowToastNotification($"Статус обновлен: {preset.Title}");
    }

    [RelayCommand]
    public void TestAudio()
    {
        AudioService.Instance.PlayTestChime();
        _mainVM.ShowToastNotification("Воспроизведение тестового 4-тонового сигнала...");
    }

    [RelayCommand]
    public void ApplySettings()
    {
        ThemeService.Instance.ApplyTheme(SelectedTheme);
        AudioService.Instance.VadSensitivityThreshold = VadThreshold;
        AudioService.Instance.IsPushToTalkEnabled = IsPushToTalk;
        AudioService.Instance.PushToTalkKey = PttKey;
        AudioService.Instance.InputVolume = InputVolume;
        AudioService.Instance.OutputVolume = OutputVolume;
        AudioService.Instance.IsNoiseSuppressionEnabled = IsNoiseSuppression;
        AudioService.Instance.IsEchoCancellationEnabled = IsEchoCancellation;
        AudioService.Instance.Is3DPositionalAudioEnabled = Is3DPositionalAudio;
        AudioService.Instance.IsLiteMode = IsLiteMode;

        if (SelectedDirectionMode.Contains("Кардиоидная")) AudioService.Instance.DirectionMode = AudioDirectionMode.Cardioid;
        else if (SelectedDirectionMode.Contains("Суперкардиоидная")) AudioService.Instance.DirectionMode = AudioDirectionMode.Hypercardioid;
        else if (SelectedDirectionMode.Contains("Круговая")) AudioService.Instance.DirectionMode = AudioDirectionMode.Omnidirectional;
        else if (SelectedDirectionMode.Contains("Студийный")) AudioService.Instance.DirectionMode = AudioDirectionMode.StudioAI;

        if (SelectedNoiseMode.Contains("RNNoise")) AudioService.Instance.NoiseSuppressionMode = NoiseSuppressionEngineMode.RNNoiseAI;
        else if (SelectedNoiseMode.Contains("DeepFilterNet")) AudioService.Instance.NoiseSuppressionMode = NoiseSuppressionEngineMode.DeepFilterNet;
        else if (SelectedNoiseMode.Contains("Стандартный")) AudioService.Instance.NoiseSuppressionMode = NoiseSuppressionEngineMode.Standard;
        else AudioService.Instance.NoiseSuppressionMode = NoiseSuppressionEngineMode.Off;

        _mainVM.CloseUserSettingsDialog();
        _mainVM.ShowToastNotification("Настройки STORM FELLOWSHIP успешно сохранены");
    }

    [RelayCommand]
    public void CloseModal()
    {
        _mainVM.CloseUserSettingsDialog();
    }

    public void Close()
    {
        CloseModal();
    }
}
