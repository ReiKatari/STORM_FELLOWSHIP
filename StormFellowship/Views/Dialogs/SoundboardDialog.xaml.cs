using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class SoundboardDialog : UserControl
{
    public SoundboardDialog()
    {
        InitializeComponent();
    }

    private void OnTrackClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is SoundboardTrack track)
        {
            SoundboardService.Instance.PlayTrack(track);
            if (DataContext is MainViewModel vm)
            {
                vm.ShowToastNotification($"Саундборд: {track.Title} воспроизведен!");
            }
        }
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseSoundboardDialog();
        }
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseSoundboardDialog();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
