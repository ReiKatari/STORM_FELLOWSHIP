using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using StormFellowship.Models;
using StormFellowship.Services;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Dialogs;

public partial class QuickSwitcherDialog : UserControl
{
    public QuickSwitcherDialog()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {
            SearchInput.Focus();
            PopulateResults(string.Empty);
        };
    }

    public void RefreshAndFocus()
    {
        SearchInput.Text = string.Empty;
        SearchInput.Focus();
        PopulateResults(string.Empty);
    }

    private void PopulateResults(string query)
    {
        ResultsPanel.Children.Clear();

        // 1. Text & Voice Channels in active fellowship
        var fellowship = FellowshipService.Instance.CurrentFellowship;
        if (fellowship != null)
        {
            foreach (var cat in fellowship.Categories)
            {
                foreach (var ch in cat.Channels)
                {
                    if (!string.IsNullOrEmpty(query) && !ch.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                        continue;

                    var btn = CreateResultButton(ch.IconGlyph, ch.Name, $"{fellowship.Name} • {cat.Name}", () =>
                    {
                        if (ch.IsVoice) CallService.Instance.JoinVoiceChannel(ch);
                        else FellowshipService.Instance.SelectChannel(ch);
                    });
                    ResultsPanel.Children.Add(btn);
                }
            }
        }

        // 2. Direct Messages
        foreach (var dm in FellowshipService.Instance.DirectMessageUsers)
        {
            if (!string.IsNullOrEmpty(query) && !dm.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase))
                continue;

            var btn = CreateResultButton(dm.AvatarGlyph, dm.DisplayName, $"Личные сообщения • {dm.CustomStatus}", () =>
            {
                FellowshipService.Instance.SelectDirectMessage(dm);
            });
            ResultsPanel.Children.Add(btn);
        }

        // 3. Quick System Actions
        if (string.IsNullOrEmpty(query) || "настройки".Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            var btn = CreateResultButton("⚙️", "Настройки пользователя", "Голос, видео, темы, оформление", () =>
            {
                if (DataContext is MainViewModel vm) vm.OpenUserSettingsDialog();
            });
            ResultsPanel.Children.Add(btn);
        }

        if (string.IsNullOrEmpty(query) || "саундборд".Contains(query, StringComparison.OrdinalIgnoreCase))
        {
            var btn = CreateResultButton("🎵", "Студийный Саундборд", "Звуковые эффекты для созвонов", () =>
            {
                if (DataContext is MainViewModel vm) vm.OpenSoundboard();
            });
            ResultsPanel.Children.Add(btn);
        }
    }

    private Button CreateResultButton(string icon, string title, string sub, Action onSelect)
    {
        var btn = new Button
        {
            Margin = new Thickness(0, 2, 0, 2),
            Padding = new Thickness(10, 8, 10, 8),
            Background = (Brush)FindResource("CardBackgroundBrush"),
            BorderBrush = (Brush)FindResource("BorderSubtleBrush"),
            BorderThickness = new Thickness(1),
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            Cursor = Cursors.Hand
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var iconBlock = new TextBlock
        {
            Text = icon,
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };
        Grid.SetColumn(iconBlock, 0);
        grid.Children.Add(iconBlock);

        var textStack = new StackPanel { VerticalAlignment = VerticalAlignment.Center };
        var titleBlock = new TextBlock
        {
            Text = title,
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            Foreground = (Brush)FindResource("TextPrimaryBrush")
        };
        var subBlock = new TextBlock
        {
            Text = sub,
            FontSize = 10,
            Foreground = (Brush)FindResource("TextMutedBrush")
        };
        textStack.Children.Add(titleBlock);
        textStack.Children.Add(subBlock);
        Grid.SetColumn(textStack, 1);
        grid.Children.Add(textStack);

        btn.Content = grid;
        btn.Click += (s, e) =>
        {
            onSelect();
            if (DataContext is MainViewModel vm) vm.CloseQuickSwitcherDialog();
        };

        return btn;
    }

    private void OnSearchInputTextChanged(object sender, TextChangedEventArgs e)
    {
        PopulateResults(SearchInput.Text);
    }

    private void OnSearchInputKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            if (DataContext is MainViewModel vm) vm.CloseQuickSwitcherDialog();
        }
    }

    private void OnBackdropMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is MainViewModel vm) vm.CloseQuickSwitcherDialog();
    }

    private void OnDialogCardMouseDown(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
