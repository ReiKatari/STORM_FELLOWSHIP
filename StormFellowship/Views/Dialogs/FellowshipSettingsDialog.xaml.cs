using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public sealed partial class FellowshipSettingsDialog : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(MainViewModel), typeof(FellowshipSettingsDialog), new PropertyMetadata(null));

    public MainViewModel ViewModel
    {
        get => (MainViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public FellowshipSettingsDialog()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        var f = FellowshipService.Instance.CurrentFellowship;
        if (f != null)
        {
            EditNameInput.Text = f.Name;
            EditDescInput.Text = f.Description;
        }
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        ViewModel?.CloseFellowshipSettingsDialog();
    }

    private void OnSaveChangesClicked(object sender, RoutedEventArgs e)
    {
        var f = FellowshipService.Instance.CurrentFellowship;
        if (f != null)
        {
            FellowshipService.Instance.RenameFellowship(f.Id, EditNameInput.Text.Trim(), EditDescInput.Text.Trim());
            ViewModel?.ShowToastNotification($"Saved settings for '{f.Name}'");
        }
        ViewModel?.CloseFellowshipSettingsDialog();
    }

    private void OnDeleteFellowshipClicked(object sender, RoutedEventArgs e)
    {
        var f = FellowshipService.Instance.CurrentFellowship;
        if (f != null)
        {
            string name = f.Name;
            FellowshipService.Instance.DeleteFellowship(f.Id);
            ViewModel?.ShowToastNotification($"Deleted fellowship '{name}'");
        }
        ViewModel?.CloseFellowshipSettingsDialog();
    }
}
