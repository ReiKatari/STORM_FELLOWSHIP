using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class SearchDialog : UserControl
{
    public SearchDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseSearchDialog();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is MainViewModel vm)
            {
                vm.ExecuteSearch();
            }
            e.Handled = true;
        }
    }
}
