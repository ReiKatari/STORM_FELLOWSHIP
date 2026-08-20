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
            OnPropertyChanged(nameof(ActiveCall));
            OnPropertyChanged(nameof(IsInCall));
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

    [RelayCommand]
    public void ToggleMute()
    {
        CallService.Instance.ToggleMute();
        OnPropertyChanged(nameof(ActiveCall));
    }

    [RelayCommand]
    public void ToggleDeafen()
    {
        CallService.Instance.ToggleDeafen();
        OnPropertyChanged(nameof(ActiveCall));
    }

    [RelayCommand]
    public void ToggleVideo()
    {
        CallService.Instance.ToggleVideo();
        OnPropertyChanged(nameof(ActiveCall));
    }

    [RelayCommand]
    public void ToggleScreenShare()
    {
        CallService.Instance.ToggleScreenShare();
        OnPropertyChanged(nameof(ActiveCall));
    }

    [RelayCommand]
    public void EndCall()
    {
        CallService.Instance.EndCall();
    }
}
