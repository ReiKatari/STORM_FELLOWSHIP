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

    private void OnFolderClicked(object sender, RoutedEventArgs e)
    {
        FellowshipFolder? folder = null;
        if (sender is Button btn && btn.Tag is FellowshipFolder f1) folder = f1;
        else if (sender is MenuItem mi && mi.Tag is FellowshipFolder f2) folder = f2;

        if (folder != null && DataContext is FellowshipRailViewModel vm)
        {
            vm.ToggleFolder(folder);
        }
    }

    private void OnFolderEditMenuClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is FellowshipFolder folder)
        {
            if (DataContext is FellowshipRailViewModel vm)
            {
                vm.EditFolder(folder);
            }
        }
    }

    private void OnFolderDeleteMenuClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is FellowshipFolder folder)
        {
            if (DataContext is FellowshipRailViewModel vm)
            {
                vm.DeleteFolder(folder);
            }
        }
    }

    private void OnNestedExtractMenuClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is Fellowship fellowship)
        {
            if (DataContext is FellowshipRailViewModel vm)
            {
                vm.ExtractFellowshipFromFolder(fellowship);
            }
        }
    }

    private void OnNestedDeleteMenuClicked(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem mi && mi.Tag is Fellowship fellowship)
        {
            if (DataContext is FellowshipRailViewModel vm)
            {
                vm.DeleteFellowship(fellowship);
            }
        }
    }
}
