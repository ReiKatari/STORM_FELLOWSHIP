using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public sealed partial class FellowshipRailControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(FellowshipRailViewModel), typeof(FellowshipRailControl), new PropertyMetadata(null));

    public FellowshipRailViewModel ViewModel
    {
        get => (FellowshipRailViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public FellowshipRailControl()
    {
        InitializeComponent();
    }

    private void OnFellowshipButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Fellowship fellowship)
        {
            ViewModel?.SelectFellowship(fellowship);
        }
    }
}
