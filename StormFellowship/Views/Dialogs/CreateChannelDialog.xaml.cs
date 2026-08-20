using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class CreateChannelDialog : UserControl
{
    public CreateChannelDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseCreateChannelDialog();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnTextTypeChecked(object sender, RoutedEventArgs e)
    {
        if (BitratePanel != null) BitratePanel.Visibility = Visibility.Collapsed;
        if (DataContext is MainViewModel vm) vm.NewChannelType = ChannelType.Text;
    }

    private void OnVoiceTypeChecked(object sender, RoutedEventArgs e)
    {
        if (BitratePanel != null) BitratePanel.Visibility = Visibility.Visible;
        if (DataContext is MainViewModel vm) vm.NewChannelType = ChannelType.Voice;
    }

    private void OnAnnounceTypeChecked(object sender, RoutedEventArgs e)
    {
        if (BitratePanel != null) BitratePanel.Visibility = Visibility.Collapsed;
        if (DataContext is MainViewModel vm) vm.NewChannelType = ChannelType.Announcements;
    }
}
