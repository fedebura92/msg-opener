using System.Globalization;

namespace MSGViewer;

public sealed record OptionItem(string Code, string Name);

public static partial class Loc
{
    /// <summary>Idiomas disponibles: código, nombre nativo, cultura para fechas.</summary>
    public static readonly (string Code, string Name, string Culture)[] Languages =
    {
        ("es", "Español",      "es-AR"),
        ("en", "English",      "en-US"),
        ("de", "Deutsch",      "de-DE"),
        ("fr", "Français",     "fr-FR"),
        ("pt", "Português",    "pt-BR"),
        ("zh", "中文（简体）",   "zh-CN"),
        ("ja", "日本語",        "ja-JP"),
        ("ko", "한국어",        "ko-KR"),
        ("pl", "Polski",       "pl-PL"),
        ("ru", "Русский",      "ru-RU"),
        ("nl", "Nederlands",   "nl-NL"),
        ("hi", "हिन्दी",         "hi-IN"),
    };

    public static event Action? Changed;

    public static string Current { get; private set; } = "es";
    public static CultureInfo Culture { get; private set; } = CultureInfo.CurrentCulture;

    /// <summary>Acepta "auto" (idioma de Windows) o un código de idioma.</summary>
    public static void Set(string setting)
    {
        string code = setting == "auto" ? Detect() : setting;

        if (!Languages.Any(l => l.Code == code))
            code = "en";

        Current = code;

        try
        {
            Culture = CultureInfo.GetCultureInfo(Languages.First(l => l.Code == code).Culture);
        }
        catch
        {
            Culture = CultureInfo.CurrentCulture;
        }

        Changed?.Invoke();
    }

    private static string Detect()
    {
        string two = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        return Languages.Any(l => l.Code == two) ? two : "en";
    }

    public static string T(string key)
    {
        if (Strings.TryGetValue(Current, out var d) && d.TryGetValue(key, out var v))
            return v;

        if (Strings["en"].TryGetValue(key, out var e))
            return e;

        return key;
    }

    public static string T(string key, params object[] args) =>
        string.Format(Culture, T(key), args);
}
