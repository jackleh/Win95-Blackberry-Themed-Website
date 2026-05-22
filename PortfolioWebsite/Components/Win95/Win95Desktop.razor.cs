using Microsoft.JSInterop;
using System.Net.Http;
using System.Text;

namespace PortfolioWebsite.Components.Win95;

public partial class Win95Desktop
{
    // ── CMD virtual filesystem ────────────────────────────────────────────────
    private record FsEntry(string Name, bool IsDir, string? FileUrl = null, string? FileType = null, string? Action = null);

    private string _cwd = "C:\\";
    private string _paintInitImageUrl = "media/image/lain.png";

    private Dictionary<string, List<FsEntry>>? _vfs;

    // ── Config helpers ────────────────────────────────────────────────────────
    private string OsName             => BaseViewModel.SiteConfig?.Win95OsName             ?? "Copland OS";
    private string OsVersion          => BaseViewModel.SiteConfig?.Win95OsVersion          ?? "Copland OS Enterprise [Version 8.00.001]";
    private string OsCopyright        => BaseViewModel.SiteConfig?.Win95OsCopyright        ?? "(C) Copyright Copland Corp 1994-1996.";
    private string OsDirName          => BaseViewModel.SiteConfig?.Win95OsDirName          ?? "COPLAND";
    private string PaintExeName       => BaseViewModel.SiteConfig?.Win95PaintExeName       ?? "paint.exe";
    private string PaintDisplayName   => BaseViewModel.SiteConfig?.Win95PaintDisplayName   ?? "Paint";
    private string NotepadExeName     => BaseViewModel.SiteConfig?.Win95NotepadExeName     ?? "notepad.exe";
    private string NotepadDisplayName => BaseViewModel.SiteConfig?.Win95NotepadDisplayName ?? "Notepad";
    private string CmdExeName         => BaseViewModel.SiteConfig?.Win95CmdExeName         ?? "cmd.exe";
    private string ResumeFileName     => BaseViewModel.SiteConfig?.Win95ResumeFileName     ?? "resume.md";
    private string ProjectsFileName   => BaseViewModel.SiteConfig?.Win95ProjectsFileName   ?? "projects.txt";
    private string ContactFileName    => BaseViewModel.SiteConfig?.Win95ContactFileName    ?? "contact.txt";
    private string Video1Title        => BaseViewModel.SiteConfig?.Win95Video1Title        ?? "lain_phone.mp4";
    private string Video2Title        => BaseViewModel.SiteConfig?.Win95Video2Title        ?? "lain_vhs.mp4";
    private string Video3Title        => BaseViewModel.SiteConfig?.Win95Video3Title        ?? "lain_falling_dance.mp4";
    private string PaintInitImageUrl  => BaseViewModel.SiteConfig?.Win95PaintInitImageUrl  ?? "media/image/lain.png";
    private string CmdTitle           => $"C:\\{OsDirName}\\system32\\{CmdExeName}";

    private void InitVfs()
    {
        _cwd               = $"C:\\{OsDirName}";
        _paintInitImageUrl = PaintInitImageUrl;

        _vfs = new Dictionary<string, List<FsEntry>>(StringComparer.OrdinalIgnoreCase)
        {
            ["C:\\"] =
            [
                new("Desktop", IsDir: true),
                new(OsDirName, IsDir: true),
                new("WWWROOT", IsDir: true),
            ],
            ["C:\\Desktop"] =
            [
                new(ResumeFileName,   IsDir: false, FileUrl: null, FileType: "desktop", Action: "open-resume"),
                new(ProjectsFileName, IsDir: false, FileUrl: null, FileType: "desktop", Action: "open-projects"),
                new(ContactFileName,  IsDir: false, FileUrl: null, FileType: "desktop", Action: "open-contact"),
            ],
            [$"C:\\{OsDirName}"] =
            [
                new("System32", IsDir: true),
            ],
            [$"C:\\{OsDirName}\\System32"] =
            [
                new(CmdExeName,     IsDir: false, FileUrl: null, FileType: "exe", Action: "run-cmd"),
                new(PaintExeName,   IsDir: false, FileUrl: null, FileType: "exe", Action: "run-paint"),
                new(NotepadExeName, IsDir: false, FileUrl: null, FileType: "exe", Action: "run-notepad"),
            ],
            ["C:\\WWWROOT"] =
            [
                new("css",         IsDir: true),
                new("data",        IsDir: true),
                new("js",          IsDir: true),
                new("media",       IsDir: true),
                new("favicon.png", IsDir: false, FileUrl: "favicon.png", FileType: "image"),
                new("index.html",  IsDir: false, FileUrl: "index.html",  FileType: "text"),
            ],
            ["C:\\WWWROOT\\css"] =
            [
                new("app.css", IsDir: false, FileUrl: "css/app.css", FileType: "text"),
            ],
            ["C:\\WWWROOT\\data"] =
            [
                new("aboutMe.json",     IsDir: false, FileUrl: "data/aboutMe.json",     FileType: "text"),
                new("contact.json",     IsDir: false, FileUrl: "data/contact.json",     FileType: "text"),
                new("person.json",      IsDir: false, FileUrl: "data/person.json",      FileType: "text"),
                new("projects.json",    IsDir: false, FileUrl: "data/projects.json",    FileType: "text"),
                new("resume.json",      IsDir: false, FileUrl: "data/resume.json",      FileType: "text"),
                new("sectionInfo.json", IsDir: false, FileUrl: "data/sectionInfo.json", FileType: "text"),
                new("siteConfig.json",  IsDir: false, FileUrl: "data/siteConfig.json",  FileType: "text"),
            ],
            ["C:\\WWWROOT\\js"] =
            [
                new("dragHelper.js",  IsDir: false, FileUrl: "js/dragHelper.js",  FileType: "text"),
                new("paintHelper.js", IsDir: false, FileUrl: "js/paintHelper.js", FileType: "text"),
            ],
            ["C:\\WWWROOT\\media"] =
            [
                new("image",       IsDir: true),
                new("resume.docx", IsDir: false, FileUrl: null, FileType: "binary"),
            ],
            ["C:\\WWWROOT\\media\\image"] =
            [
                new("lain.png",      IsDir: false, FileUrl: "media/image/lain.png",      FileType: "image"),
                new("lain_navi.png", IsDir: false, FileUrl: "media/image/lain_navi.png", FileType: "image"),
            ],
        };
    }

    private List<string> CmdDir()
    {
        var lines = new List<string>();
        if (_vfs == null || !_vfs.TryGetValue(_cwd, out var entries))
        {
            lines.Add("The system cannot find the path specified.");
            return lines;
        }

        lines.Add($" Directory of {_cwd}");
        lines.Add("");
        if (_cwd != "C:\\")
            lines.Add("05/17/96  09:00a  <DIR>             ..");

        int fileCount = 0, dirCount = 0;
        foreach (var e in entries)
        {
            if (e.IsDir)
            {
                lines.Add($"05/17/96  09:00a  <DIR>             {e.Name}");
                dirCount++;
            }
            else
            {
                var size = e.Name.Length * 137 + 512;
                lines.Add($"05/17/96  09:00a       {size,9:#,##0}  {e.Name}");
                fileCount++;
            }
        }

        lines.Add("");
        lines.Add($"            {fileCount} File(s)");
        lines.Add($"            {dirCount} Dir(s)   8,423,424 bytes free");
        return lines;
    }

    private void CmdCd(string arg)
    {
        if (string.IsNullOrWhiteSpace(arg) || arg == ".")
        {
            if (string.IsNullOrWhiteSpace(arg))
                _cmdHistory.Add(_cwd);
            return;
        }

        var resolved = ResolvePath(arg);
        if (resolved != null)
            _cwd = resolved;
        else
            _cmdHistory.Add("The system cannot find the path specified.");
    }

    // Resolves a path arg relative to _cwd, handling .., absolute, and root-relative paths.
    // Returns the canonical VFS key, or null if the destination doesn't exist.
    private string? ResolvePath(string path)
    {
        if (_vfs == null) return null;

        string current;
        string[] parts;

        if (path.Length >= 2 && path[1] == ':')
        {
            // Absolute: C:\WWWROOT\data
            current = "C:\\";
            parts   = path.Length > 3 ? path[3..].Split('\\', StringSplitOptions.RemoveEmptyEntries) : [];
        }
        else if (path.StartsWith('\\'))
        {
            // Root-relative: \WWWROOT\data
            current = "C:\\";
            parts   = path[1..].Split('\\', StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
            // Relative to _cwd
            current = _cwd;
            parts   = path.Split('\\', StringSplitOptions.RemoveEmptyEntries);
        }

        foreach (var part in parts)
        {
            if (part == ".") continue;

            if (part == "..")
            {
                var sep = current.TrimEnd('\\').LastIndexOf('\\');
                current = sep <= 2 ? "C:\\" : current[..sep];
            }
            else
            {
                var candidate = current.TrimEnd('\\') + "\\" + part;
                var match = _vfs.Keys.FirstOrDefault(k => k.Equals(candidate, StringComparison.OrdinalIgnoreCase));
                if (match == null) return null;
                current = match;
            }
        }

        // Verify the final path is a known directory in the VFS
        return _vfs.ContainsKey(current) ? current : null;
    }

    private async Task<List<string>> CmdOpen(string filename)
    {
        var lines = new List<string>();

        if (_vfs == null || !_vfs.TryGetValue(_cwd, out var entries))
        {
            lines.Add($"'{filename}' is not recognized as an internal or external command,");
            lines.Add("operable program or batch file.");
            return lines;
        }

        var entry = entries.FirstOrDefault(e => e.Name.Equals(filename, StringComparison.OrdinalIgnoreCase));
        if (entry == null)
        {
            lines.Add($"'{filename}' is not recognized as an internal or external command,");
            lines.Add("operable program or batch file.");
            return lines;
        }

        if (entry.IsDir)
        {
            lines.Add("Access is denied.");
            return lines;
        }

        switch (entry.FileType)
        {
            case "desktop":
                switch (entry.Action)
                {
                    case "open-resume":   OpenResumeWindow();   break;
                    case "open-projects": OpenProjectsWindow(); break;
                    case "open-contact":  OpenContactWindow();  break;
                }
                break;

            case "text":
                try
                {
                    var content = await Http.GetStringAsync(entry.FileUrl);
                    _notepadText      = content;
                    _notepadOpen      = true;
                    _notepadNeedsInit = true;
                    _notepadMinimized = false;
                    _notepadMaximized = false;
                }
                catch
                {
                    lines.Add("Error: Could not open file.");
                }
                break;

            case "image":
                _paintInitImageUrl = entry.FileUrl!;
                if (_paintOpen)
                    await JS.InvokeVoidAsync("paintHelper.loadImage", "paintCanvas", entry.FileUrl);
                else
                {
                    _paintOpen      = true;
                    _paintMinimized = false;
                }
                break;

            case "binary":
                lines.Add($"Cannot open binary file '{entry.Name}'.");
                break;

            case "exe":
                switch (entry.Action)
                {
                    case "run-cmd":
                        lines.Add("CMD is already running in this session.");
                        break;
                    case "run-paint":
                        _paintInitImageUrl = PaintInitImageUrl;
                        _paintOpen         = true;
                        _paintMinimized    = false;
                        break;
                    case "run-notepad":
                        _notepadOpen      = true;
                        _notepadMinimized = false;
                        break;
                }
                break;

            default:
                lines.Add($"'{filename}' is not recognized as an internal or external command,");
                lines.Add("operable program or batch file.");
                break;
        }

        return lines;
    }

    // ── Resume / Projects / Contact text ─────────────────────────────────────
    private string ProjectsText
    {
        get
        {
            var projects = BaseViewModel.ProjectsInfo?.Projects;
            if (projects == null || projects.Count == 0)
                return "No projects found.";

            var sb = new StringBuilder();
            for (int i = 0; i < projects.Count; i++)
            {
                var p      = projects[i];
                var status = !string.IsNullOrEmpty(p.Status) ? $"  [{p.Status.ToUpper()}]" : "";
                var stack  = string.Join(" | ", p.Technologies);
                sb.AppendLine($"  [{(i + 1):D2}] {p.Name}{status}");
                sb.AppendLine($"       Stack  : {stack}");
                sb.AppendLine($"       Desc   : {p.Description}");
                if (!string.IsNullOrEmpty(p.Url))
                    sb.AppendLine($"       Repo   : {p.Url}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }

    private string ContactText
    {
        get
        {
            var contact  = BaseViewModel.Contact;
            var linkedin = BaseViewModel.Person?.LinkedinUrl ?? "N/A";
            var github   = BaseViewModel.AboutMe?.GithubUrl  ?? "N/A";
            var email    = BaseViewModel.Resume?.Email    ?? "N/A";
            var location = BaseViewModel.Resume?.Location ?? "N/A";

            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(contact?.ContactNote))
            {
                sb.AppendLine($"  {contact.ContactNote}");
                sb.AppendLine();
            }
            sb.AppendLine($"  LinkedIn >> {linkedin}");
            sb.AppendLine($"  GitHub   >> {github}");
            sb.AppendLine($"  Email    >> {email}");
            sb.AppendLine($"  Location >> {location}");

            if (contact?.CallToActionLines?.Count > 0)
            {
                var maxLen = contact.CallToActionLines.Max(l => l.Length);
                var border = "  +" + new string('-', maxLen + 2) + "+";
                sb.AppendLine();
                sb.AppendLine(border);
                foreach (var line in contact.CallToActionLines)
                    sb.AppendLine($"  | {line.PadRight(maxLen)} |");
                sb.AppendLine(border);
            }

            return sb.ToString();
        }
    }

    private static string SafeHref(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return string.Empty;
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return string.Empty;
        return url;
    }
}
