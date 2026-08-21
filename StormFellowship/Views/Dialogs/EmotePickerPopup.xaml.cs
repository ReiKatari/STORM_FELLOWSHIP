using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace StormFellowship.Views.Dialogs;

public partial class EmotePickerPopup : UserControl
{
    public event Action<string>? EmoteSelected;
    public event Action? Closed;

    // Sticker data: (label, background gradient start, background gradient end, glow color)
    private static readonly (string Label, string Bg1, string Bg2, string Glow)[] Stickers = new[]
    {
        ("⚡ STORM BOOST",  "#00D2FF", "#0284C7", "#00D2FF"),
        ("🛡️ DEFENDER",     "#34D399", "#059669", "#34D399"),
        ("👑 CHAMPION",     "#FBBF24", "#D97706", "#FBBF24"),
        ("🔥 ON FIRE",      "#FB7185", "#E11D48", "#FB7185"),
        ("🎯 HEADSHOT",     "#EF4444", "#B91C1C", "#EF4444"),
        ("🚀 TO THE MOON",  "#A855F7", "#7E22CE", "#A855F7"),
        ("⚔️ GG WP",        "#38BDF8", "#1D4ED8", "#38BDF8"),
        ("👾 CYBER STORM",  "#C084FC", "#7E22CE", "#C084FC"),
        ("💎 DIAMOND",      "#67E8F9", "#06B6D4", "#67E8F9"),
        ("🏆 VICTORY",      "#FCD34D", "#F59E0B", "#FCD34D"),
        ("🎧 PRO GAMER",    "#818CF8", "#6366F1", "#818CF8"),
        ("🌟 LEGENDARY",    "#F472B6", "#EC4899", "#F472B6"),
    };

    private static readonly string[] Emotes = new[]
    {
        "😀", "😎", "🔥", "🚀", "⚡", "✨", "🎉", "❤️",
        "👍", "👏", "👑", "💪", "🧠", "🎯", "💎", "🎮",
        "🍕", "☕", "🛡️", "⚔️", "🏆", "🌟", "👾", "🤖",
        "😂", "🥳", "😡", "💀", "🫡", "🤝", "💯", "🔔"
    };

    public EmotePickerPopup()
    {
        InitializeComponent();
        PopulateItems(string.Empty);
    }

    private void PopulateItems(string query)
    {
        StickerWrapPanel.Children.Clear();
        EmoteWrapPanel.Children.Clear();

        foreach (var (label, bg1, bg2, glow) in Stickers)
        {
            if (!string.IsNullOrEmpty(query) && !label.Contains(query, StringComparison.OrdinalIgnoreCase))
                continue;

            var gradBrush = new LinearGradientBrush(
                (Color)ColorConverter.ConvertFromString(bg1),
                (Color)ColorConverter.ConvertFromString(bg2),
                45.0);

            var glowColor = (Color)ColorConverter.ConvertFromString(glow);

            var textBlock = new TextBlock
            {
                Text = label,
                FontSize = 10,
                FontWeight = FontWeights.ExtraBold,
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Segoe UI Emoji, Segoe UI, sans-serif")
            };

            var border = new Border
            {
                Background = gradBrush,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(10, 5, 10, 5),
                Margin = new Thickness(0, 0, 4, 4),
                Cursor = System.Windows.Input.Cursors.Hand,
                Effect = new DropShadowEffect
                {
                    Color = glowColor,
                    BlurRadius = 10,
                    ShadowDepth = 0,
                    Opacity = 0.5
                },
                Child = textBlock
            };

            border.MouseLeftButtonUp += (s, e) =>
            {
                EmoteSelected?.Invoke(label);
            };

            StickerWrapPanel.Children.Add(border);
        }

        foreach (var emote in Emotes)
        {
            if (!string.IsNullOrEmpty(query) && !emote.Contains(query, StringComparison.OrdinalIgnoreCase))
                continue;

            var textBlock = new TextBlock
            {
                Text = emote,
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Segoe UI Emoji")
            };

            var border = new Border
            {
                Width = 38,
                Height = 38,
                CornerRadius = new CornerRadius(8),
                Background = Brushes.Transparent,
                Margin = new Thickness(1),
                Cursor = System.Windows.Input.Cursors.Hand,
                Child = textBlock,
                ToolTip = emote
            };

            border.MouseEnter += (s, e) =>
            {
                if (s is Border b) b.Background = (Brush)FindResource("CardHoverBrush");
            };
            border.MouseLeave += (s, e) =>
            {
                if (s is Border b) b.Background = Brushes.Transparent;
            };

            border.MouseLeftButtonUp += (s, e) =>
            {
                EmoteSelected?.Invoke(emote);
            };

            EmoteWrapPanel.Children.Add(border);
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        PopulateItems(SearchBox.Text);
    }

    private void OnCloseClicked(object sender, RoutedEventArgs e)
    {
        Closed?.Invoke();
    }
}
