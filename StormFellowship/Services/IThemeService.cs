using StormFellowship.Models;

namespace StormFellowship.Services;

public interface IThemeService
{
    ThemeType CurrentTheme { get; }
    event Action<ThemeType>? ThemeChanged;
    void SetTheme(ThemeType theme);
}
