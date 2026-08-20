using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class CallViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public CallSession? ActiveCall => CallService.Instance.ActiveCall;
    public bool IsInCall => CallService.Instance.IsInCall;

    public string CallTitle => ActiveCall?.Title ?? "ПРЯМОЙ ВЫЗОВ";
    public string RemoteDisplayName => ActiveCall?.RemoteUser.DisplayName ?? "Собеседник";
    public string RemoteCustomStatus => ActiveCall?.RemoteUser.CustomStatus ?? "В разговоре";
    public string RemoteAvatarGlyph => ActiveCall?.RemoteUser.AvatarGlyph ?? "👤";
    public string LocalDisplayName => FellowshipService.Instance.CurrentUser.DisplayName;
    public string LocalAvatarGlyph => FellowshipService.Instance.CurrentUser.AvatarGlyph;

    public bool IsRemoteSpeaking => ActiveCall?.IsRemoteSpeaking ?? true;
    public bool IsLocalSpeaking => ActiveCall?.IsLocalSpeaking ?? false;
    public bool IsMicMuted => AudioService.Instance.IsMuted;
    public bool IsDeafened => AudioService.Instance.IsDeafened;
    public bool IsVideoOn => ActiveCall?.IsVideoOn ?? false;
    public bool IsScreenSharing => ActiveCall?.IsScreenSharing ?? false;

    public string DurationFormatted => ActiveCall?.DurationFormatted ?? "00:00";
    public string BottomCallStatus => $"Вызов активен • {DurationFormatted} • {ActiveCall?.Codec ?? "Opus 128 Кбит/с"}";
    public string ConnectionStats => $"Пинг: {ActiveCall?.PingMs ?? 16} мс | Потери: {ActiveCall?.PacketLossPercent:0.0}%";

    [ObservableProperty]
    private double _callVolume = 100.0;

    [ObservableProperty]
    private double _bar1Height = 12.0;

    [ObservableProperty]
    private double _bar2Height = 22.0;

    [ObservableProperty]
    private double _bar3Height = 28.0;

    [ObservableProperty]
    private double _bar4Height = 18.0;

    [ObservableProperty]
    private double _bar5Height = 26.0;

    [ObservableProperty]
    private double _bar6Height = 20.0;

    [ObservableProperty]
    private double _bar7Height = 10.0;

    public CallViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        CallService.Instance.CallStateChanged += (call) =>
        {
            RefreshProperties();
        };

        CallService.Instance.WaveformUpdated += (bars) =>
        {
            if (bars.Length >= 7)
            {
                Bar1Height = bars[0];
                Bar2Height = bars[1];
                Bar3Height = bars[2];
                Bar4Height = bars[3];
                Bar5Height = bars[4];
                Bar6Height = bars[5];
                Bar7Height = bars[6];
            }
        };
    }

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(ActiveCall));
        OnPropertyChanged(nameof(IsInCall));
        OnPropertyChanged(nameof(CallTitle));
        OnPropertyChanged(nameof(RemoteDisplayName));
        OnPropertyChanged(nameof(RemoteCustomStatus));
        OnPropertyChanged(nameof(RemoteAvatarGlyph));
        OnPropertyChanged(nameof(LocalDisplayName));
        OnPropertyChanged(nameof(LocalAvatarGlyph));
        OnPropertyChanged(nameof(IsRemoteSpeaking));
        OnPropertyChanged(nameof(IsLocalSpeaking));
        OnPropertyChanged(nameof(IsMicMuted));
        OnPropertyChanged(nameof(IsDeafened));
        OnPropertyChanged(nameof(IsVideoOn));
        OnPropertyChanged(nameof(IsScreenSharing));
        OnPropertyChanged(nameof(DurationFormatted));
        OnPropertyChanged(nameof(BottomCallStatus));
        OnPropertyChanged(nameof(ConnectionStats));
    }

    [RelayCommand]
    public void ToggleMute()
    {
        CallService.Instance.ToggleMute();
        RefreshProperties();
    }

    [RelayCommand]
    public void ToggleDeafen()
    {
        CallService.Instance.ToggleDeafen();
        RefreshProperties();
    }

    [RelayCommand]
    public void ToggleVideo()
    {
        CallService.Instance.ToggleVideo();
        RefreshProperties();
        _mainVM.ShowToastNotification(IsVideoOn ? "Камера включена" : "Камера выключена");
    }

    [RelayCommand]
    public void ToggleScreenShare()
    {
        CallService.Instance.ToggleScreenShare();
        RefreshProperties();
        _mainVM.ShowToastNotification(IsScreenSharing ? "Трансляция экрана запущена" : "Трансляция экрана остановлена");
    }

    [RelayCommand]
    public void EndCall()
    {
        CallService.Instance.EndCall();
        RefreshProperties();
        _mainVM.ShowToastNotification("Вызов завершен");
    }

    partial void OnCallVolumeChanged(double value)
    {
        CallService.Instance.CallVolume = value;
    }
}
