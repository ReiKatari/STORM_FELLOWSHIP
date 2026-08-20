using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public partial class Fellowship : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = "Storm Sanctuary";

    [ObservableProperty]
    private string _tag = "STORM";

    [ObservableProperty]
    private string _description = "Official Storm Fellowship headquarters for gaming and voice ops.";

    [ObservableProperty]
    private string _iconUrl = "ms-appx:///Assets/Logo.png";

    [ObservableProperty]
    private string _bannerUrl = string.Empty;

    [ObservableProperty]
    private string _ownerId = string.Empty;

    [ObservableProperty]
    private int _unreadTotal = 0;

    [ObservableProperty]
    private bool _hasUnread = false;

    [ObservableProperty]
    private bool _isSelected = false;

    public ObservableCollection<Role> Roles { get; } = new();
    public ObservableCollection<ChannelCategory> Categories { get; } = new();
    public ObservableCollection<User> Members { get; } = new();

    public string ShortInitials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Name)) return "SF";
            var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return Name.Substring(0, Math.Min(3, Name.Length)).ToUpper();
            return string.Concat(parts.Take(3).Select(p => p[0])).ToUpper();
        }
    }
}
