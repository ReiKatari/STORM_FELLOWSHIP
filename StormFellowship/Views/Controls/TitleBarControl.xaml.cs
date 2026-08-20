using System.Windows;
using System.Windows.Controls;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.Views.Controls;

public partial class TitleBarControl : UserControl
{
    public TitleBarControl()
    {
        InitializeComponent();
    }

    private void OnMinimizeClicked(object sender, RoutedEventArgs e)
    {
        var win = Window.GetWindow(this);
        if (win != null) win.WindowState = WindowState.Minimized;
    }

    private void OnMaximizeRestoreClicked(object sender, RoutedEventArgs e)
    {
        var win = Window.GetWindow(this);
        if (win != null)
        {
            win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        var win = Window.GetWindow(this);
        win?.Close();
    }

    private void OnDarkThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDark);
    }

    private void OnNightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormNight);
    }

    private void OnDayThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDay);
    }

    private void OnMidnightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormMidnight);
    }
}
