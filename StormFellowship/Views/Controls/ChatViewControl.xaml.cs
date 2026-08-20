using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using StormFellowship.Models;
using StormFellowship.ViewModels;
using Windows.System;

namespace StormFellowship.Views.Controls;

public sealed partial class ChatViewControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(ChatViewModel), typeof(ChatViewControl), new PropertyMetadata(null));

    public ChatViewModel ViewModel
    {
        get => (ChatViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public event RoutedEventHandler? ToggleMemberListRequested;

    public ChatViewControl()
    {
        InitializeComponent();
    }

    private void OnToggleMemberListClicked(object sender, RoutedEventArgs e)
    {
        ToggleMemberListRequested?.Invoke(this, e);
    }

    private void OnMessageInputKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            ViewModel?.SendMessage();
            e.Handled = true;
        }
    }

    private void OnEmojiItemClicked(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is EmojiItem emoji)
        {
            ViewModel?.InsertEmoji(emoji);
        }
    }

    private void OnStickerItemClicked(object sender, ItemClickEventArgs e)
    {
        if (e.ClickedItem is StickerItem sticker)
        {
            ViewModel?.SendSticker(sticker);
        }
    }

    private void OnReactionButtonClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is MessageReaction rx && btn.DataContext is ChatMessage msg)
        {
            ViewModel?.ToggleReaction(new object[] { msg, rx.EmojiSymbol, rx.EmojiCode });
        }
    }

    private void OnQuickReactLightning(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChatMessage msg)
        {
            ViewModel?.ToggleReaction(new object[] { msg, "⚡", ":storm_bolt:" });
        }
    }

    private void OnQuickReactHeart(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChatMessage msg)
        {
            ViewModel?.ToggleReaction(new object[] { msg, "💖", ":heart:" });
        }
    }

    private void OnReplyClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChatMessage msg)
        {
            ViewModel?.ReplyTo(msg);
            MessageInputBox.Focus(FocusState.Programmatic);
        }
    }

    private void OnPinClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChatMessage msg)
        {
            ViewModel?.PinMessage(msg);
        }
    }

    private void OnDeleteClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ChatMessage msg)
        {
            ViewModel?.DeleteMessage(msg);
        }
    }

    private void OnAttachFileClicked(object sender, RoutedEventArgs e)
    {
        // Add sample sticker/media attachment
        var channelId = ViewModel?.CurrentChannel?.Id ?? "general";
        Services.ChatService.Instance.SendMessage(channelId, "Attached tactical play screenshot:", attachmentUrl: "ms-appx:///Assets/Logo.png");
    }
}
