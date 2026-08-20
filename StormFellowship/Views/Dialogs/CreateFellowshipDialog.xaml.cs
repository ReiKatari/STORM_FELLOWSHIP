using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public sealed partial class CreateFellowshipDialog : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(CreateFellowshipDialog), new PropertyMetadata(null));

    public MainViewModel ViewModel
    {
        get => (MainViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public CreateFellowshipDialog()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.CloseCreateFellowshipDialog();
    }

    private void OnCreateClicked(object sender, RoutedEventArgs e)
    {
        string name = FellowshipNameInput.Text.Trim();
        string desc = FellowshipDescInput.Text.Trim();
        if (string.IsNullOrEmpty(name)) name = "My Storm Guild";

        FellowshipService.Instance.CreateFellowship(name, desc);
        ViewModel?.ShowToastNotification($"Created Fellowship '{name}'");
        ViewModel?.CloseCreateFellowshipDialog();
    }

    private void OnJoinWithCodeClicked(object sender, RoutedEventArgs e)
    {
        string code = InviteCodeInput.Text.Trim();
        if (!string.IsNullOrEmpty(code))
        {
            ViewModel?.ShowToastNotification($"Joined Fellowship via invite code!");
            ViewModel?.CloseCreateFellowshipDialog();
        }
    }
}
