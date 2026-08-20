using System.Windows;
using System.Windows.Controls;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class TitleBarControl : UserControl
{
    public TitleBarControl()
    {
        InitializeComponent();
    }

    private void OnCheckUpdatesClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CheckForUpdates();
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
            win.WindowState = (win.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        var win = Window.GetWindow(this);
        win?.Close();
    }
}
