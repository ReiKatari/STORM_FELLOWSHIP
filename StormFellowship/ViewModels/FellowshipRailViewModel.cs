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

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(Fellowships));
        OnPropertyChanged(nameof(Folders));
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
        if (folder == null) return;

        folder.IsExpanded = !folder.IsExpanded;

        if (folder.Fellowships.Count > 0)
        {
            if (folder.IsExpanded)
            {
                var first = folder.Fellowships.FirstOrDefault();
                if (first != null)
                {
                    FellowshipService.Instance.SelectFellowship(first);
                    _mainVM.ShowToastNotification($"📁 Папка «{folder.Name}» → {first.Name}");
                    return;
                }
            }
            _mainVM.ShowToastNotification(folder.IsExpanded ? $"📁 Папка «{folder.Name}» раскрыта" : $"📁 Папка «{folder.Name}» свернута");
        }
        else
        {
            _mainVM.OpenFolderManager(folder);
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
    public void CreateFolder()
    {
        var emojis = new[] { "📁", "🎮", "⚡", "🔥", "🛡️", "🚀", "🎯", "👑" };
        var colors = new[] { "#A855F7", "#00E5FF", "#10B981", "#F59E0B", "#EF4444", "#3B82F6" };
        var random = new Random();
        var icon = emojis[random.Next(emojis.Length)];
        var color = colors[random.Next(colors.Length)];
        
        var folder = FellowshipService.Instance.CreateFolder($"Папка {Folders.Count + 1}", icon, color);
        _mainVM.ShowToastNotification($"📁 Создана новая папка «{folder.Name}» {folder.Icon}");
    }

    [RelayCommand]
    public void DeleteFolder(FellowshipFolder? folder)
    {
        if (folder != null)
        {
            var name = folder.Name;
            FellowshipService.Instance.DeleteFolder(folder);
            _mainVM.ShowToastNotification($"🗑️ Папка «{name}» удалена");
        }
    }

    [RelayCommand]
    public void EditFolder(FellowshipFolder? folder)
    {
        if (folder != null)
        {
            _mainVM.OpenFolderManager(folder);
        }
    }

    [RelayCommand]
    public void ExtractFellowshipFromFolder(Fellowship? fellowship)
    {
        if (fellowship == null) return;
        foreach (var folder in Folders)
        {
            if (folder.Fellowships.Contains(fellowship))
            {
                folder.Fellowships.Remove(fellowship);
                if (!Fellowships.Contains(fellowship))
                {
                    Fellowships.Add(fellowship);
                }
                _mainVM.ShowToastNotification($"📤 Содружество «{fellowship.Name}» извлечено из папки «{folder.Name}»");
                break;
            }
        }
    }

    [RelayCommand]
    public void DeleteFellowship(Fellowship? fellowship)
    {
        if (fellowship == null) return;
        var name = fellowship.Name;
        FellowshipService.Instance.DeleteFellowship(fellowship.Id);
        _mainVM.ShowToastNotification($"🗑️ Содружество «{name}» удалено");
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
