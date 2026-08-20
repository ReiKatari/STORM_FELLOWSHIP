using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using StormFellowship.Models;
using Windows.UI;

namespace StormFellowship.Services;

public class ThemeService : IThemeService
{
    private static ThemeService? _instance;
    public static ThemeService Instance => _instance ??= new ThemeService();

    public ThemeType CurrentTheme { get; private set; } = ThemeType.StormDark;
    public event Action<ThemeType>? ThemeChanged;

    public void SetTheme(ThemeType theme)
    {
        CurrentTheme = theme;
        ApplyThemeResources(theme);
        ThemeChanged?.Invoke(theme);
    }

    private void ApplyThemeResources(ThemeType theme)
    {
        var res = Application.Current.Resources;

        switch (theme)
        {
            case ThemeType.StormDark:
                UpdateBrush(res, "AppBackgroundBrush", ColorFromHex("#0B0F17"));
                UpdateBrush(res, "TitleBarBackgroundBrush", ColorFromHex("#090D14"));
                UpdateBrush(res, "RailBackgroundBrush", ColorFromHex("#070A0F"));
                UpdateBrush(res, "SidebarBackgroundBrush", ColorFromHex("#0F1520"));
                UpdateBrush(res, "ContentBackgroundBrush", ColorFromHex("#131B26"));
                UpdateBrush(res, "CardBackgroundBrush", ColorFromHex("#182230"));
                UpdateBrush(res, "CardHoverBrush", ColorFromHex("#202D3F"));
                UpdateBrush(res, "InputBackgroundBrush", ColorFromHex("#101722"));
                UpdateBrush(res, "BorderSubtleBrush", ColorFromHex("#1E2B3D"));
                UpdateBrush(res, "DividerBrush", ColorFromHex("#172230"));
                UpdateBrush(res, "AccentBrush", ColorFromHex("#00A3FF"));
                UpdateBrush(res, "AccentHoverBrush", ColorFromHex("#33B5FF"));
                UpdateBrush(res, "AccentPressedBrush", ColorFromHex("#008BE0"));
                UpdateBrush(res, "AccentGlowBrush", ColorFromHex("#3300A3FF"));
                UpdateBrush(res, "AccentMutedBrush", ColorFromHex("#1A00A3FF"));
                UpdateBrush(res, "TextPrimaryBrush", ColorFromHex("#F1F5F9"));
                UpdateBrush(res, "TextSecondaryBrush", ColorFromHex("#94A3B8"));
                UpdateBrush(res, "TextMutedBrush", ColorFromHex("#64748B"));
                UpdateBrush(res, "TextOnAccentBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "CallContainerBackgroundBrush", ColorFromHex("#111722"));
                UpdateBrush(res, "CallControlBarBackgroundBrush", ColorFromHex("#0C1017"));
                UpdateBrush(res, "CallControlButtonBrush", ColorFromHex("#182332"));
                UpdateBrush(res, "CallControlButtonHoverBrush", ColorFromHex("#223247"));
                UpdateBrush(res, "ModalDialogBackgroundBrush", ColorFromHex("#111722"));
                UpdateBrush(res, "ModalHeaderBackgroundBrush", ColorFromHex("#0D121B"));
                break;

            case ThemeType.StormNight:
                UpdateBrush(res, "AppBackgroundBrush", ColorFromHex("#000000"));
                UpdateBrush(res, "TitleBarBackgroundBrush", ColorFromHex("#000000"));
                UpdateBrush(res, "RailBackgroundBrush", ColorFromHex("#000000"));
                UpdateBrush(res, "SidebarBackgroundBrush", ColorFromHex("#08080A"));
                UpdateBrush(res, "ContentBackgroundBrush", ColorFromHex("#101014"));
                UpdateBrush(res, "CardBackgroundBrush", ColorFromHex("#18181E"));
                UpdateBrush(res, "CardHoverBrush", ColorFromHex("#22222A"));
                UpdateBrush(res, "InputBackgroundBrush", ColorFromHex("#0B0B0E"));
                UpdateBrush(res, "BorderSubtleBrush", ColorFromHex("#24242C"));
                UpdateBrush(res, "DividerBrush", ColorFromHex("#1B1B22"));
                UpdateBrush(res, "AccentBrush", ColorFromHex("#00E5FF"));
                UpdateBrush(res, "AccentHoverBrush", ColorFromHex("#48EEFF"));
                UpdateBrush(res, "AccentPressedBrush", ColorFromHex("#00B5CC"));
                UpdateBrush(res, "AccentGlowBrush", ColorFromHex("#4000E5FF"));
                UpdateBrush(res, "AccentMutedBrush", ColorFromHex("#1A00E5FF"));
                UpdateBrush(res, "TextPrimaryBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "TextSecondaryBrush", ColorFromHex("#A1A1AA"));
                UpdateBrush(res, "TextMutedBrush", ColorFromHex("#71717A"));
                UpdateBrush(res, "TextOnAccentBrush", ColorFromHex("#000000"));
                UpdateBrush(res, "CallContainerBackgroundBrush", ColorFromHex("#0B0B0E"));
                UpdateBrush(res, "CallControlBarBackgroundBrush", ColorFromHex("#040405"));
                UpdateBrush(res, "CallControlButtonBrush", ColorFromHex("#18181E"));
                UpdateBrush(res, "CallControlButtonHoverBrush", ColorFromHex("#22222A"));
                UpdateBrush(res, "ModalDialogBackgroundBrush", ColorFromHex("#0E0E12"));
                UpdateBrush(res, "ModalHeaderBackgroundBrush", ColorFromHex("#08080B"));
                break;

            case ThemeType.StormDay:
                UpdateBrush(res, "AppBackgroundBrush", ColorFromHex("#F1F5F9"));
                UpdateBrush(res, "TitleBarBackgroundBrush", ColorFromHex("#E2E8F0"));
                UpdateBrush(res, "RailBackgroundBrush", ColorFromHex("#E2E8F0"));
                UpdateBrush(res, "SidebarBackgroundBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "ContentBackgroundBrush", ColorFromHex("#F8FAFC"));
                UpdateBrush(res, "CardBackgroundBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "CardHoverBrush", ColorFromHex("#EDF2F7"));
                UpdateBrush(res, "InputBackgroundBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "BorderSubtleBrush", ColorFromHex("#CBD5E1"));
                UpdateBrush(res, "DividerBrush", ColorFromHex("#E2E8F0"));
                UpdateBrush(res, "AccentBrush", ColorFromHex("#0284C7"));
                UpdateBrush(res, "AccentHoverBrush", ColorFromHex("#38BDF8"));
                UpdateBrush(res, "AccentPressedBrush", ColorFromHex("#0369A1"));
                UpdateBrush(res, "AccentGlowBrush", ColorFromHex("#250284C7"));
                UpdateBrush(res, "AccentMutedBrush", ColorFromHex("#1A0284C7"));
                UpdateBrush(res, "TextPrimaryBrush", ColorFromHex("#0F172A"));
                UpdateBrush(res, "TextSecondaryBrush", ColorFromHex("#475569"));
                UpdateBrush(res, "TextMutedBrush", ColorFromHex("#94A3B8"));
                UpdateBrush(res, "TextOnAccentBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "CallContainerBackgroundBrush", ColorFromHex("#F1F5F9"));
                UpdateBrush(res, "CallControlBarBackgroundBrush", ColorFromHex("#E2E8F0"));
                UpdateBrush(res, "CallControlButtonBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "CallControlButtonHoverBrush", ColorFromHex("#E2E8F0"));
                UpdateBrush(res, "ModalDialogBackgroundBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "ModalHeaderBackgroundBrush", ColorFromHex("#F1F5F9"));
                break;

            case ThemeType.StormMidnight:
                UpdateBrush(res, "AppBackgroundBrush", ColorFromHex("#0D0714"));
                UpdateBrush(res, "TitleBarBackgroundBrush", ColorFromHex("#08040C"));
                UpdateBrush(res, "RailBackgroundBrush", ColorFromHex("#06020A"));
                UpdateBrush(res, "SidebarBackgroundBrush", ColorFromHex("#140B21"));
                UpdateBrush(res, "ContentBackgroundBrush", ColorFromHex("#1C0F2E"));
                UpdateBrush(res, "CardBackgroundBrush", ColorFromHex("#25143C"));
                UpdateBrush(res, "CardHoverBrush", ColorFromHex("#321C50"));
                UpdateBrush(res, "InputBackgroundBrush", ColorFromHex("#150B22"));
                UpdateBrush(res, "BorderSubtleBrush", ColorFromHex("#3B1D5F"));
                UpdateBrush(res, "DividerBrush", ColorFromHex("#2D1648"));
                UpdateBrush(res, "AccentBrush", ColorFromHex("#A855F7"));
                UpdateBrush(res, "AccentHoverBrush", ColorFromHex("#C084FC"));
                UpdateBrush(res, "AccentPressedBrush", ColorFromHex("#9333EA"));
                UpdateBrush(res, "AccentGlowBrush", ColorFromHex("#40A855F7"));
                UpdateBrush(res, "AccentMutedBrush", ColorFromHex("#20A855F7"));
                UpdateBrush(res, "TextPrimaryBrush", ColorFromHex("#FAF5FF"));
                UpdateBrush(res, "TextSecondaryBrush", ColorFromHex("#E9D5FF"));
                UpdateBrush(res, "TextMutedBrush", ColorFromHex("#A855F7"));
                UpdateBrush(res, "TextOnAccentBrush", ColorFromHex("#FFFFFF"));
                UpdateBrush(res, "CallContainerBackgroundBrush", ColorFromHex("#170B26"));
                UpdateBrush(res, "CallControlBarBackgroundBrush", ColorFromHex("#0E0617"));
                UpdateBrush(res, "CallControlButtonBrush", ColorFromHex("#25143C"));
                UpdateBrush(res, "CallControlButtonHoverBrush", ColorFromHex("#341B53"));
                UpdateBrush(res, "ModalDialogBackgroundBrush", ColorFromHex("#1B0E2C"));
                UpdateBrush(res, "ModalHeaderBackgroundBrush", ColorFromHex("#130920"));
                break;
        }
    }

    private static void UpdateBrush(ResourceDictionary res, string key, Color color)
    {
        if (res.ContainsKey(key) && res[key] is SolidColorBrush brush)
        {
            brush.Color = color;
        }
        else
        {
            res[key] = new SolidColorBrush(color);
        }
    }

    public static Color ColorFromHex(string hex)
    {
        hex = hex.Replace("#", "");
        byte a = 255;
        byte r = 0, g = 0, b = 0;

        if (hex.Length == 8)
        {
            a = Convert.ToByte(hex.Substring(0, 2), 16);
            r = Convert.ToByte(hex.Substring(2, 2), 16);
            g = Convert.ToByte(hex.Substring(4, 2), 16);
            b = Convert.ToByte(hex.Substring(6, 2), 16);
        }
        else if (hex.Length == 6)
        {
            r = Convert.ToByte(hex.Substring(0, 2), 16);
            g = Convert.ToByte(hex.Substring(2, 2), 16);
            b = Convert.ToByte(hex.Substring(4, 2), 16);
        }

        return Color.FromArgb(a, r, g, b);
    }
}
