using System.IO;
using System.Text.Json;

namespace MSGViewer;

public sealed class AppSettings
{
    /// <summary>"auto" o un código de idioma (es, en, de, fr, pt, zh, ja, ko, pl, ru, nl, hi).</summary>
    public string Language { get; set; } = "auto";

    /// <summary>"auto" (según Windows), "light" o "dark".</summary>
    public string Theme { get; set; } = "auto";

    public static AppSettings Current { get; private set; } = new();

    private static string FilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MSGOpener", "settings.json");

    public static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
                Current = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath)) ?? new();
        }
        catch
        {
            Current = new();
        }
    }

    public static void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(Current));
        }
        catch
        {
            // Si no se puede guardar, la configuración vale solo para esta sesión.
        }
    }
}
