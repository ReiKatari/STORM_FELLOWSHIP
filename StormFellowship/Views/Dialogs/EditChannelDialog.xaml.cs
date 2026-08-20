using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class EditChannelDialog : UserControl
{
    public EditChannelDialog()
    {
        InitializeComponent();
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.CloseEditChannelDialog();
        }
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
