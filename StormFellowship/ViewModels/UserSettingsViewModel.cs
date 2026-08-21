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
                _mainVM.ShowToastNotification("Пользовательский аватар успешно загружен!");
            }
        }
        catch (Exception ex)
        {
            _mainVM.ShowToastNotification($"Ошибка при загрузке аватара: {ex.Message}");
        }
    }

    [RelayCommand]
    public void SelectStatusPreset(StatusPresetItem preset)
    {
        CustomStatus = $"{preset.Icon} {preset.Title}";
        CurrentUser.CustomStatus = CustomStatus;
        OnPropertyChanged(nameof(CustomStatus));
        _mainVM.ShowToastNotification($"Статус обновлен: {preset.Icon} {preset.Title}");
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
