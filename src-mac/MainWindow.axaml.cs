using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsgReader.Outlook;

namespace MSGViewer;

public partial class MainWindow : Window
{
    private readonly List<AttachmentItem> _attachments = new();

    // Datos del mensaje abierto, para poder retraducir al cambiar de idioma.
    private bool _hasMessage;
    private string? _subject;
    private Func<CultureInfo, string>? _formatSent;
    private string? _formattedHtml;

    public MainWindow()
    {
        InitializeComponent();

        Loc.Changed += ApplyLanguage;
        Closed += (_, _) => Loc.Changed -= ApplyLanguage;

        // Botones de adjuntos (se atienden acá porque están dentro de una plantilla).
        AttachmentsList.AddHandler(Button.ClickEvent, Attachment_Click);

        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DragOverEvent, Window_DragOver);
        AddHandler(DragDrop.DropEvent, Window_Drop);

        ApplyLanguage();
    }

    // ------------------------------------------------------------------ idioma

    private void ApplyLanguage()
    {
        OpenButton.Content = Loc.T("openFile");
        SettingsLabel.Text = Loc.T("settings");
        SubtitleText.Text = Loc.T("subtitle");
        FromLabel.Text = Loc.T("from");
        ToLabel.Text = Loc.T("to");
        CcLabel.Text = Loc.T("cc");
        FooterText.Text = Loc.T("footer");
        FormattedButton.Content = MacText.ViewFormatted;
        AttachmentHeader.Text = Loc.T("attachments", _attachments.Count);

        // Vuelve a armar la lista para que los botones Abrir/Guardar cambien de idioma.
        AttachmentsList.ItemsSource = null;
        AttachmentsList.ItemsSource = _attachments.ToList();

        if (_hasMessage)
        {
            SubjectText.Text = string.IsNullOrWhiteSpace(_subject) ? Loc.T("noSubject") : _subject;
            Title = $"{SubjectText.Text} - MSG Opener";

            if (_formatSent != null)
                DateText.Text = _formatSent(Loc.Culture);
        }
        else
        {
            SubjectText.Text = Loc.T("dropHint");
            BodyText.Text = Loc.T("bodyPlaceholder");
        }
    }

    // ------------------------------------------------------------ abrir archivo

    private async void Open_Click(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = Loc.T("openTitle"),
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType(Loc.T("filterMsg")) { Patterns = new[] { "*.msg" } },
                FilePickerFileTypes.All
            }
        });

        var path = files.Count > 0 ? files[0].TryGetLocalPath() : null;

        if (path != null)
            OpenMsgFile(path);
    }

    private async void Settings_Click(object? sender, RoutedEventArgs e)
    {
        await new SettingsWindow().ShowDialog(this);
    }

    private void Window_DragOver(object? sender, DragEventArgs e)
    {
#pragma warning disable CS0618
        e.DragEffects = e.Data.Contains(DataFormats.Files) ? DragDropEffects.Copy : DragDropEffects.None;
#pragma warning restore CS0618
    }

    private void Window_Drop(object? sender, DragEventArgs e)
    {
#pragma warning disable CS0618
        var files = e.Data.GetFiles();
#pragma warning restore CS0618

        var path = files?.Select(f => f.TryGetLocalPath()).FirstOrDefault(p => p != null);

        if (path != null)
            OpenMsgFile(path);
    }

    public async void OpenMsgFile(string path)
    {
        try
        {
            if (!path.EndsWith(".msg", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Loc.T("errNotMsg"));

            using var msg = new MsgReader.Outlook.Storage.Message(path);

            _hasMessage = true;
            _subject = msg.Subject;

            SubjectText.Text = string.IsNullOrWhiteSpace(_subject)
                ? Loc.T("noSubject")
                : _subject;

            FromText.Text = FormatSender(msg.Sender?.DisplayName, msg.Sender?.Email);
            ToText.Text = EmptyDash(msg.GetEmailRecipients(RecipientType.To, false, false));
            CcText.Text = EmptyDash(msg.GetEmailRecipients(RecipientType.Cc, false, false));

            var sentOn = msg.SentOn;
            _formatSent = culture => sentOn?.ToString("g", culture) ?? "—";
            DateText.Text = _formatSent(Loc.Culture);

            Title = $"{SubjectText.Text} - MSG Opener";

            _attachments.Clear();

            foreach (var item in msg.Attachments)
            {
                if (item is MsgReader.Outlook.Storage.Attachment att &&
                    att.Data is { Length: > 0 })
                {
                    _attachments.Add(new AttachmentItem
                    {
                        FileName = string.IsNullOrWhiteSpace(att.FileName)
                            ? Loc.T("attachmentDefault")
                            : att.FileName,
                        Data = att.Data
                    });
                }
            }

            AttachmentsList.ItemsSource = null;
            AttachmentsList.ItemsSource = _attachments.ToList();
            AttachmentHeader.Text = Loc.T("attachments", _attachments.Count);

            // Cuerpo: texto plano en pantalla; la versión con formato se abre en el navegador.
            string? html = msg.BodyHtml;
            _formattedHtml = string.IsNullOrWhiteSpace(html)
                ? null
                : ResolveEmbeddedImages(html, msg);

            FormattedButton.IsEnabled = _formattedHtml != null;

            string text = msg.BodyText ?? "";

            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(html))
                text = HtmlToText(html);

            BodyText.Text = text.Trim();
        }
        catch (Exception ex)
        {
            await Dialogs.ConfirmAsync(this, $"{Loc.T("errOpen")}\n\n{ex.Message}", Loc.T("close"), null);
        }
    }

    // ---------------------------------------------------------------- contenido

    private static string HtmlToText(string html)
    {
        html = Regex.Replace(html, @"<(script|style)[^>]*>.*?</\1>", "",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        html = Regex.Replace(html, @"<br\s*/?>|</p>|</div>|</tr>|</h[1-6]>", "\n",
            RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<[^>]+>", "");
        html = WebUtility.HtmlDecode(html);
        html = Regex.Replace(html, @"[ \t]+\n", "\n");
        html = Regex.Replace(html, @"\n{3,}", "\n\n");
        return html;
    }

    private static string ResolveEmbeddedImages(string html, MsgReader.Outlook.Storage.Message msg)
    {
        foreach (var item in msg.Attachments)
        {
            if (item is not MsgReader.Outlook.Storage.Attachment att)
                continue;

            if (att.Data is not { Length: > 0 } || string.IsNullOrWhiteSpace(att.ContentId))
                continue;

            string contentId = att.ContentId.Trim().Trim('<', '>');
            string dataUrl = $"data:{GetMimeType(att.FileName)};base64,{Convert.ToBase64String(att.Data)}";

            html = Regex.Replace(
                html,
                $@"cid:\s*<?{Regex.Escape(contentId)}>?",
                dataUrl,
                RegexOptions.IgnoreCase);
        }

        return html;
    }

    private static string GetMimeType(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return "application/octet-stream";

        return Path.GetExtension(fileName).ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            ".tif" or ".tiff" => "image/tiff",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
    }

    private static string BuildHardenedHtml(string html) =>
        "<!DOCTYPE html><html><head><meta charset='utf-8'>" +
        "<meta name='color-scheme' content='light'>" +
        // Sin scripts, sin recursos remotos: solo imágenes incrustadas.
        "<meta http-equiv='Content-Security-Policy' " +
        "content=\"default-src 'none'; img-src data:; style-src 'unsafe-inline'; font-src data:;\">" +
        "<style>" +
        "html,body{background:#ffffff !important;color:#1f2937 !important;color-scheme:light !important;}" +
        "body{font-family:-apple-system,'Helvetica Neue',Arial,sans-serif;font-size:14px;" +
        "margin:20px;overflow-wrap:anywhere;}" +
        "img{max-width:100%;height:auto;}table{max-width:100%;}" +
        "</style></head><body>" + html + "</body></html>";

    private static string EmptyDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value;

    private static string FormatSender(string? name, string? email) =>
        string.IsNullOrWhiteSpace(name)
            ? EmptyDash(email)
            : string.IsNullOrWhiteSpace(email)
                ? name
                : $"{name} <{email}>";

    // ------------------------------------------------------------ ver con formato

    private void Formatted_Click(object? sender, RoutedEventArgs e)
    {
        if (_formattedHtml == null)
            return;

        try
        {
            var folder = Path.Combine(Path.GetTempPath(), "MSGOpener", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, "message.html");
            File.WriteAllText(file, BuildHardenedHtml(_formattedHtml), new UTF8Encoding(false));

            OpenWithSystem(file);
        }
        catch (Exception ex)
        {
            _ = Dialogs.ConfirmAsync(this, $"{Loc.T("errOpen")}\n\n{ex.Message}", Loc.T("close"), null);
        }
    }

    // ------------------------------------------------------------------ adjuntos

    private async void Attachment_Click(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not Button button || button.Tag is not AttachmentItem item)
            return;

        if (button.Classes.Contains("attOpen"))
            await OpenAttachmentAsync(item);
        else if (button.Classes.Contains("attSave"))
            await SaveAttachmentAsync(item);
    }

    private async Task SaveAttachmentAsync(AttachmentItem item)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = Loc.T("save"),
            SuggestedFileName = Path.GetFileName(item.FileName)
        });

        var path = file?.TryGetLocalPath();

        if (path != null)
            await File.WriteAllBytesAsync(path, item.Data);
    }

    private async Task OpenAttachmentAsync(AttachmentItem item)
    {
        var safeName = Path.GetFileName(item.FileName);

        string[] risky =
        {
            // Windows
            ".exe", ".msi", ".bat", ".cmd", ".com", ".scr", ".pif", ".ps1", ".psm1",
            ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".hta", ".lnk", ".reg",
            ".cpl", ".dll", ".msc",
            // macOS / Unix
            ".app", ".command", ".sh", ".tool", ".pkg", ".dmg", ".scpt", ".scptd",
            ".workflow", ".terminal", ".action", ".pl", ".py", ".rb",
            // Genéricos
            ".jar", ".iso", ".img"
        };

        if (risky.Contains(Path.GetExtension(safeName).ToLowerInvariant()))
        {
            bool proceed = await Dialogs.ConfirmAsync(
                this, Loc.T("riskyWarn", safeName), Loc.T("open"), Loc.T("close"));

            if (!proceed)
                return;
        }

        try
        {
            var folder = Path.Combine(Path.GetTempPath(), "MSGOpener", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, safeName);
            await File.WriteAllBytesAsync(file, item.Data);

            OpenWithSystem(file);
        }
        catch (Exception ex)
        {
            await Dialogs.ConfirmAsync(this, $"{Loc.T("errOpen")}\n\n{ex.Message}", Loc.T("close"), null);
        }
    }

    private static void OpenWithSystem(string file)
    {
        if (OperatingSystem.IsMacOS())
        {
            var psi = new ProcessStartInfo("open") { UseShellExecute = false };
            psi.ArgumentList.Add(file);
            Process.Start(psi);
        }
        else if (OperatingSystem.IsWindows())
        {
            Process.Start(new ProcessStartInfo(file) { UseShellExecute = true });
        }
        else
        {
            var psi = new ProcessStartInfo("xdg-open") { UseShellExecute = false };
            psi.ArgumentList.Add(file);
            Process.Start(psi);
        }
    }
}
