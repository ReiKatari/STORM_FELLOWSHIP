using System.Windows;
using StormFellowship.Models;

namespace StormFellowship.Services;

public class ThemeService : IThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    public ThemeType CurrentTheme { get; private set; } = ThemeType.StormDark;
    public event Action<ThemeType>? ThemeChanged;

    public void ApplyTheme(ThemeType theme) => SetTheme(theme);

    public void SetTheme(ThemeType theme)
    {
        CurrentTheme = theme;
        try
        {
            string themeFile = theme switch
            {
                ThemeType.StormDark => "Themes/StormDarkTheme.xaml",
                ThemeType.StormNight => "Themes/StormNightTheme.xaml",
                ThemeType.StormDay => "Themes/StormDayTheme.xaml",
                ThemeType.StormMidnight => "Themes/StormMidnightTheme.xaml",
                _ => "Themes/StormDarkTheme.xaml"
            };

            var dict = new ResourceDictionary { Source = new Uri(themeFile, UriKind.RelativeOrAbsolute) };
            if (Application.Current != null)
            {
                var merged = Application.Current.Resources.MergedDictionaries;
                var existingTheme = merged.FirstOrDefault(d => d.Source != null && d.Source.OriginalString.Contains("Theme"));
                if (existingTheme != null)
                {
                    int index = merged.IndexOf(existingTheme);
                    merged[index] = dict;
                }
                else
                {
                    merged.Insert(0, dict);
                }
            }
        }
        catch
        {
            // Safe fallback
        }

        ThemeChanged?.Invoke(theme);
    }
}
