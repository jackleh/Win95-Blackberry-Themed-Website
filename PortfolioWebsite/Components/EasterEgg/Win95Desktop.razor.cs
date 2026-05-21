using Microsoft.JSInterop;
using System.Net.Http;

namespace PortfolioWebsite.Components.EasterEgg;

public partial class Win95Desktop
{
    private record FsEntry(string Name, bool IsDir, string? FileUrl = null, string? FileType = null);

    private string _cwd = "C:\\COPLAND";
    private string _paintInitImageUrl = "media/image/lain.png";

    private static readonly Dictionary<string, List<FsEntry>> _vfs = new(StringComparer.OrdinalIgnoreCase)
    {
        ["C:\\"] =
        [
            new("Desktop", IsDir: true),
            new("COPLAND", IsDir: true),
            new("WWWROOT", IsDir: true),
        ],
        ["C:\\Desktop"] =
        [
            new("resume.md",    IsDir: false, FileUrl: null, FileType: "desktop"),
            new("projects.txt", IsDir: false, FileUrl: null, FileType: "desktop"),
            new("contact.txt",  IsDir: false, FileUrl: null, FileType: "desktop"),
        ],
        ["C:\\COPLAND"] =
        [
            new("System32", IsDir: true),
        ],
        ["C:\\COPLAND\\System32"] =
        [
            new("cmd.exe",      IsDir: false, FileUrl: null, FileType: "exe"),
            new("paint.exe",    IsDir: false, FileUrl: null, FileType: "exe"),
            new("notepad.exe",  IsDir: false, FileUrl: null, FileType: "exe"),
        ],
        ["C:\\WWWROOT"] =
        [
            new("css",          IsDir: true),
            new("data",         IsDir: true),
            new("js",           IsDir: true),
            new("media",        IsDir: true),
            new("favicon.png",  IsDir: false, FileUrl: "favicon.png",  FileType: "image"),
            new("index.html",   IsDir: false, FileUrl: "index.html",   FileType: "text"),
        ],
        ["C:\\WWWROOT\\css"] =
        [
            new("app.css", IsDir: false, FileUrl: "css/app.css", FileType: "text"),
        ],
        ["C:\\WWWROOT\\data"] =
        [
            new("aboutMe.json",    IsDir: false, FileUrl: "data/aboutMe.json",     FileType: "text"),
            new("person.json",     IsDir: false, FileUrl: "data/person.json",      FileType: "text"),
            new("projects.json",   IsDir: false, FileUrl: "data/projects.json",    FileType: "text"),
            new("resume.json",     IsDir: false, FileUrl: "data/resume.json",      FileType: "text"),
            new("siteConfig.json", IsDir: false, FileUrl: "data/siteConfig.json",  FileType: "text"),
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

    private List<string> CmdDir()
    {
        var lines = new List<string>();
        if (!_vfs.TryGetValue(_cwd, out var entries))
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
        string current;
        string[] parts;

        if (path.Length >= 2 && path[1] == ':')
        {
            current = "C:\\";
            parts   = path.Length > 3 ? path[3..].Split('\\', StringSplitOptions.RemoveEmptyEntries) : [];
        }
        else if (path.StartsWith('\\'))
        {
            current = "C:\\";
            parts   = path[1..].Split('\\', StringSplitOptions.RemoveEmptyEntries);
        }
        else
        {
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

        return _vfs.ContainsKey(current) ? current : null;
    }

    private async Task<List<string>> CmdOpen(string filename)
    {
        var lines = new List<string>();

        if (!_vfs.TryGetValue(_cwd, out var entries))
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
                switch (entry.Name.ToLower())
                {
                    case "resume.md":    OpenResumeWindow();   break;
                    case "projects.txt": OpenProjectsWindow(); break;
                    case "contact.txt":  OpenContactWindow();  break;
                }
                break;

            case "text":
                try
                {
                    var content = await Http.GetStringAsync(entry.FileUrl);
                    _notepadText      = content;
                    _notepadOpen      = true;
                    _notepadMinimized = false;
                    _notepadMaximized = false;
                    _notepadNeedsInit = true;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"CmdOpen text failed for {entry.FileUrl}: {ex.Message}");
                    lines.Add("Error: Could not open file.");
                }
                break;

            case "image":
                _paintInitImageUrl = entry.FileUrl!;
                if (_paintOpen)
                {
                    _paintMinimized = false;
                    try
                    {
                        await JS.InvokeVoidAsync("paintHelper.loadImage", "paintCanvas", entry.FileUrl);
                        await JS.InvokeVoidAsync("dragHelper.raiseWindow", "paintWindow");
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine($"paintHelper.loadImage failed: {ex.Message}");
                    }
                }
                else
                {
                    _paintOpen      = true;
                    _paintMinimized = false;
                    _paintNeedsInit = true;
                }
                break;

            case "binary":
                lines.Add($"Cannot open binary file '{entry.Name}'.");
                break;

            case "exe":
                switch (entry.Name.ToLower())
                {
                    case "cmd.exe":
                        lines.Add("CMD is already running in this session.");
                        break;
                    case "paint.exe":
                        _paintInitImageUrl = "media/image/lain.png";
                        if (_paintOpen)
                        {
                            _paintMinimized = false;
                            try
                            {
                                await JS.InvokeVoidAsync("paintHelper.loadImage", "paintCanvas", _paintInitImageUrl);
                                await JS.InvokeVoidAsync("dragHelper.raiseWindow", "paintWindow");
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine($"paintHelper.loadImage failed: {ex.Message}");
                            }
                        }
                        else
                        {
                            _paintOpen      = true;
                            _paintMinimized = false;
                            _paintNeedsInit = true;
                        }
                        break;
                    case "notepad.exe":
                        _notepadOpen      = true;
                        _notepadMinimized = false;
                        _notepadNeedsInit = true;
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

    private string ProjectsText
    {
        get
        {
            var projects = BaseViewModel.ProjectsInfo?.Projects;
            if (projects == null || projects.Count == 0)
                return "No projects found.";

            var sb = new System.Text.StringBuilder();
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
            var linkedin = BaseViewModel.Person?.LinkedinUrl ?? "N/A";
            var github   = BaseViewModel.AboutMe?.GithubUrl  ?? "N/A";
            var email    = BaseViewModel.Resume?.Email       ?? "N/A";
            var location = BaseViewModel.Resume?.Location ?? BaseViewModel.AboutMe?.Location ?? "N/A";

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("  Best way to reach me: LinkedIn");
            sb.AppendLine();
            sb.AppendLine($"  LinkedIn >> {linkedin}");
            sb.AppendLine($"  GitHub   >> {github}");
            sb.AppendLine($"  Email    >> {email}");
            sb.AppendLine($"  Location >> {location}");
            sb.AppendLine();
            sb.AppendLine("  +---------------------------------------------+");
            sb.AppendLine("  | Feel free to reach out for opportunities,   |");
            sb.AppendLine("  | collaborations, or just to say hello!       |");
            sb.AppendLine("  +---------------------------------------------+");
            return sb.ToString();
        }
    }

    // Only http(s) URLs are safe to inject into <a href> markup. Anything else
    // (e.g. javascript:, data:) is replaced with "#".
    private static string SafeHref(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return string.Empty;
        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps) return string.Empty;
        return url;
    }
}
