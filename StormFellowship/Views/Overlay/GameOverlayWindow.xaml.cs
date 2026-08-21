using System.Windows;
using System.Windows.Input;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Overlay;

public partial class GameOverlayWindow : Window
{
    public GameOverlayWindow()
    {
        InitializeComponent();
        DataContext = new GameOverlayViewModel();
    }

    private void OnWindowDrag(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OnQuickMessageKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is GameOverlayViewModel vm)
            {
                vm.SendQuickMessage();
            }
            e.Handled = true;
        }
    }
}
