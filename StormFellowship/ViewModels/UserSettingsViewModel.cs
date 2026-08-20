using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public record StatusPresetItem(string Icon, string Title);

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
        }
    }

    public string CustomStatus
    {
        get => CurrentUser.CustomStatus;
        set
        {
            CurrentUser.CustomStatus = value;
            OnPropertyChanged(nameof(CustomStatus));
        }
    }

    public string AvatarGlyph
    {
        get => CurrentUser.AvatarGlyph;
        set
        {
            CurrentUser.AvatarGlyph = value;
            OnPropertyChanged(nameof(AvatarGlyph));
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
        }
    }

    public bool HasCustomAvatar => !string.IsNullOrWhiteSpace(CurrentUser.AvatarPath);

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
    private double _vadThreshold = 35.0;

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
    private string _selectedAudioInput = string.Empty;

    [ObservableProperty]
    private string _selectedAudioOutput = string.Empty;

    public ObservableCollection<string> AudioInputDevices { get; } = new();
    public ObservableCollection<string> AudioOutputDevices { get; } = new();

    public ObservableCollection<string> AvatarPresets { get; } = new()
    {
        "⚡", "🛡️", "👑", "🐺", "🦅", "🐉", "⚔️", "🌌",
        "🚀", "💎", "🐱", "🦊", "🐯", "🐼", "🦁", "🤖",
        "🎮", "🎧", "🏆", "🔥", "🔮", "🎯", "🌟", "✨"
    };

    public ObservableCollection<StatusPresetItem> StatusPresets { get; } = new()
    {
        new("🎮", "Играет в игру"),
        new("🎧", "Слушает музыку"),
        new("💻", "Программирует"),
        new("🚀", "На созвоне"),
        new("⚔️", "В рейде"),
        new("🏆", "На турнире"),
        new("⚡", "Заряжен энергией"),
        new("🛡️", "На страже содружества"),
        new("☕", "Пьет кофе"),
        new("🍕", "На перекусе"),
        new("💤", "Спит / Отдыхает"),
        new("🏖️", "В отпуске"),
        new("✈️", "В путешествии"),
        new("🎬", "Смотрит фильм"),
        new("🎯", "В прицеле"),
        new("🔥", "В огне / Продуктивность"),
        new("🌊", "На своей волне"),
        new("🕹️", "Ведет стрим"),
        new("🏋️", "В спортзале"),
        new("🎤", "Записывает подкаст"),
        new("🤖", "Автоответчик"),
        new("📚", "Учится / Читает"),
        new("🧠", "В размышлениях"),
        new("🎨", "Создает дизайн")
    };

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
    public void SelectAvatarPreset(string glyph)
    {
        AvatarGlyph = glyph;
        AvatarPath = string.Empty;
        CurrentUser.AvatarGlyph = glyph;
        CurrentUser.AvatarPath = string.Empty;
        OnPropertyChanged(nameof(AvatarGlyph));
        OnPropertyChanged(nameof(AvatarPath));
        OnPropertyChanged(nameof(HasCustomAvatar));
        _mainVM.ShowToastNotification($"Аватар изменен на {glyph}");
    }

    [RelayCommand]
    public void UploadCustomAvatar()
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Title = "Выберите изображение для аватара",
                Filter = "Изображения (*.png;*.jpg;*.jpeg;*.ico;*.bmp)|*.png;*.jpg;*.jpeg;*.ico;*.bmp|Все файлы (*.*)|*.*",
                CheckFileExists = true
            };

            if (dialog.ShowDialog() == true)
            {
                AvatarPath = dialog.FileName;
                CurrentUser.AvatarPath = dialog.FileName;
                OnPropertyChanged(nameof(AvatarPath));
                OnPropertyChanged(nameof(HasCustomAvatar));
                _mainVM.ShowToastNotification("Пользовательский аватар успешно загружен!");
            }
        }
        catch (Exception ex)
        {
            _mainVM.ShowToastNotification($"Ошибка загрузки аватара: {ex.Message}");
        }
    }

    [RelayCommand]
    public void SelectStatusPreset(StatusPresetItem item)
    {
        if (item != null)
        {
            string fullStatus = $"{item.Icon} {item.Title}";
            CustomStatus = fullStatus;
            CurrentUser.CustomStatus = fullStatus;
            OnPropertyChanged(nameof(CustomStatus));
            _mainVM.ShowToastNotification($"Установлен статус: {fullStatus}");
        }
    }

    [RelayCommand]
    public void TestAudio()
    {
        AudioService.Instance.PlayTestChime();
        _mainVM.ShowToastNotification("Воспроизведение тестового звукового сигнала...");
    }

    [RelayCommand]
    public void SelectTheme(string themeName)
    {
        if (Enum.TryParse<ThemeType>(themeName, out var theme))
        {
            SelectedTheme = theme;
            ThemeService.Instance.SetTheme(theme);
            _mainVM.ShowToastNotification("Тема оформления изменена");
        }
    }

    [RelayCommand]
    public void SetUserStatus(string statusString)
    {
        if (Enum.TryParse<UserStatus>(statusString, out var status))
        {
            CurrentUser.Status = status;
            _mainVM.ShowToastNotification($"Статус изменен: {CurrentUser.StatusText}");
        }
    }

    [RelayCommand]
    public void Close()
    {
        _mainVM.CloseUserSettingsDialog();
    }

    [RelayCommand]
    public void Save()
    {
        _mainVM.ShowToastNotification("Настройки пользователя успешно сохранены!");
        _mainVM.CloseUserSettingsDialog();
    }

    partial void OnInputVolumeChanged(double value)
    {
        AudioService.Instance.InputVolume = value;
    }

    partial void OnOutputVolumeChanged(double value)
    {
        AudioService.Instance.OutputVolume = value;
    }

    partial void OnVadThresholdChanged(double value)
    {
        AudioService.Instance.VadSensitivityThreshold = value;
    }

    partial void OnIsPushToTalkChanged(bool value)
    {
        AudioService.Instance.IsPushToTalkEnabled = value;
        OnPropertyChanged(nameof(IsVadMode));
        OnPropertyChanged(nameof(IsPttMode));
    }

    partial void OnIsNoiseSuppressionChanged(bool value)
    {
        AudioService.Instance.IsNoiseSuppressionEnabled = value;
    }

    partial void OnIsEchoCancellationChanged(bool value)
    {
        AudioService.Instance.IsEchoCancellationEnabled = value;
    }

    partial void OnIs3DPositionalAudioChanged(bool value)
    {
        AudioService.Instance.Is3DPositionalAudioEnabled = value;
    }
}
