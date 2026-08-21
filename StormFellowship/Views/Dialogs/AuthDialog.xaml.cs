using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class AuthDialog : UserControl
{
    public AuthDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AuthViewModel vm)
        {
            vm.CloseModal();
        }
    }

    private void OnCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnDialogKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (DataContext is AuthViewModel vm)
            {
                vm.CloseModal();
            }
            e.Handled = true;
        }
    }

    private void OnLoginPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm && sender is PasswordBox pb)
        {
            vm.LoginPassword = pb.Password;
        }
    }

    private void OnRegisterPasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AuthViewModel vm && sender is PasswordBox pb)
        {
            vm.RegisterPassword = pb.Password;
        }
    }

    private void OnTabLoginClicked(object sender, RoutedEventArgs e)
    {
        TabLoginContent.Visibility = Visibility.Visible;
        TabRegisterContent.Visibility = Visibility.Collapsed;
        TabCloudContent.Visibility = Visibility.Collapsed;

        PillLogin.Background = (Brush)FindResource("CardHoverBrush");
        PillRegister.Background = Brushes.Transparent;
        PillCloud.Background = Brushes.Transparent;
    }

    private void OnTabRegisterClicked(object sender, RoutedEventArgs e)
    {
        TabLoginContent.Visibility = Visibility.Collapsed;
        TabRegisterContent.Visibility = Visibility.Visible;
        TabCloudContent.Visibility = Visibility.Collapsed;

        PillLogin.Background = Brushes.Transparent;
        PillRegister.Background = (Brush)FindResource("CardHoverBrush");
        PillCloud.Background = Brushes.Transparent;
    }

    private void OnTabCloudClicked(object sender, RoutedEventArgs e)
    {
        TabLoginContent.Visibility = Visibility.Collapsed;
        TabRegisterContent.Visibility = Visibility.Collapsed;
        TabCloudContent.Visibility = Visibility.Visible;

        PillLogin.Background = Brushes.Transparent;
        PillRegister.Background = Brushes.Transparent;
        PillCloud.Background = (Brush)FindResource("CardHoverBrush");
    }
}
