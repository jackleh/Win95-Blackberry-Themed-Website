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
    
    public bool EnableEasterEggs { get; set; }
    public string EasterEggVideo1Url { get; set; }
    public string EasterEggVideo2Url { get; set; }
    public string EasterEggVideo3Url { get; set; }
    public string EasterEggDesktopPictureUrl { get; set; }

    public string EasterEggOsName { get; set; } = "Copland OS";
    public string EasterEggOsVersion { get; set; } = "Copland OS Enterprise [Version 8.00.001]";
    public string EasterEggOsCopyright { get; set; } = "(C) Copyright Copland Corp 1994-1996.";
    public string EasterEggOsDirName { get; set; } = "COPLAND";
    public string EasterEggPaintExeName { get; set; } = "paint.exe";
    public string EasterEggPaintDisplayName { get; set; } = "Paint";
    public string EasterEggNotepadExeName { get; set; } = "notepad.exe";
    public string EasterEggNotepadDisplayName { get; set; } = "Notepad";
    public string EasterEggCmdExeName { get; set; } = "cmd.exe";
    public string EasterEggResumeFileName { get; set; } = "resume.md";
    public string EasterEggProjectsFileName { get; set; } = "projects.txt";
    public string EasterEggContactFileName { get; set; } = "contact.txt";
    public string EasterEggVideo1Title { get; set; } = "lain_phone.mp4";
    public string EasterEggVideo2Title { get; set; } = "lain_vhs.mp4";
    public string EasterEggVideo3Title { get; set; } = "lain_falling_dance.mp4";
    public string EasterEggPaintInitImageUrl { get; set; } = "media/image/lain.png";
}