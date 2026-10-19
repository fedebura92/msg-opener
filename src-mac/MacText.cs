namespace MSGViewer;

/// <summary>Textos propios de la versión Mac (los demás vienen de Strings.cs).</summary>
public static class MacText
{
    private static readonly Dictionary<string, string> ViewFormattedByLang = new()
    {
        ["es"] = "Ver con formato en el navegador",
        ["en"] = "View formatted in browser",
        ["de"] = "Formatiert im Browser anzeigen",
        ["fr"] = "Afficher avec mise en forme dans le navigateur",
        ["pt"] = "Ver com formatação no navegador",
        ["zh"] = "在浏览器中查看格式化内容",
        ["ja"] = "ブラウザで書式付きで表示",
        ["ko"] = "브라우저에서 서식 있는 보기",
        ["pl"] = "Pokaż z formatowaniem w przeglądarce",
        ["ru"] = "Открыть с форматированием в браузере",
        ["nl"] = "Opgemaakt weergeven in browser",
        ["hi"] = "ब्राउज़र में फ़ॉर्मेट के साथ देखें",
    };

    private static readonly Dictionary<string, string> AutoByLang = new()
    {
        ["es"] = "Automático (según el sistema)",
        ["en"] = "Automatic (follow system)",
        ["de"] = "Automatisch (wie das System)",
        ["fr"] = "Automatique (selon le système)",
        ["pt"] = "Automático (seguir o sistema)",
        ["zh"] = "自动（跟随系统）",
        ["ja"] = "自動（システムに従う）",
        ["ko"] = "자동 (시스템 설정 따름)",
        ["pl"] = "Automatyczny (zgodnie z systemem)",
        ["ru"] = "Автоматически (как в системе)",
        ["nl"] = "Automatisch (volgens systeem)",
        ["hi"] = "स्वचालित (सिस्टम के अनुसार)",
    };

    public static string Auto =>
        AutoByLang.TryGetValue(Loc.Current, out var a) ? a : AutoByLang["en"];

    public static string ViewFormatted =>
        ViewFormattedByLang.TryGetValue(Loc.Current, out var v) ? v : ViewFormattedByLang["en"];
}
