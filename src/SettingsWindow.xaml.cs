using System.Windows;
using System.Windows.Controls;

namespace MSGViewer;

public partial class SettingsWindow : Window
{
    private bool _loading;

    public SettingsWindow()
    {
        InitializeComponent();

        SourceInitialized += (_, _) => ThemeManager.ApplyTitleBar(this);

        Loc.Changed += Refresh;
        Closed += (_, _) => Loc.Changed -= Refresh;

        Refresh();
    }

    private void Refresh()
    {
        _loading = true;

        Title = Loc.T("settingsTitle");
        HeadingText.Text = Loc.T("settingsTitle");
        LanguageLabel.Text = Loc.T("language");
        ThemeLabel.Text = Loc.T("appearance");
        CloseButton.Content = Loc.T("close");

        var languages = new List<OptionItem> { new("auto", Loc.T("langAuto")) };
        languages.AddRange(Loc.Languages.Select(l => new OptionItem(l.Code, l.Name)));

        LangList.ItemsSource = languages;
        LangList.SelectedItem =
            languages.FirstOrDefault(o => o.Code == AppSettings.Current.Language) ?? languages[0];

        var themes = new List<OptionItem>
        {
            new("auto",  Loc.T("themeAuto")),
            new("light", Loc.T("themeLight")),
            new("dark",  Loc.T("themeDark")),
        };

        ThemeList.ItemsSource = themes;
        ThemeList.SelectedItem =
            themes.FirstOrDefault(o => o.Code == AppSettings.Current.Theme) ?? themes[0];

        _loading = false;
    }

    private void LangList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading || LangList.SelectedItem is not OptionItem option)
            return;

        AppSettings.Current.Language = option.Code;
        AppSettings.Save();

        // Se aplica fuera del evento porque Refresh vuelve a armar las listas.
        Dispatcher.BeginInvoke(new Action(() => Loc.Set(option.Code)));
    }

    private void ThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_loading || ThemeList.SelectedItem is not OptionItem option)
            return;

        AppSettings.Current.Theme = option.Code;
        AppSettings.Save();

        ThemeManager.Apply();
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
