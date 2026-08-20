using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public sealed partial class ChannelSidebarControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ChannelSidebarViewModel), typeof(ChannelSidebarControl), new PropertyMetadata(null));

    public ChannelSidebarViewModel ViewModel
    {
        get => (ChannelSidebarViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ChannelSidebarControl()
    {
        InitializeComponent();
    }

    private void OnCategoryHeaderClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChannelCategory cat)
        {
            ViewModel?.ToggleCategory(cat);
        }
    }

    private void OnChannelClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Channel chan)
        {
            ViewModel?.SelectChannel(chan);
        }
    }

    private void OnDmUserClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            ViewModel?.SelectDmUser(user);
        }
    }

    private void OnDirectCallButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            ViewModel?.StartDirectCallWithUser(user);
        }
    }

    private void OnServerSettingsClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.OpenFellowshipSettings();
    }

    private void OnCreateChannelClicked(object sender, RoutedEventArgs e)
    {
        // Add default text channel
        if (ViewModel?.CurrentFellowship != null)
        {
            var cat = ViewModel.CurrentFellowship.Categories.FirstOrDefault();
            if (cat != null)
            {
                Services.FellowshipService.Instance.AddChannel(ViewModel.CurrentFellowship.Id, cat.Id, "new-channel", ChannelType.Text);
            }
        }
    }

    private void OnCopyInviteClicked(object sender, RoutedEventArgs e)
    {
        var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
        dp.SetText($"storm://invite/{ViewModel?.CurrentFellowship?.Id ?? "sanctuary"}");
        Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
    }
}
