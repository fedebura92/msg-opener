using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace MSGViewer;

/// <summary>Avalonia no trae MessageBox; este es un diálogo mínimo.</summary>
public static class Dialogs
{
    public static async Task<bool> ConfirmAsync(
        Window owner, string text, string okText, string? cancelText)
    {
        var tcs = new TaskCompletionSource<bool>();

        var ok = new Button { Content = okText, MinWidth = 90 };
        var buttons = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8
        };

        Button? cancel = null;

        if (cancelText != null)
        {
            cancel = new Button { Content = cancelText, MinWidth = 90 };
            buttons.Children.Add(cancel);
        }

        buttons.Children.Add(ok);

        var dialog = new Window
        {
            Title = "MSG Opener",
            Width = 480,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new Thickness(22),
                Spacing = 18,
                Children =
                {
                    new TextBlock { Text = text, TextWrapping = TextWrapping.Wrap },
                    buttons
                }
            }
        };

        ok.Click += (_, _) => { tcs.TrySetResult(true); dialog.Close(); };

        if (cancel != null)
            cancel.Click += (_, _) => { tcs.TrySetResult(false); dialog.Close(); };

        dialog.Closed += (_, _) => tcs.TrySetResult(false);

        await dialog.ShowDialog(owner);

        return await tcs.Task;
    }
}
