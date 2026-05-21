namespace PortfolioWebsite.Models;

public class SiteConfig
{
    public int OpeningSectionHoldDurationInMs { get; set; }

    private int _openingSectionFadeDurationInMs { get; set; }

    [Limitation("Only works if 1k,1.5k,2k,2.5k,3k from JSON due to how CSS struggles with dynamic values")]
    public int OpeningSectionFadeDurationInMs
    {
        get => _openingSectionFadeDurationInMs;
        set
        {
            int[] allowedValues = [1000, 1500, 2000, 2500, 3000];
            _openingSectionFadeDurationInMs = allowedValues.Contains(value)
                ? value
                : allowedValues.OrderBy(x => Math.Abs(x - value)).First();
        }
    }

    private int _sectionSelectionFlashTimeDurationInMs { get; set; }

    [Limitation("Only works if 1k,1.5k,2k,2.5k,3k from JSON due to how CSS struggles with dynamic values")]
    public int SectionSelectionFlashTimeDurationInMs
    {
        get => _sectionSelectionFlashTimeDurationInMs;
        set
        {
           int[] allowedValues = [1000, 1500, 2000, 2500, 3000];
           _sectionSelectionFlashTimeDurationInMs = allowedValues.Contains(value) 
               ? value 
               : allowedValues.OrderBy(x => Math.Abs(x - value)).First();
        }
    }
    public string SiteTitle { get; set; }
    
    public bool Win95Enabled { get; set; }
    public string Win95Video1Url { get; set; }
    public string Win95Video2Url { get; set; }
    public string Win95Video3Url { get; set; }
    public string Win95DesktopPictureUrl { get; set; }

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
}