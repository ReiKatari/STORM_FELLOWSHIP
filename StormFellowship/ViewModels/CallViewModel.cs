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

    public string CallTitle => ActiveCall?.Title ?? "1-1 DIRECT CALL";
    public string RemoteDisplayName => ActiveCall?.RemoteUser.DisplayName ?? "Sakura";
    public string RemoteCustomStatus => ActiveCall?.RemoteUser.CustomStatus ?? "What the bobba";
    public string RemoteAvatarPath => ActiveCall?.RemoteUser.AvatarPath ?? "ms-appx:///Assets/Avatars/sakura.png";
    public string LocalDisplayName => ActiveCall?.LocalUser.DisplayName ?? "You";
    public string LocalAvatarPath => ActiveCall?.LocalUser.AvatarPath ?? "ms-appx:///Assets/Avatars/you.png";
    public bool IsRemoteSpeaking => ActiveCall?.IsRemoteSpeaking ?? true;
    public bool IsLocalSpeaking => ActiveCall?.IsLocalSpeaking ?? false;
    public bool IsMicMuted => ActiveCall?.IsMicMuted ?? false;
    public bool IsDeafened => ActiveCall?.IsDeafened ?? false;
    public string DurationFormatted => ActiveCall?.DurationFormatted ?? "13:37";
    public string BottomCallStatus => ActiveCall?.BottomCallStatus ?? "Call ongoing • 13:37";

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
        OnPropertyChanged(nameof(RemoteAvatarPath));
        OnPropertyChanged(nameof(LocalDisplayName));
        OnPropertyChanged(nameof(LocalAvatarPath));
        OnPropertyChanged(nameof(IsRemoteSpeaking));
        OnPropertyChanged(nameof(IsLocalSpeaking));
        OnPropertyChanged(nameof(IsMicMuted));
        OnPropertyChanged(nameof(IsDeafened));
        OnPropertyChanged(nameof(DurationFormatted));
        OnPropertyChanged(nameof(BottomCallStatus));
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
    }

    [RelayCommand]
    public void ToggleScreenShare()
    {
        CallService.Instance.ToggleScreenShare();
        RefreshProperties();
    }

    [RelayCommand]
    public void EndCall()
    {
        CallService.Instance.EndCall();
        RefreshProperties();
    }
}
