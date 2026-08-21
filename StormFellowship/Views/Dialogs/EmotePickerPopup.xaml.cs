using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StormFellowship.Views.Dialogs;

public partial class EmotePickerPopup : UserControl
{
    public event Action<string>? EmoteSelected;
    public event Action? Closed;

    private static readonly string[] Stickers = new[]
    {
        "⚡ STORM BOOST", "🛡️ DEFENDER", "👑 CHAMPION", "🔥 ON FIRE",
        "🎯 HEADSHOT", "🚀 TO THE MOON", "⚔️ GG WP", "👾 CYBER STORM",
        "💎 DIAMOND", "🏆 VICTORY", "🎧 PRO GAMER", "🌟 LEGENDARY"
    };

    private static readonly string[] Emotes = new[]
    {
        "😀", "😎", "🔥", "🚀", "⚡", "✨", "🎉", "❤️",
        "👍", "👏", "👑", "💪", "🧠", "🎯", "💎", "🎮",
        "🍕", "☕", "🛡️", "⚔️", "🏆", "🌟", "👾", "🤖"
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

        foreach (var sticker in Stickers)
        {
            if (!string.IsNullOrEmpty(query) && !sticker.Contains(query, StringComparison.OrdinalIgnoreCase))
                continue;

            var btn = new Button
            {
                Content = sticker,
                Margin = new Thickness(0, 0, 4, 4),
                Padding = new Thickness(8, 4, 8, 4),
                FontSize = 10,
                FontWeight = FontWeights.Bold,
                Background = (Brush)FindResource("CardHoverBrush"),
                Foreground = (Brush)FindResource("AccentBrush"),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = sticker
            };
            btn.Click += (s, e) =>
            {
                EmoteSelected?.Invoke(sticker);
            };
            StickerWrapPanel.Children.Add(btn);
        }

        foreach (var emote in Emotes)
        {
            var btn = new Button
            {
                Content = emote,
                Width = 32,
                Height = 32,
                Margin = new Thickness(0, 0, 4, 4),
                FontSize = 14,
                Background = (Brush)FindResource("InputBackgroundBrush"),
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = emote
            };
            btn.Click += (s, e) =>
            {
                EmoteSelected?.Invoke(emote);
            };
            EmoteWrapPanel.Children.Add(btn);
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
