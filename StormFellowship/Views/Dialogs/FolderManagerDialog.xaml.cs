using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class FolderManagerDialog : UserControl
{
    public FolderManagerDialog()
    {
        InitializeComponent();
    }

    private void OnDialogKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.CloseFolderManager();
                e.Handled = true;
            }
        }
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseFolderManager();
            e.Handled = true;
        }
    }

    private void OnCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
