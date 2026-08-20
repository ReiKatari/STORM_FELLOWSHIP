using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class ChannelSidebarViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public Fellowship? CurrentFellowship => FellowshipService.Instance.CurrentFellowship;
    public User? CurrentDmUser => FellowshipService.Instance.CurrentDmUser;
    public bool IsDirectMessagesSelected => FellowshipService.Instance.IsDirectMessagesSelected;
    public ObservableCollection<User> DirectMessageUsers => FellowshipService.Instance.DirectMessageUsers;
    public User CurrentUser => FellowshipService.Instance.CurrentUser;

    [ObservableProperty]
    private bool _isMuted = false;

    [ObservableProperty]
    private bool _isDeafened = false;

    [ObservableProperty]
    private bool _isSpeaking = false;

    public ChannelSidebarViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        FellowshipService.Instance.CurrentFellowshipChanged += (f) =>
        {
            OnPropertyChanged(nameof(CurrentFellowship));
            OnPropertyChanged(nameof(IsDirectMessagesSelected));
        };

        FellowshipService.Instance.CurrentDmUserChanged += (u) =>
        {
            OnPropertyChanged(nameof(CurrentDmUser));
            OnPropertyChanged(nameof(IsDirectMessagesSelected));
        };

        AudioService.Instance.SpeakingStateChanged += (speaking) =>
        {
            IsSpeaking = speaking;
        };
    }

    [RelayCommand]
    public void SelectChannel(Channel? channel)
    {
        if (channel == null) return;

        if (channel.IsVoice)
        {
            CallService.Instance.JoinVoiceChannel(channel);
        }
        else
        {
            FellowshipService.Instance.SelectChannel(channel);
            _mainVM.ActiveView = ActiveMainView.Fellowship;
        }
    }

    [RelayCommand]
    public void SelectDmUser(User? user)
    {
        FellowshipService.Instance.SelectDirectMessage(user);
        _mainVM.ActiveView = ActiveMainView.DirectMessages;
    }

    [RelayCommand]
    public void ToggleCategory(ChannelCategory category)
    {
        category.IsCollapsed = !category.IsCollapsed;
    }

    [RelayCommand]
    public void ToggleMute()
    {
        IsMuted = !IsMuted;
        AudioService.Instance.IsMuted = IsMuted;
        CurrentUser.IsMuted = IsMuted;
        CallService.Instance.ToggleMute();
    }

    [RelayCommand]
    public void ToggleDeafen()
    {
        IsDeafened = !IsDeafened;
        AudioService.Instance.IsDeafened = IsDeafened;
        CurrentUser.IsDeafened = IsDeafened;
        CallService.Instance.ToggleDeafen();
    }

    [RelayCommand]
    public void OpenFellowshipSettings()
    {
        _mainVM.OpenFellowshipSettingsDialog();
    }

    [RelayCommand]
    public void OpenUserSettings()
    {
        _mainVM.OpenUserSettingsDialog();
    }

    [RelayCommand]
    public void StartDirectCallWithUser(User user)
    {
        CallService.Instance.StartDirectCall(user, isVideo: false);
    }
}
