using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

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
    private string _selectedAudioInput = "Микрофон по умолчанию";

    [ObservableProperty]
    private string _selectedAudioOutput = "Динамики по умолчанию";

    public ObservableCollection<string> AudioInputDevices { get; } = new()
    {
        "Микрофон по умолчанию",
        "Микрофон Realtek High Definition",
        "Игровая гарнитура",
        "Линейный вход"
    };

    public ObservableCollection<string> AudioOutputDevices { get; } = new()
    {
        "Динамики по умолчанию",
        "Наушники",
        "Динамики Realtek High Definition",
        "Цифровой оптический выход"
    };

    public UserSettingsViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;
        SelectedTheme = ThemeService.Instance.CurrentTheme;
        VadThreshold = AudioService.Instance.VadSensitivityThreshold;
        IsPushToTalk = AudioService.Instance.IsPushToTalkEnabled;
        PttKey = AudioService.Instance.PushToTalkKey;
        if (PttKey == "Mouse4") PttKey = "Боковая кнопка 4";
        IsNoiseSuppression = AudioService.Instance.IsNoiseSuppressionEnabled;
        IsEchoCancellation = AudioService.Instance.IsEchoCancellationEnabled;
        Is3DPositionalAudio = AudioService.Instance.Is3DPositionalAudioEnabled;

        AudioService.Instance.MicLevelChanged += (level) =>
        {
            MicLiveLevel = level;
            OnPropertyChanged(nameof(LiveMicLevel));
        };
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
