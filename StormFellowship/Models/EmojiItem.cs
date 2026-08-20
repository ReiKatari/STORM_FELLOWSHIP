using CommunityToolkit.Mvvm.ComponentModel;

namespace StormFellowship.Models;

public partial class EmojiItem : ObservableObject
{
    [ObservableProperty]
    private string _code = string.Empty;

    [ObservableProperty]
    private string _symbol = string.Empty;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _category = "Storm";

    [ObservableProperty]
    private bool _isCustom;

    [ObservableProperty]
    private string _customImagePath = string.Empty;
}

public partial class StickerItem : ObservableObject
{
    [ObservableProperty]
    private string _id = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    private string _packName = "Storm Fellowship Essentials";
}
