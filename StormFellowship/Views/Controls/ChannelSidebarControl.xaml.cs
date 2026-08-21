using System.Windows;
using System.Windows.Controls;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class ChannelSidebarControl : UserControl
{
    public ChannelSidebarControl()
    {
        InitializeComponent();
    }

    private void OnChannelClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Channel chan && DataContext is ChannelSidebarViewModel vm)
        {
            vm.SelectChannel(chan);
        }
    }

    private void OnCategoryHeaderClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChannelCategory cat && DataContext is ChannelSidebarViewModel vm)
        {
            vm.ToggleCategory(cat);
        }
    }

    private void OnAddChannelToCategoryClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChannelCategory cat && DataContext is ChannelSidebarViewModel vm)
        {
            vm.OpenCreateChannel(cat);
        }
    }

    private void OnContextEditChannelClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item && item.Tag is Channel chan && DataContext is ChannelSidebarViewModel vm)
        {
            vm.OpenEditChannel(chan);
        }
    }

    private void OnContextDeleteChannelClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item && item.Tag is Channel chan && DataContext is ChannelSidebarViewModel vm)
        {
            vm.DeleteChannel(chan);
        }
    }

    private void OnDirectCallButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user && DataContext is ChannelSidebarViewModel vm)
        {
            vm.StartDirectCallWithUser(user);
        }
    }

    private void OnDirectVideoCallButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            CallService.Instance.StartDirectCall(user, isVideo: true);
        }
    }

    private void OnDmUserClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            FellowshipService.Instance.SelectDirectMessage(user);
        }
    }
}
