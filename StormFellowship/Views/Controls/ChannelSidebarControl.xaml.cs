using System.Windows;
using System.Windows.Controls;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class ChannelSidebarControl : UserControl
{
    public ChannelSidebarControl()
    {
        InitializeComponent();
    }

    private void OnCreateChannelClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChannelSidebarViewModel vm)
        {
            if (vm.CurrentFellowship != null)
            {
                var cat = vm.CurrentFellowship.Categories.FirstOrDefault();
                if (cat != null)
                {
                    Services.FellowshipService.Instance.AddChannel(vm.CurrentFellowship.Id, cat.Id, "new-channel", ChannelType.Text);
                }
            }
        }
    }

    private void OnServerSettingsClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChannelSidebarViewModel vm)
        {
            vm.OpenFellowshipSettings();
        }
    }

    private void OnCategoryHeaderClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChannelCategory cat)
        {
            if (DataContext is ChannelSidebarViewModel vm)
            {
                vm.ToggleCategory(cat);
            }
        }
    }

    private void OnChannelClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Channel chan)
        {
            if (DataContext is ChannelSidebarViewModel vm)
            {
                vm.SelectChannel(chan);
            }
        }
    }

    private void OnDirectCallButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            if (DataContext is ChannelSidebarViewModel vm)
            {
                vm.StartDirectCallWithUser(user);
            }
        }
    }
}
