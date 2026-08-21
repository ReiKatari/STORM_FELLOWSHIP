using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class CallViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly System.Timers.Timer _animTimer;
    private readonly Random _random = new();

    public CallSession? ActiveCall => CallService.Instance.ActiveCall;

    public bool IsInCall => ActiveCall != null && ActiveCall.State != CallState.Ended;

    public string CallTitle => ActiveCall != null
        ? ActiveCall.Title
        : "Вызов не активен";

    public string RemoteDisplayName => ActiveCall?.RemoteUser?.DisplayName ?? "Собеседник";
    public string RemoteCustomStatus => ActiveCall?.RemoteUser?.CustomStatus ?? "В разговоре";
    public string RemoteAvatarGlyph => ActiveCall?.RemoteUser?.AvatarGlyph ?? "👤";

    public string LocalDisplayName => FellowshipService.Instance.CurrentUser.DisplayName;
    public string LocalAvatarGlyph => FellowshipService.Instance.CurrentUser.AvatarGlyph;

    public bool IsMicMuted => AudioService.Instance.IsMuted;
    public bool IsDeafened => AudioService.Instance.IsDeafened;
    public bool IsVideoOn => ActiveCall?.IsVideoOn ?? false;
    public bool IsScreenSharing => ActiveCall?.IsScreenSharing ?? false;

    public double CallVolume
    {
        get => CallService.Instance.CallVolume;
        set
        {
            CallService.Instance.CallVolume = value;
            OnPropertyChanged(nameof(CallVolume));
        }
    }

    public string ConnectionStats => $"Пинг: {ActiveCall?.PingMs ?? 14} мс • Потери: 0.0% • 🔒 E2EE Шифрование (AES-256)";

    public string BottomCallStatus => $"Вызов активен • {DurationFormatted} • Opus 128 Кбит/с • Сверхнизкая задержка • 3D Spatial";

    public string DurationFormatted => ActiveCall != null
        ? ActiveCall.DurationFormatted
        : "00:00";

    // Dynamic Waveform heights
    [ObservableProperty] private double _bar1Height = 8;
    [ObservableProperty] private double _bar2Height = 14;
    [ObservableProperty] private double _bar3Height = 22;
    [ObservableProperty] private double _bar4Height = 32;
    [ObservableProperty] private double _bar5Height = 24;
    [ObservableProperty] private double _bar6Height = 16;
    [ObservableProperty] private double _bar7Height = 10;

    [ObservableProperty] private bool _isRemoteSpeaking = true;
    [ObservableProperty] private bool _isLocalSpeaking = false;

    public CallViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        _animTimer = new System.Timers.Timer(100);
        _animTimer.Elapsed += OnAnimTimerElapsed;
        _animTimer.AutoReset = true;

        CallService.Instance.CallStateChanged += (call) =>
        {
            if (call != null)
            {
                RefreshAll();
                _animTimer.Start();
            }
            else
            {
                _animTimer.Stop();
                RefreshAll();
            }
        };

        AudioService.Instance.SpeakingStateChanged += (speaking) =>
        {
            IsLocalSpeaking = speaking;
        };
    }

    private void OnAnimTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (ActiveCall == null) return;

        if (AudioService.Instance.IsLiteMode)
        {
            Bar1Height = 12; Bar2Height = 18; Bar3Height = 24;
            Bar4Height = 30; Bar5Height = 24; Bar6Height = 18; Bar7Height = 12;
            OnPropertyChanged(nameof(DurationFormatted));
            return;
        }

        // Animated dynamic equalizer bars
        Bar1Height = _random.Next(6, 26);
        Bar2Height = _random.Next(10, 34);
        Bar3Height = _random.Next(14, 40);
        Bar4Height = _random.Next(18, 48);
        Bar5Height = _random.Next(14, 40);
        Bar6Height = _random.Next(10, 34);
        Bar7Height = _random.Next(6, 26);

        IsRemoteSpeaking = _random.NextDouble() > 0.3;

        OnPropertyChanged(nameof(DurationFormatted));
        OnPropertyChanged(nameof(ConnectionStats));
        OnPropertyChanged(nameof(BottomCallStatus));
    }

    public void RefreshAll()
    {
        OnPropertyChanged(nameof(ActiveCall));
        OnPropertyChanged(nameof(IsInCall));
        OnPropertyChanged(nameof(CallTitle));
        OnPropertyChanged(nameof(RemoteDisplayName));
        OnPropertyChanged(nameof(RemoteCustomStatus));
        OnPropertyChanged(nameof(RemoteAvatarGlyph));
        OnPropertyChanged(nameof(LocalDisplayName));
        OnPropertyChanged(nameof(LocalAvatarGlyph));
        OnPropertyChanged(nameof(IsMicMuted));
        OnPropertyChanged(nameof(IsDeafened));
        OnPropertyChanged(nameof(IsVideoOn));
        OnPropertyChanged(nameof(IsScreenSharing));
        OnPropertyChanged(nameof(CallVolume));
        OnPropertyChanged(nameof(ConnectionStats));
        OnPropertyChanged(nameof(DurationFormatted));
        OnPropertyChanged(nameof(BottomCallStatus));
    }

    [RelayCommand]
    public void ToggleMute()
    {
        CallService.Instance.ToggleMute();
        RefreshAll();
    }

    [RelayCommand]
    public void ToggleDeafen()
    {
        CallService.Instance.ToggleDeafen();
        RefreshAll();
    }

    [RelayCommand]
    public void ToggleVideo()
    {
        CallService.Instance.ToggleVideo();
        RefreshAll();
    }

    [RelayCommand]
    public void ToggleScreenShare()
    {
        _mainVM.OpenScreenShareDialog();
    }

    [RelayCommand]
    public void OpenE2EEDialog()
    {
        _mainVM.OpenE2EESecurityDialog();
    }

    [RelayCommand]
    public void EndCall()
    {
        CallService.Instance.EndCall();
        RefreshAll();
    }
}
