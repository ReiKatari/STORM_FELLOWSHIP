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

    public void RefreshMembers()
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
        if (user == null) return;
        CallService.Instance.StartDirectCall(user);
    }

    [RelayCommand]
    public void VideoCallMember(User user)
    {
        if (user == null) return;
        CallService.Instance.StartDirectCall(user);
        CallService.Instance.ToggleVideo();
        _mainVM.ShowToastNotification($"📹 Начало видеозвонка с {user.DisplayName}");
    }

    [RelayCommand]
    public void MessageMember(User user)
    {
        if (user == null) return;
        FellowshipService.Instance.SelectDirectMessage(user);
    }

    [RelayCommand]
    public void ToggleMuteForMe(User user)
    {
        if (user != null)
        {
            user.IsMutedForMe = !user.IsMutedForMe;
            _mainVM.ShowToastNotification(user.IsMutedForMe
                ? $"🔇 Пользователь {user.DisplayName} заглушен для вас"
                : $"🔊 Звук пользователя {user.DisplayName} включен");
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
    public void SetMemberRole(object[] parameters)
    {
        if (parameters != null && parameters.Length >= 2 && parameters[0] is User user && parameters[1] is string roleName)
        {
            user.RoleName = roleName;
            user.RoleColorHex = roleName switch
            {
                "Модератор" => "#22C55E",
                "Оратор" => "#F59E0B",
                "Администратор" => "#EF4444",
                _ => "#94A3B8"
            };
            RefreshMembers();
            _mainVM.ShowToastNotification($"👑 Роль {roleName} присвоена {user.DisplayName}");
        }
    }

    [RelayCommand]
    public void KickMember(User user)
    {
        if (user != null && CurrentFellowship != null)
        {
            CurrentFellowship.Members.Remove(user);
            RefreshMembers();
            _mainVM.ShowToastNotification($"🚫 Пользователь {user.DisplayName} исключен из содружества");
        }
    }

    [RelayCommand]
    public void BanMember(User user)
    {
        if (user != null && CurrentFellowship != null)
        {
            CurrentFellowship.Members.Remove(user);
            RefreshMembers();
            _mainVM.ShowToastNotification($"⛔ Пользователь {user.DisplayName} заблокирован");
        }
    }

    [RelayCommand]
    public void VerifyE2EE(User user)
    {
        _mainVM.OpenE2EESecurityDialog();
    }
}
