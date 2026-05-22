namespace Website.Models;

public class Win95VfsEntry
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsDir { get; set; }
    public string? FileUrl { get; set; }
    public string? FileType { get; set; }
    public string? Action { get; set; }
}
