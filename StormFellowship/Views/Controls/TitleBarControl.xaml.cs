using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class TitleBarControl : UserControl
{
    public TitleBarControl()
    {
        InitializeComponent();
    }

    private void OnTitleBarMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Window.GetWindow(this)?.DragMove();
        }
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
        if (DataContext is MainViewModel vm) vm.ShowToastNotification("Switched to STORM DARK");
    }

    private void OnNightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormNight);
        if (DataContext is MainViewModel vm) vm.ShowToastNotification("Switched to STORM NIGHT");
    }

    private void OnDayThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDay);
        if (DataContext is MainViewModel vm) vm.ShowToastNotification("Switched to STORM DAY");
    }

    private void OnMidnightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormMidnight);
        if (DataContext is MainViewModel vm) vm.ShowToastNotification("Switched to STORM MIDNIGHT");
    }
}
