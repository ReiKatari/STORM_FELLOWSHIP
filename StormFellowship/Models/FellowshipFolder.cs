using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public partial class FellowshipFolder : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "Папка";

    [ObservableProperty]
    private string _icon = "📁";

    [ObservableProperty]
    private string _colorHex = "#00A3FF";

    [ObservableProperty]
    private bool _isExpanded = false;

    public ObservableCollection<Fellowship> Fellowships { get; } = new();

    public int UnreadTotal => Fellowships.Sum(f => f.UnreadTotal);
    public bool HasUnread => UnreadTotal > 0;
}
