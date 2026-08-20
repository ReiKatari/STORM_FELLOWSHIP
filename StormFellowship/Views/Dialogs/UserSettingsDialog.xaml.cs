using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public sealed partial class UserSettingsDialog : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(UserSettingsViewModel), typeof(UserSettingsDialog), new PropertyMetadata(null));

    public UserSettingsViewModel ViewModel
    {
        get => (UserSettingsViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public event RoutedEventHandler? CloseRequested;

    public UserSettingsDialog()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        CloseRequested?.Invoke(this, e);
    }

    private void OnNavThemesClicked(object sender, RoutedEventArgs e)
    {
        ThemesTab.Visibility = Visibility.Visible;
        VoiceTab.Visibility = Visibility.Collapsed;
        ProfileTab.Visibility = Visibility.Collapsed;
        AboutTab.Visibility = Visibility.Collapsed;
    }

    private void OnNavVoiceClicked(object sender, RoutedEventArgs e)
    {
        ThemesTab.Visibility = Visibility.Collapsed;
        VoiceTab.Visibility = Visibility.Visible;
        ProfileTab.Visibility = Visibility.Collapsed;
        AboutTab.Visibility = Visibility.Collapsed;
    }

    private void OnNavProfileClicked(object sender, RoutedEventArgs e)
    {
        ThemesTab.Visibility = Visibility.Collapsed;
        VoiceTab.Visibility = Visibility.Collapsed;
        ProfileTab.Visibility = Visibility.Visible;
        AboutTab.Visibility = Visibility.Collapsed;
    }

    private void OnNavAboutClicked(object sender, RoutedEventArgs e)
    {
        ThemesTab.Visibility = Visibility.Collapsed;
        VoiceTab.Visibility = Visibility.Collapsed;
        ProfileTab.Visibility = Visibility.Collapsed;
        AboutTab.Visibility = Visibility.Visible;
    }

    private void OnApplyDarkClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.SelectTheme("StormDark");
    }

    private void OnApplyNightClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.SelectTheme("StormNight");
    }

    private void OnApplyDayClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.SelectTheme("StormDay");
    }

    private void OnApplyMidnightClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.SelectTheme("StormMidnight");
    }

    private void OnStatusOnlineClicked(object sender, RoutedEventArgs e) => ViewModel?.SetUserStatus("Online");
    private void OnStatusIdleClicked(object sender, RoutedEventArgs e) => ViewModel?.SetUserStatus("Idle");
    private void OnStatusDndClicked(object sender, RoutedEventArgs e) => ViewModel?.SetUserStatus("DoNotDisturb");
    private void OnStatusOfflineClicked(object sender, RoutedEventArgs e) => ViewModel?.SetUserStatus("Offline");
}
