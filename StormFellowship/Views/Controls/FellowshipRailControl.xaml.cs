using System.Windows;
using System.Windows.Controls;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class FellowshipRailControl : UserControl
{
    public FellowshipRailControl()
    {
        InitializeComponent();
    }

    private void OnFellowshipClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Fellowship fellowship)
        {
            if (DataContext is FellowshipRailViewModel vm)
            {
                vm.SelectFellowship(fellowship);
            }
        }
    }
}
