using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Win32;

namespace MSGViewer;

public static class ThemeManager
{
    private static readonly Dictionary<string, string> Light = new()
    {
        ["WindowBg"]          = "#F3F4F6",
        ["Surface"]           = "#FFFFFF",
        ["TextPrimary"]       = "#202124",
        ["TextSecondary"]     = "#6B7280",
        ["BorderLight"]       = "#E5E7EB",
        ["Accent"]            = "#2563EB",
        ["AccentHover"]       = "#1D4ED8",
        ["AccentSoft"]        = "#EFF6FF",
        ["ButtonBg"]          = "#FFFFFF",
        ["ButtonFg"]          = "#374151",
        ["ButtonBorder"]      = "#D1D5DB",
        ["ButtonHoverBg"]     = "#F9FAFB",
        ["ButtonHoverBorder"] = "#9CA3AF",
        ["ButtonPressedBg"]   = "#F3F4F6",
        ["ItemBg"]            = "#F9FAFB",
        ["ItemBorder"]        = "#E5E7EB",
    };

    private static readonly Dictionary<string, string> Dark = new()
    {
        ["WindowBg"]          = "#0A0A0A",
        ["Surface"]           = "#161618",
        ["TextPrimary"]       = "#F3F4F6",
        ["TextSecondary"]     = "#9CA3AF",
        ["BorderLight"]       = "#2A2A2E",
        ["Accent"]            = "#3B82F6",
        ["AccentHover"]       = "#60A5FA",
        ["AccentSoft"]        = "#1E2A44",
        ["ButtonBg"]          = "#1F1F23",
        ["ButtonFg"]          = "#E5E7EB",
        ["ButtonBorder"]      = "#3A3A40",
        ["ButtonHoverBg"]     = "#2A2A30",
        ["ButtonHoverBorder"] = "#6B7280",
        ["ButtonPressedBg"]   = "#151518",
        ["ItemBg"]            = "#1C1C1F",
        ["ItemBorder"]        = "#2E2E33",
    };

    public static bool IsDark { get; private set; }

    public static void Initialize()
    {
        // Si el modo es "automático", sigue los cambios de Windows en vivo.
        SystemEvents.UserPreferenceChanged += (_, _) =>
        {
            if (AppSettings.Current.Theme == "auto")
                Application.Current?.Dispatcher.BeginInvoke(new Action(Apply));
        };

        Apply();
    }

    public static void Apply()
    {
        string mode = AppSettings.Current.Theme;
        IsDark = mode == "dark" || (mode == "auto" && SystemUsesDarkTheme());

        var palette = IsDark ? Dark : Light;
        var resources = Application.Current.Resources;

        foreach (var kv in palette)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(kv.Value));
            brush.Freeze();
            resources[kv.Key] = brush;
        }

        foreach (Window w in Application.Current.Windows)
            ApplyTitleBar(w);
    }

    private static bool SystemUsesDarkTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            return key?.GetValue("AppsUseLightTheme") is int v && v == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>Pone la barra de título de Windows en oscuro o claro.</summary>
    public static void ApplyTitleBar(Window window)
    {
        IntPtr handle = new WindowInteropHelper(window).Handle;
        if (handle == IntPtr.Zero)
            return;

        int value = IsDark ? 1 : 0;

        // 20 = Windows 10 20H1+ y Windows 11; 19 = versiones anteriores de Windows 10.
        if (DwmSetWindowAttribute(handle, 20, ref value, sizeof(int)) != 0)
            DwmSetWindowAttribute(handle, 19, ref value, sizeof(int));
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);
}
