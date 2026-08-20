using System.Windows;
using System.Windows.Controls;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class UserSettingsDialog : UserControl
{
    public UserSettingsDialog()
    {
        InitializeComponent();
    }

    private void HideAllTabs()
    {
        TabThemesContent.Visibility = Visibility.Collapsed;
        TabAudioContent.Visibility = Visibility.Collapsed;
        TabProfileContent.Visibility = Visibility.Collapsed;
        TabHotkeysContent.Visibility = Visibility.Collapsed;
        TabAboutContent.Visibility = Visibility.Collapsed;
    }

    private void OnTabThemesClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        TabThemesContent.Visibility = Visibility.Visible;
    }

    private void OnTabAudioClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        TabAudioContent.Visibility = Visibility.Visible;
    }

    private void OnTabProfileClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        TabProfileContent.Visibility = Visibility.Visible;
    }

    private void OnTabHotkeysClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        TabHotkeysContent.Visibility = Visibility.Visible;
    }

    private void OnTabAboutClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        TabAboutContent.Visibility = Visibility.Visible;
    }

    private void OnThemeDarkClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDark);
    }

    private void OnThemeNightClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormNight);
    }

    private void OnThemeDayClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDay);
    }

    private void OnThemeMidnightClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormMidnight);
    }
}
