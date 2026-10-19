using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace MSGViewer;

public sealed class SettingsWindow : Window
{
    private readonly TextBlock _languageLabel = new() { FontWeight = Avalonia.Media.FontWeight.SemiBold };
    private readonly TextBlock _themeLabel = new() { FontWeight = Avalonia.Media.FontWeight.SemiBold };
    private readonly ComboBox _languageBox = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly ComboBox _themeBox = new() { HorizontalAlignment = HorizontalAlignment.Stretch };
    private readonly Button _closeButton = new() { HorizontalAlignment = HorizontalAlignment.Right, MinWidth = 110 };

    private readonly List<string> _languageCodes = new();
    private readonly string[] _themeCodes = { "auto", "light", "dark" };
    private bool _loading;

    public SettingsWindow()
    {
        Width = 420;
        SizeToContent = SizeToContent.Height;
        CanResize = false;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;

        Content = new StackPanel
        {
            Margin = new Thickness(26, 22),
            Spacing = 8,
            Children =
            {
                _languageLabel,
                _languageBox,
                new Border { Height = 8 },
                _themeLabel,
                _themeBox,
                new Border { Height = 10 },
                _closeButton
            }
        };

        _languageBox.SelectionChanged += (_, _) =>
        {
            if (_loading || _languageBox.SelectedIndex < 0)
                return;

            var code = _languageCodes[_languageBox.SelectedIndex];
            AppSettings.Current.Language = code;
            AppSettings.Save();

            // Se aplica fuera del evento porque Refresh vuelve a armar las listas.
            Avalonia.Threading.Dispatcher.UIThread.Post(() => Loc.Set(code));
        };

        _themeBox.SelectionChanged += (_, _) =>
        {
            if (_loading || _themeBox.SelectedIndex < 0)
                return;

            AppSettings.Current.Theme = _themeCodes[_themeBox.SelectedIndex];
            AppSettings.Save();
            (Application.Current as App)?.ApplyTheme();
        };

        _closeButton.Click += (_, _) => Close();

        Loc.Changed += Refresh;
        Closed += (_, _) => Loc.Changed -= Refresh;

        Refresh();
    }

    private void Refresh()
    {
        _loading = true;

        Title = Loc.T("settingsTitle");
        _languageLabel.Text = Loc.T("language");
        _themeLabel.Text = Loc.T("appearance");
        _closeButton.Content = Loc.T("close");

        _languageCodes.Clear();
        var names = new List<string>();

        _languageCodes.Add("auto");
        names.Add(MacText.Auto);

        foreach (var l in Loc.Languages)
        {
            _languageCodes.Add(l.Code);
            names.Add(l.Name);
        }

        _languageBox.ItemsSource = names;
        var langIndex = _languageCodes.IndexOf(AppSettings.Current.Language);
        _languageBox.SelectedIndex = langIndex < 0 ? 0 : langIndex;

        _themeBox.ItemsSource = new[] { MacText.Auto, Loc.T("themeLight"), Loc.T("themeDark") };
        var themeIndex = Array.IndexOf(_themeCodes, AppSettings.Current.Theme);
        _themeBox.SelectedIndex = themeIndex < 0 ? 0 : themeIndex;

        _loading = false;
    }
}
