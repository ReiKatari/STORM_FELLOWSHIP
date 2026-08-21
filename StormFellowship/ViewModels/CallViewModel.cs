using System.Collections.ObjectModel;
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
    private readonly List<int> _pingHistory = new() { 16, 15, 18, 14, 15, 17, 14, 16, 15, 14 };

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

    public string ConnectionStats => $"Пинг: {ActiveCall?.PingMs ?? 14} мс • Джиттер: 1.2 мс • 🔒 E2EE AES-256";

    public string BottomCallStatus => $"Вызов активен • {DurationFormatted} • Opus 128 Кбит/с • 32-Band FFT • 3D Spatial Audio";

    public string DurationFormatted => ActiveCall != null
        ? ActiveCall.DurationFormatted
        : "00:00";

    // 32-band FFT Spectrum collection
    public ObservableCollection<double> SpectrumBands { get; } = new();

    // Network quality sparkline polyline points string (e.g. "0,15 10,12 20,18 ...")
    [ObservableProperty]
    private string _sparklinePoints = "0,15 10,12 20,16 30,14 40,15 50,13 60,16 70,14 80,15 90,14";

    // Dynamic Voice Pulse Radii
    [ObservableProperty] private double _remoteGlowRadius = 18;
    [ObservableProperty] private double _localGlowRadius = 12;

    [ObservableProperty] private bool _isRemoteSpeaking = true;
    [ObservableProperty] private bool _isLocalSpeaking = false;

    public CallViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        for (int i = 0; i < 32; i++)
        {
            SpectrumBands.Add(6.0);
        }

        SpectrumAnalyzerService.Instance.SpectrumUpdated += (bands) =>
        {
            App.Current?.Dispatcher.Invoke(() =>
            {
                if (AudioService.Instance.IsLiteMode) return;
                for (int i = 0; i < Math.Min(bands.Length, SpectrumBands.Count); i++)
                {
                    SpectrumBands[i] = bands[i];
                }
            });
        };

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
            LocalGlowRadius = speaking ? 26.0 : 8.0;
        };
    }

    private void OnAnimTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        if (ActiveCall == null) return;

        IsRemoteSpeaking = _random.NextDouble() > 0.3;
        RemoteGlowRadius = IsRemoteSpeaking ? _random.Next(16, 32) : 6.0;

        // Update Sparkline
        int newPing = Math.Clamp((ActiveCall.PingMs) + _random.Next(-2, 3), 10, 28);
        _pingHistory.Add(newPing);
        if (_pingHistory.Count > 10) _pingHistory.RemoveAt(0);

        var points = new List<string>();
        for (int i = 0; i < _pingHistory.Count; i++)
        {
            double x = i * 10;
            double y = Math.Clamp(28 - _pingHistory[i], 2, 26);
            points.Add($"{x},{y}");
        }
        SparklinePoints = string.Join(" ", points);

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
