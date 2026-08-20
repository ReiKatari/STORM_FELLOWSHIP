using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public sealed partial class TitleBarControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(TitleBarControl), new PropertyMetadata(null));

    public MainViewModel ViewModel
    {
        get => (MainViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public TitleBarControl()
    {
        InitializeComponent();
    }

    private void OnDarkThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDark);
        ViewModel?.ShowToastNotification("Switched to STORM DARK");
    }

    private void OnNightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormNight);
        ViewModel?.ShowToastNotification("Switched to STORM NIGHT");
    }

    private void OnDayThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormDay);
        ViewModel?.ShowToastNotification("Switched to STORM DAY");
    }

    private void OnMidnightThemeClicked(object sender, RoutedEventArgs e)
    {
        ThemeService.Instance.SetTheme(ThemeType.StormMidnight);
        ViewModel?.ShowToastNotification("Switched to STORM MIDNIGHT");
    }
}
