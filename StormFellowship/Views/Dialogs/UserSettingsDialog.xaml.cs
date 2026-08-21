using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is UserSettingsViewModel vm)
        {
            vm.Close();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void HideAllTabs()
    {
        TabThemesContent.Visibility = Visibility.Collapsed;
        TabAudioContent.Visibility = Visibility.Collapsed;
        TabProfileContent.Visibility = Visibility.Collapsed;
        TabHotkeysContent.Visibility = Visibility.Collapsed;
        TabAboutContent.Visibility = Visibility.Collapsed;

        var transparentBrush = System.Windows.Media.Brushes.Transparent;
        BtnTabThemes.Background = transparentBrush;
        BtnTabAudio.Background = transparentBrush;
        BtnTabProfile.Background = transparentBrush;
        BtnTabHotkeys.Background = transparentBrush;
        BtnTabAbout.Background = transparentBrush;
    }

    private void SetActiveButton(Button btn)
    {
        try
        {
            btn.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x2A, 0x00, 0xD2, 0xFF));
            btn.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x66, 0x00, 0xD2, 0xFF));
        }
        catch { }
    }

    private void OnTabThemesClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        SetActiveButton(BtnTabThemes);
        TabThemesContent.Visibility = Visibility.Visible;
    }

    private void OnTabAudioClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        SetActiveButton(BtnTabAudio);
        TabAudioContent.Visibility = Visibility.Visible;
    }

    private void OnTabProfileClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        SetActiveButton(BtnTabProfile);
        TabProfileContent.Visibility = Visibility.Visible;
    }

    private void OnTabHotkeysClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        SetActiveButton(BtnTabHotkeys);
        TabHotkeysContent.Visibility = Visibility.Visible;
    }

    private void OnTabAboutClicked(object sender, RoutedEventArgs e)
    {
        HideAllTabs();
        SetActiveButton(BtnTabAbout);
        TabAboutContent.Visibility = Visibility.Visible;
    }

    private void OnAvatarPresetClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string glyph && DataContext is UserSettingsViewModel vm)
        {
            vm.SelectAvatarPreset(glyph);
        }
    }

    private void OnStatusPresetClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is StatusPresetItem item && DataContext is UserSettingsViewModel vm)
        {
            vm.SelectStatusPreset(item);
        }
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
