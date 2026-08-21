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
    public ObservableCollection<FellowshipFolder> Folders => FellowshipService.Instance.Folders;

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
    }

    [RelayCommand]
    public void ToggleFolder(FellowshipFolder? folder)
    {
        if (folder != null)
        {
            folder.IsExpanded = !folder.IsExpanded;
        }
    }

    [RelayCommand]
    public void SelectHome()
    {
        SelectDirectMessages();
    }

    [RelayCommand]
    public void SelectDirectMessages()
    {
        IsDirectMessagesSelected = true;
        var firstDm = FellowshipService.Instance.DirectMessageUsers.FirstOrDefault();
        FellowshipService.Instance.SelectDirectMessage(firstDm);
    }

    [RelayCommand]
    public void CreateFellowship()
    {
        AddFellowship();
    }

    [RelayCommand]
    public void AddFellowship()
    {
        _mainVM.OpenCreateFellowshipDialog();
    }

    [RelayCommand]
    public void OpenUserSettings()
    {
        _mainVM.OpenUserSettingsDialog();
    }

    [RelayCommand]
    public void OpenSettings()
    {
        _mainVM.OpenUserSettingsDialog();
    }
}
