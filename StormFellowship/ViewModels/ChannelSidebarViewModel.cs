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

    public string FellowshipTitle => IsDirectMessagesSelected ? "ЛИЧНЫЕ СООБЩЕНИЯ" : (CurrentFellowship?.Name ?? "СОДРУЖЕСТВО");
    public bool IsInDmMode => IsDirectMessagesSelected;
    public bool IsInFellowshipMode => !IsDirectMessagesSelected;
    public ObservableCollection<User> DmUsers => DirectMessageUsers;
    public ObservableCollection<ChannelCategory> Categories => CurrentFellowship?.Categories ?? new ObservableCollection<ChannelCategory>();

    public bool IsMuted => AudioService.Instance.IsMuted;
    public bool IsDeafened => AudioService.Instance.IsDeafened;
    public string MicIcon => IsMuted ? "🔇" : "🎤";
    public string SoundIcon => IsDeafened ? "🔇" : "🎧";
    public string MicTooltip => IsMuted ? "Включить микрофон" : "Отключить микрофон";
    public string SoundTooltip => IsDeafened ? "Включить звук" : "Заглушить звук";

    [ObservableProperty]
    private bool _isSpeaking = false;

    public ChannelSidebarViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        FellowshipService.Instance.CurrentFellowshipChanged += (f) =>
        {
            RefreshAll();
        };

        FellowshipService.Instance.CurrentDmUserChanged += (u) =>
        {
            RefreshAll();
        };

        AudioService.Instance.SpeakingStateChanged += (speaking) =>
        {
            IsSpeaking = speaking;
        };
    }

    public void RefreshAll()
    {
        OnPropertyChanged(nameof(CurrentFellowship));
        OnPropertyChanged(nameof(CurrentDmUser));
        OnPropertyChanged(nameof(IsDirectMessagesSelected));
        OnPropertyChanged(nameof(FellowshipTitle));
        OnPropertyChanged(nameof(IsInDmMode));
        OnPropertyChanged(nameof(IsInFellowshipMode));
        OnPropertyChanged(nameof(Categories));
        OnPropertyChanged(nameof(CurrentUser));
        OnPropertyChanged(nameof(IsMuted));
        OnPropertyChanged(nameof(IsDeafened));
        OnPropertyChanged(nameof(MicIcon));
        OnPropertyChanged(nameof(SoundIcon));
        OnPropertyChanged(nameof(MicTooltip));
        OnPropertyChanged(nameof(SoundTooltip));
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
        }
    }

    [RelayCommand]
    public void SelectDmUser(User? user)
    {
        FellowshipService.Instance.SelectDirectMessage(user);
    }

    [RelayCommand]
    public void ToggleCategory(ChannelCategory category)
    {
        category.IsCollapsed = !category.IsCollapsed;
    }

    [RelayCommand]
    public void OpenCreateChannel(ChannelCategory? category = null)
    {
        _mainVM.OpenCreateChannelDialog(category);
    }

    [RelayCommand]
    public void OpenEditChannel(Channel? channel)
    {
        if (channel != null)
        {
            _mainVM.OpenEditChannelDialog(channel);
        }
    }

    [RelayCommand]
    public void DeleteChannel(Channel? channel)
    {
        if (channel != null && CurrentFellowship != null)
        {
            FellowshipService.Instance.DeleteChannel(CurrentFellowship.Id, channel.Id);
            _mainVM.ShowToastNotification($"Канал #{channel.Name} удален");
            RefreshAll();
        }
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
