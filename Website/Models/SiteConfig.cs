namespace Website.Models;

public class SiteConfig
{
    public string SiteTitle { get; set; } = string.Empty;
    public bool Win95Enabled { get; set; }
    public string Win95Video1Url { get; set; } = string.Empty;
    public string Win95Video2Url { get; set; } = string.Empty;
    public string Win95Video3Url { get; set; } = string.Empty;
    public string Win95DesktopPictureUrl { get; set; } = string.Empty;
    public string Win95OsName { get; set; } = "Copland OS";
    public string Win95OsVersion { get; set; } = "Copland OS Enterprise [Version 8.00.001]";
    public string Win95OsCopyright { get; set; } = "(C) Copyright Copland Corp 1994-1996.";
    public string Win95OsDirName { get; set; } = "COPLAND";
    public string Win95PaintExeName { get; set; } = "paint.exe";
    public string Win95PaintDisplayName { get; set; } = "Paint";
    public string Win95NotepadExeName { get; set; } = "notepad.exe";
    public string Win95NotepadDisplayName { get; set; } = "Notepad";
    public string Win95CmdExeName { get; set; } = "cmd.exe";
    public string Win95ResumeFileName { get; set; } = "resume.md";
    public string Win95ProjectsFileName { get; set; } = "projects.txt";
    public string Win95ContactFileName { get; set; } = "contact.txt";
    public string Win95Video1Title { get; set; } = "lain_phone.mp4";
    public string Win95Video2Title { get; set; } = "lain_vhs.mp4";
    public string Win95Video3Title { get; set; } = "lain_falling_dance.mp4";
    public string Win95PaintInitImageUrl { get; set; } = "media/image/lain.png";
    public string DonateLink { get; set; } = string.Empty;
    public string DonateText { get; set; } = string.Empty;
    public List<Win95VfsEntry> Win95VfsEntries { get; set; } = [];
}
