using System.Windows;
using StormFellowship.ViewModels;

namespace StormFellowship;

public partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();
        DataContext = ViewModel;
    }

    private void OnToggleMemberListRequested(object sender, RoutedEventArgs e)
    {
        ViewModel.ToggleMemberList();
    }
}
