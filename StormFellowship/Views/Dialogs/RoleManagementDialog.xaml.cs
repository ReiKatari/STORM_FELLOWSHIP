using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class RoleManagementDialog : UserControl
{
    public RoleManagementDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is RoleManagementViewModel vm)
        {
            vm.Close();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnRoleItemClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Role role && DataContext is RoleManagementViewModel vm)
        {
            vm.SelectedRole = role;
        }
    }

    private void OnColorPresetClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string color && DataContext is RoleManagementViewModel vm)
        {
            vm.SelectColorPreset(color);
        }
    }
}
