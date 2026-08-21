using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class MemberListViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public Fellowship? CurrentFellowship => FellowshipService.Instance.CurrentFellowship;

    public ObservableCollection<User> Members => CurrentFellowship?.Members ?? FellowshipService.Instance.DirectMessageUsers;
    public string HeaderText => $"УЧАСТНИКИ — {OnlineMembers.Count}";

    public ObservableCollection<User> OnlineMembers { get; } = new();
    public ObservableCollection<User> OfflineMembers { get; } = new();

    public MemberListViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;

        FellowshipService.Instance.CurrentFellowshipChanged += (f) =>
        {
            RefreshMembers();
        };

        RefreshMembers();
    }

    private void RefreshMembers()
    {
        OnlineMembers.Clear();
        OfflineMembers.Clear();

        var members = CurrentFellowship?.Members ?? FellowshipService.Instance.DirectMessageUsers;
        foreach (var m in members)
        {
            if (m.Status == UserStatus.Offline)
            {
                OfflineMembers.Add(m);
            }
            else
            {
                OnlineMembers.Add(m);
            }
        }

        OnPropertyChanged(nameof(CurrentFellowship));
        OnPropertyChanged(nameof(Members));
        OnPropertyChanged(nameof(HeaderText));
    }

    [RelayCommand]
    public void CallMember(User user)
    {
        CallService.Instance.StartDirectCall(user);
    }

    [RelayCommand]
    public void MessageMember(User user)
    {
        FellowshipService.Instance.SelectDirectMessage(user);
    }

    [RelayCommand]
    public void ToggleMuteForMe(User user)
    {
        if (user != null)
        {
            user.IsMutedForMe = !user.IsMutedForMe;
            _mainVM.ShowToastNotification(user.IsMutedForMe
                ? $"Пользователь {user.DisplayName} заглушен для вас"
                : $"Звук пользователя {user.DisplayName} включен");
        }
    }

    [RelayCommand]
    public void TogglePrioritySpeaker(User user)
    {
        if (user != null)
        {
            user.IsPrioritySpeaker = !user.IsPrioritySpeaker;
            _mainVM.ShowToastNotification(user.IsPrioritySpeaker
                ? $"🎙️ {user.DisplayName} назначен приоритетным оратором"
                : $"Приоритетный голос отключен для {user.DisplayName}");
        }
    }

    [RelayCommand]
    public void VerifyE2EE(User user)
    {
        _mainVM.OpenE2EESecurityDialog();
    }
}
