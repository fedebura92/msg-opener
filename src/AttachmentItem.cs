namespace MSGViewer;

public sealed class AttachmentItem
{
    public string FileName { get; init; } = "Attachment";
    public byte[] Data { get; init; } = Array.Empty<byte>();

    public string SizeText =>
        Data.Length < 1024 ? $"{Data.Length} B"
        : Data.Length < 1024 * 1024 ? $"{Data.Length / 1024d:0.#} KB"
        : $"{Data.Length / 1024d / 1024d:0.#} MB";

    // Textos de los botones (se actualizan al cambiar de idioma).
    public string OpenLabel => Loc.T("open");
    public string SaveLabel => Loc.T("save");
}
