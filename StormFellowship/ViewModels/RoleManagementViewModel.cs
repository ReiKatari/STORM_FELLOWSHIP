using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class RoleManagementViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public Fellowship? CurrentFellowship => FellowshipService.Instance.CurrentFellowship;

    public ObservableCollection<Role> Roles => CurrentFellowship?.Roles ?? new ObservableCollection<Role>();

    [ObservableProperty]
    private Role? _selectedRole;

    [ObservableProperty]
    private string _editingRoleName = string.Empty;

    [ObservableProperty]
    private string _editingRoleColor = "#00A3FF";

    [ObservableProperty]
    private bool _permSendMessages = true;

    [ObservableProperty]
    private bool _permConnectVoice = true;

    [ObservableProperty]
    private bool _permPrioritySpeaker = false;

    [ObservableProperty]
    private bool _permManageChannels = false;

    [ObservableProperty]
    private bool _permAdministrator = false;

    public ObservableCollection<string> ColorPresets { get; } = new()
    {
        "#00A3FF", "#22C55E", "#F59E0B", "#EF4444", "#A855F7",
        "#EC4899", "#06B6D4", "#EAB308", "#10B981", "#64748B"
    };

    public RoleManagementViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;
        SelectedRole = Roles.FirstOrDefault();
    }

    partial void OnSelectedRoleChanged(Role? value)
    {
        if (value != null)
        {
            EditingRoleName = value.Name;
            EditingRoleColor = value.ColorHex;
            PermSendMessages = value.HasPermission(RolePermissions.SendMessages);
            PermConnectVoice = value.HasPermission(RolePermissions.ConnectVoice);
            PermPrioritySpeaker = value.HasPermission(RolePermissions.PrioritySpeaker);
            PermManageChannels = value.HasPermission(RolePermissions.ManageChannels);
            PermAdministrator = value.HasPermission(RolePermissions.Administrator);
        }
    }

    [RelayCommand]
    public void CreateNewRole()
    {
        if (CurrentFellowship == null) return;
        var newRole = new Role
        {
            Name = "Новая роль",
            ColorHex = "#22C55E",
            Permissions = RolePermissions.SendMessages | RolePermissions.ConnectVoice | RolePermissions.Speak
        };
        CurrentFellowship.Roles.Add(newRole);
        SelectedRole = newRole;
        _mainVM.ShowToastNotification("Создана новая роль");
    }

    [RelayCommand]
    public void SaveCurrentRole()
    {
        if (SelectedRole == null) return;

        SelectedRole.Name = EditingRoleName;
        SelectedRole.ColorHex = EditingRoleColor;

        RolePermissions perms = RolePermissions.None;
        if (PermSendMessages) perms |= RolePermissions.SendMessages | RolePermissions.AttachFiles;
        if (PermConnectVoice) perms |= RolePermissions.ConnectVoice | RolePermissions.Speak | RolePermissions.Video;
        if (PermPrioritySpeaker) perms |= RolePermissions.PrioritySpeaker;
        if (PermManageChannels) perms |= RolePermissions.ManageChannels;
        if (PermAdministrator) perms |= RolePermissions.Administrator | (RolePermissions)0x3FFF;

        SelectedRole.Permissions = perms;
        _mainVM.ShowToastNotification($"Роль «{SelectedRole.Name}» сохранена");
    }

    [RelayCommand]
    public void DeleteCurrentRole()
    {
        if (SelectedRole == null || CurrentFellowship == null) return;
        var name = SelectedRole.Name;
        CurrentFellowship.Roles.Remove(SelectedRole);
        SelectedRole = Roles.FirstOrDefault();
        _mainVM.ShowToastNotification($"Роль «{name}» удалена");
    }

    [RelayCommand]
    public void SelectColorPreset(string color)
    {
        EditingRoleColor = color;
    }

    [RelayCommand]
    public void Close()
    {
        _mainVM.IsRoleManagementModalOpen = false;
    }
}
