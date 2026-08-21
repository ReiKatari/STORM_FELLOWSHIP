using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class E2EESecurityDialog : UserControl
{
    public E2EESecurityDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseE2EESecurityDialog();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
