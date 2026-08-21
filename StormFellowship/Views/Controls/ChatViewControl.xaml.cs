using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormFellowship.Models;
using StormFellowship.ViewModels;

namespace StormFellowship.Views.Controls;

public partial class ChatViewControl : UserControl
{
    public ChatViewControl()
    {
        InitializeComponent();
    }

    private void OnMessageInputKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is ChatViewModel vm)
            {
                vm.SendMessage();
            }
            e.Handled = true;
        }
    }

    private void OnEmojiButtonClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
        {
            vm.InsertEmoji(new EmojiItem { Symbol = "⚡", Code = ":storm_bolt:" });
        }
    }

    private void OnStickerButtonClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
        {
            vm.SendSticker(new StickerItem { Name = "STORM GG" });
        }
    }

    private void OnVoiceNoteButtonClicked(object sender, RoutedEventArgs e)
    {
        var channelId = (DataContext as ChatViewModel)?.CurrentChannel?.Id ?? "general";
        Services.ChatService.Instance.SendVoiceNote(channelId, 12);
    }

    private void OnAttachButtonClicked(object sender, RoutedEventArgs e)
    {
        if (DataContext is ChatViewModel vm)
        {
            vm.AttachFile();
        }
    }

    private void OnChatDragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private void OnChatDrop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0 && DataContext is ChatViewModel vm)
            {
                foreach (var file in files)
                {
                    vm.ProcessFileAttachment(file);
                }
            }
            e.Handled = true;
        }
    }

    private void OnReactionBadgeClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is MessageReaction rx && btn.DataContext is ChatMessage msg)
        {
            if (DataContext is ChatViewModel vm)
            {
                vm.ToggleReaction(new object[] { msg, rx.EmojiSymbol, rx.EmojiCode });
            }
        }
    }
}
