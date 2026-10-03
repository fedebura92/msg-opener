using System.Windows;
namespace MSGViewer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppSettings.Load();
        ThemeManager.Initialize();
        Loc.Set(AppSettings.Current.Language);

        // El archivo .msg recibido por línea de comandos (doble clic) lo abre
        // MainWindow una vez que WebView2 está listo.
        new MainWindow().Show();
    }
}
