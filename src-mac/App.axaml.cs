using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using Avalonia.Threading;

namespace MSGViewer;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        AppSettings.Load();
        Loc.Set(AppSettings.Current.Language);
        ApplyTheme();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var window = new MainWindow();
            desktop.MainWindow = window;

            // Archivo recibido por línea de comandos.
            var args = desktop.Args ?? Array.Empty<string>();
            var file = args.FirstOrDefault(a =>
                a.EndsWith(".msg", StringComparison.OrdinalIgnoreCase) && File.Exists(a));

            if (file != null)
                window.Opened += (_, _) => window.OpenMsgFile(file);

            // En macOS, el doble clic sobre un .msg (o "Abrir con") llega como
            // un evento de activación de archivo, no como argumento.
            if (TryGetFeature(typeof(IActivatableLifetime)) is IActivatableLifetime activatable)
            {
                activatable.Activated += (_, e) =>
                {
                    if (e is not FileActivatedEventArgs fileArgs)
                        return;

                    foreach (var item in fileArgs.Files)
                    {
                        var path = item.TryGetLocalPath();

                        if (path != null &&
                            path.EndsWith(".msg", StringComparison.OrdinalIgnoreCase))
                        {
                            Dispatcher.UIThread.Post(() => window.OpenMsgFile(path));
                            break;
                        }
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>"auto" sigue el modo claro/oscuro de macOS.</summary>
    public void ApplyTheme()
    {
        RequestedThemeVariant = AppSettings.Current.Theme switch
        {
            "light" => ThemeVariant.Light,
            "dark" => ThemeVariant.Dark,
            _ => ThemeVariant.Default
        };
    }
}
