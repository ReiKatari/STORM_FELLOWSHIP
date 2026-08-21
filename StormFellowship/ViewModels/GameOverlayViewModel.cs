using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class GameOverlayViewModel : ObservableObject
{
    [ObservableProperty]
    private double _overlayOpacity = 0.90;

    [ObservableProperty]
    private bool _isLocked = false;

    [ObservableProperty]
    private bool _isChatExpanded = true;

    [ObservableProperty]
    private string _quickMessageText = string.Empty;

    public User CurrentUser => FellowshipService.Instance.CurrentUser;
    public Channel? CurrentChannel => FellowshipService.Instance.CurrentChannel;

    public ObservableCollection<User> SpeakingParticipants => FellowshipService.Instance.CurrentFellowship?.Members
        ?? new ObservableCollection<User> { CurrentUser };

    public ObservableCollection<ChatMessage> RecentChatMessages => CurrentChannel?.Messages
        ?? new ObservableCollection<ChatMessage>();

    public bool IsMicMuted => AudioService.Instance.IsMuted;
    public bool IsDeafened => AudioService.Instance.IsDeafened;

    public string MicIcon => IsMicMuted ? "🔴" : "🎙️";
    public string SoundIcon => IsDeafened ? "🔴" : "🎧";

    public GameOverlayViewModel()
    {
        FellowshipService.Instance.CurrentChannelChanged += (c) =>
        {
            OnPropertyChanged(nameof(CurrentChannel));
            OnPropertyChanged(nameof(RecentChatMessages));
        };

        FellowshipService.Instance.CurrentFellowshipChanged += (f) =>
        {
            OnPropertyChanged(nameof(SpeakingParticipants));
        };

        AudioService.Instance.SpeakingStateChanged += (s) =>
        {
            OnPropertyChanged(nameof(SpeakingParticipants));
        };
    }

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(CurrentUser));
        OnPropertyChanged(nameof(SpeakingParticipants));
        OnPropertyChanged(nameof(CurrentChannel));
        OnPropertyChanged(nameof(RecentChatMessages));
        OnPropertyChanged(nameof(IsMicMuted));
        OnPropertyChanged(nameof(IsDeafened));
    }

    [RelayCommand]
    public void ToggleMute()
    {
        AudioService.Instance.IsMuted = !AudioService.Instance.IsMuted;
        OnPropertyChanged(nameof(IsMicMuted));
        OnPropertyChanged(nameof(MicIcon));
    }

    [RelayCommand]
    public void ToggleDeafen()
    {
        AudioService.Instance.IsDeafened = !AudioService.Instance.IsDeafened;
        OnPropertyChanged(nameof(IsDeafened));
        OnPropertyChanged(nameof(SoundIcon));
    }

    [RelayCommand]
    public void ToggleChat()
    {
        IsChatExpanded = !IsChatExpanded;
    }

    [RelayCommand]
    public void SendQuickMessage()
    {
        if (string.IsNullOrWhiteSpace(QuickMessageText) || CurrentChannel == null) return;
        var msg = ChatService.Instance.SendMessage(CurrentChannel.Id, QuickMessageText);
        CurrentChannel.Messages.Add(msg);
        QuickMessageText = string.Empty;
        OnPropertyChanged(nameof(RecentChatMessages));
    }

    [ObservableProperty]
    private User? _selectedParticipant;

    [RelayCommand]
    public void SelectParticipant(User? user)
    {
        SelectedParticipant = user;
    }

    [RelayCommand]
    public void MuteParticipantForMe(User? user)
    {
        if (user != null)
        {
            user.IsMutedForMe = !user.IsMutedForMe;
        }
    }

    [RelayCommand]
    public void CloseOverlay()
    {
        GameOverlayService.Instance.HideOverlay();
    }
}
