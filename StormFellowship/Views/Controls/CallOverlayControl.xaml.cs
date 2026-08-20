using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public sealed partial class CallOverlayControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(CallViewModel), typeof(CallOverlayControl), new PropertyMetadata(null));

    public CallViewModel ViewModel
    {
        get => (CallViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public CallOverlayControl()
    {
        InitializeComponent();
    }
}
