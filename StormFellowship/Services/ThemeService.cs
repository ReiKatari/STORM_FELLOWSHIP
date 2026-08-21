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

    public void SetAccentColor(string hex)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hex)) return;
            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);
            var accentBrush = new System.Windows.Media.SolidColorBrush(color);
            var hoverColor = System.Windows.Media.Color.FromArgb(0xFF, (byte)Math.Min(255, color.R + 35), (byte)Math.Min(255, color.G + 35), (byte)Math.Min(255, color.B + 35));
            var hoverBrush = new System.Windows.Media.SolidColorBrush(hoverColor);
            var glowBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0x40, color.R, color.G, color.B));

            if (Application.Current != null)
            {
                Application.Current.Resources["AccentBrush"] = accentBrush;
                Application.Current.Resources["AccentPrimaryBrush"] = accentBrush;
                Application.Current.Resources["AccentHoverBrush"] = hoverBrush;
                Application.Current.Resources["AccentGlowBrush"] = glowBrush;
            }
        }
        catch { }
    }
}
