using System.Globalization;
using System.IO;
using Microsoft.Win32;
using MsgReader.Outlook;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace MSGViewer;

public partial class MainWindow : Window
{
    private readonly List<AttachmentItem> _attachments = new();
    private string? _currentFile;

    // Datos del mensaje abierto, para poder retraducir al cambiar de idioma.
    private bool _hasMessage;
    private string? _subject;
    private Func<CultureInfo, string>? _formatSent;

    public MainWindow()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        InitializeComponent();

        // WebView2 por defecto crea su carpeta de datos junto al .exe, lo que
        // falla si se instala en Program Files. Se usa LocalAppData.
        BodyView.CreationProperties = new Microsoft.Web.WebView2.Wpf.CoreWebView2CreationProperties
        {
            UserDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MSGOpener", "WebView2")
        };

        SourceInitialized += (_, _) => ThemeManager.ApplyTitleBar(this);

        Loc.Changed += ApplyLanguage;
        ApplyLanguage();

        Loaded += async (_, _) =>
        {
            await BodyView.EnsureCoreWebView2Async();

            if (!_hasMessage)
                ShowPlaceholder();

            // Si MSG Opener recibió un archivo .msg como argumento,
            // abrirlo automáticamente.
            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1 && File.Exists(args[1]))
            {
                OpenMsgFile(args[1]);
            }
        };
    }

    private void ApplyLanguage()
    {
        OpenButton.Content = Loc.T("openFile");
        PrintButton.Content = Loc.T("print");
        SettingsButton.Content = "⚙  " + Loc.T("settings");
        SubtitleText.Text = Loc.T("subtitle");
        FromLabel.Text = Loc.T("from");
        ToLabel.Text = Loc.T("to");
        CcLabel.Text = Loc.T("cc");
        FooterText.Text = Loc.T("footer");
        AttachmentHeader.Text = Loc.T("attachments", _attachments.Count);

        // Vuelve a armar la lista para que los botones Abrir/Guardar cambien de idioma.
        AttachmentsList.ItemsSource = null;
        AttachmentsList.ItemsSource = _attachments;

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
            ShowPlaceholder();
        }
    }

    private void ShowPlaceholder()
    {
        ShowHtml(
            "<div style='font-family:Segoe UI;padding:24px;color:#6b7280'>" +
            WebUtility.HtmlEncode(Loc.T("bodyPlaceholder")) +
            "</div>");
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        new SettingsWindow { Owner = this }.ShowDialog();
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = $"{Loc.T("filterMsg")}|*.msg|{Loc.T("filterAll")}|*.*",
            Title = Loc.T("openTitle")
        };

        if (dialog.ShowDialog() == true)
            OpenMsgFile(dialog.FileName);
    }

    public async void OpenMsgFile(string path)
    {
        try
        {
            if (!path.EndsWith(".msg", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(Loc.T("errNotMsg"));

            _currentFile = path;

            using var msg = new MsgReader.Outlook.Storage.Message(path);

            _hasMessage = true;
            _subject = msg.Subject;

            SubjectText.Text = string.IsNullOrWhiteSpace(_subject)
                ? Loc.T("noSubject")
                : _subject;

            FromText.Text = FormatSender(
                msg.Sender?.DisplayName,
                msg.Sender?.Email);

            ToText.Text = EmptyDash(
                msg.GetEmailRecipients(
                    RecipientType.To,
                    false,
                    false));

            CcText.Text = EmptyDash(
                msg.GetEmailRecipients(
                    RecipientType.Cc,
                    false,
                    false));

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
            AttachmentsList.ItemsSource = _attachments;

            AttachmentHeader.Text = Loc.T("attachments", _attachments.Count);

            await BodyView.EnsureCoreWebView2Async();

            if (!string.IsNullOrWhiteSpace(msg.BodyHtml))
            {
                string html = msg.BodyHtml;

                // Resolver imágenes incrustadas cid: usando los adjuntos
                html = ResolveEmbeddedImages(html, msg);

                ShowHtml(html);
            }
            else
            {
                ShowHtml(
                    $"<pre style='white-space:pre-wrap;font-family:Segoe UI;font-size:14px;padding:20px'>" +
                    $"{WebUtility.HtmlEncode(msg.BodyText ?? "")}" +
                    "</pre>");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"{Loc.T("errOpen")}\n\n{ex.Message}",
                "MSG Opener",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private static string ResolveEmbeddedImages(
        string html,
        MsgReader.Outlook.Storage.Message msg)
    {
        if (string.IsNullOrWhiteSpace(html))
            return html;

        foreach (var item in msg.Attachments)
        {
            if (item is not MsgReader.Outlook.Storage.Attachment att)
                continue;

            if (att.Data is not { Length: > 0 })
                continue;

            if (string.IsNullOrWhiteSpace(att.ContentId))
                continue;

            string contentId = att.ContentId.Trim();

            // Algunos mensajes incluyen < > alrededor del Content-ID.
            contentId = contentId.Trim('<', '>');

            string mimeType = GetMimeType(att.FileName);

            string dataUrl =
                $"data:{mimeType};base64,{Convert.ToBase64String(att.Data)}";

            // Busca:
            // cid:xxxxx
            // cid:<xxxxx>
            // ignorando mayúsculas/minúsculas.
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

        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        return extension switch
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

    private void ShowHtml(string html)
    {
        if (BodyView.CoreWebView2 == null)
            return;

        var hardened =
            "<!DOCTYPE html>" +
            "<html>" +
            "<head>" +
            "<meta charset='utf-8'>" +
            "<meta name='color-scheme' content='light'>" +
            "<meta http-equiv='Content-Security-Policy' " +
            "content=\"default-src 'none'; img-src data: cid:; style-src 'unsafe-inline'; font-src data:;\">" +

            "<style>" +

            "html{" +
            "background:#ffffff !important;" +
            "color:#1f2937 !important;" +
            "color-scheme:light !important;" +
            "}" +

            "body{" +
            "background:#ffffff !important;" +
            "color:#1f2937 !important;" +
            "font-family:'Segoe UI',Arial,sans-serif;" +
            "font-size:14px;" +
            "margin:20px;" +
            "padding:0;" +
            "overflow-wrap:anywhere;" +
            "color-scheme:light !important;" +
            "}" +

            "body *{" +
            "color-scheme:light;" +
            "}" +

            "img{" +
            "max-width:100%;" +
            "height:auto;" +
            "}" +

            "table{" +
            "max-width:100%;" +
            "}" +

            "</style>" +
            "</head>" +
            "<body>" +
            html +
            "</body>" +
            "</html>";

        BodyView.NavigateToString(hardened);
    }

    private static string EmptyDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "—" : value;

    private static string FormatSender(string? name, string? email) =>
        string.IsNullOrWhiteSpace(name)
            ? EmptyDash(email)
            : string.IsNullOrWhiteSpace(email)
                ? name
                : $"{name} <{email}>";

    private void SaveAttachment_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not AttachmentItem item)
            return;

        var dlg = new SaveFileDialog
        {
            FileName = item.FileName,
            Filter = $"{Loc.T("filterAll")}|*.*"
        };

        if (dlg.ShowDialog() == true)
            File.WriteAllBytes(dlg.FileName, item.Data);
    }

    private void OpenAttachment_Click(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.Tag is not AttachmentItem item)
            return;

        var safeName = Path.GetFileName(item.FileName);

        string[] risky =
        {
            ".exe", ".msi", ".bat", ".cmd", ".com", ".scr", ".pif", ".ps1", ".psm1",
            ".vbs", ".vbe", ".js", ".jse", ".wsf", ".wsh", ".hta", ".jar", ".lnk",
            ".reg", ".cpl", ".dll", ".msc", ".iso", ".img"
        };

        if (risky.Contains(Path.GetExtension(safeName).ToLowerInvariant()))
        {
            var answer = MessageBox.Show(
                Loc.T("riskyWarn", safeName),
                "MSG Opener",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning,
                MessageBoxResult.No);

            if (answer != MessageBoxResult.Yes)
                return;
        }

        var folder = Path.Combine(
            Path.GetTempPath(),
            "MSGOpener",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(folder);

        var file = Path.Combine(folder, safeName);

        File.WriteAllBytes(file, item.Data);

        Process.Start(
            new ProcessStartInfo(file)
            {
                UseShellExecute = true
            });
    }

    private void Print_Click(object sender, RoutedEventArgs e)
    {
        if (BodyView.CoreWebView2 != null)
        {
            BodyView.CoreWebView2.ShowPrintUI(
                Microsoft.Web.WebView2.Core.CoreWebView2PrintDialogKind.Browser);
        }
    }

    private void Window_DragOver(object sender, DragEventArgs e)
    {
        e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
            ? DragDropEffects.Copy
            : DragDropEffects.None;
    }

    private void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetData(DataFormats.FileDrop) is string[] files &&
            files.Length > 0)
        {
            OpenMsgFile(files[0]);
        }
    }
}