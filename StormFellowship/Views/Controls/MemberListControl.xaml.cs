using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public sealed partial class MemberListControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MemberListViewModel), typeof(MemberListControl), new PropertyMetadata(null));

    public MemberListViewModel ViewModel
    {
        get => (MemberListViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public MemberListControl()
    {
        InitializeComponent();
    }

    private void OnMemberClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is User user)
        {
            ViewModel?.MessageMember(user);
        }
    }

    private void OnSendDmClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is User user)
        {
            ViewModel?.MessageMember(user);
        }
    }

    private void OnStartCallClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem item && item.Tag is User user)
        {
            ViewModel?.CallMember(user);
        }
    }
}
