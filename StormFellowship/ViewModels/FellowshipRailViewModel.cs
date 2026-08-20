using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StormFellowship.Models;
using StormFellowship.Services;

namespace StormFellowship.ViewModels;

public partial class FellowshipRailViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;

    public ObservableCollection<Fellowship> Fellowships => FellowshipService.Instance.Fellowships;

    [ObservableProperty]
    private bool _isDirectMessagesSelected = false;

    public FellowshipRailViewModel(MainViewModel mainVM)
    {
        _mainVM = mainVM;
    }

    [RelayCommand]
    public void SelectFellowship(Fellowship? fellowship)
    {
        IsDirectMessagesSelected = false;
        FellowshipService.Instance.SelectFellowship(fellowship);
        _mainVM.ActiveView = ActiveMainView.Fellowship;
    }

    [RelayCommand]
    public void SelectDirectMessages()
    {
        IsDirectMessagesSelected = true;
        var firstDm = FellowshipService.Instance.DirectMessageUsers.FirstOrDefault();
        FellowshipService.Instance.SelectDirectMessage(firstDm);
        _mainVM.ActiveView = ActiveMainView.DirectMessages;
    }

    [RelayCommand]
    public void AddFellowship()
    {
        _mainVM.OpenCreateFellowshipDialog();
    }

    [RelayCommand]
    public void OpenSettings()
    {
        _mainVM.OpenUserSettingsDialog();
    }
}
