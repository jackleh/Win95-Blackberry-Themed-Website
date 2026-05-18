using Microsoft.JSInterop;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;

namespace PortfolioWebsite.Components.EasterEgg;

public partial class Win95Desktop
{
    // ── CMD virtual filesystem ────────────────────────────────────────────────
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
            new("sectionInfo.json",IsDir: false, FileUrl: "data/sectionInfo.json", FileType: "text"),
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
                switch (entry.Name.ToLower())
                {
                    case "cmd.exe":
                        lines.Add("CMD is already running in this session.");
                        break;
                    case "paint.exe":
                        _paintInitImageUrl = "media/image/lain.png";
                        _paintOpen         = true;
                        _paintMinimized    = false;
                        break;
                    case "notepad.exe":
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

    private string ResumeHtml
    {
        get
        {
            var linkedin = BaseViewModel.Person?.LinkedinUrl ?? "#";
            var github   = BaseViewModel.AboutMe?.GithubUrl  ?? "#";

            // Validate and encode URLs to prevent XSS
            if (!Uri.TryCreate(linkedin, UriKind.Absolute, out _) && linkedin != "#")
                linkedin = "#";
            if (!Uri.TryCreate(github, UriKind.Absolute, out _) && github != "#")
                github = "#";

            var linkedinEncoded = HtmlEncoder.Default.Encode(linkedin);
            var githubEncoded = HtmlEncoder.Default.Encode(github);

            var sb = new StringBuilder();
            sb.AppendLine("<div class='resume-doc'>");

            // Header
            sb.AppendLine("  <h1>Jack Lehman</h1>");
            sb.AppendLine("  <p class='resume-tagline'>Mobile Software Developer</p>");
            sb.AppendLine("  <p class='resume-meta'>");
            sb.AppendLine("    jackrlehman05@gmail.com &nbsp;&middot;&nbsp;");
            sb.AppendLine("    210-429-3959 &nbsp;&middot;&nbsp;");
            sb.AppendLine("    Las Vegas, NV &mdash; Open to Remote &nbsp;&middot;&nbsp;");
            sb.AppendLine($"    <a href='{linkedinEncoded}' target='_blank'>LinkedIn</a> &nbsp;&middot;&nbsp;");
            sb.AppendLine($"    <a href='{githubEncoded}' target='_blank'>GitHub</a>");
            sb.AppendLine("  </p>");
            sb.AppendLine("  <hr/>");

            // About
            sb.AppendLine("  <h2>About</h2>");
            sb.AppendLine("  <p>Senior Software Developer specialized in .NET MAUI, Blazor Software Development, and Sports Betting. I am committed to and take pride in developing quality software products. I excel at problem solving, leadership, teamwork, and adaptability.</p>");

            // Job History
            sb.AppendLine("  <h2>Job History</h2>");

            sb.AppendLine("  <h3>Circa Resort &amp; Casino &mdash; Las Vegas</h3>");

            sb.AppendLine("  <p class='resume-role'>Senior Software Engineer &nbsp;&middot;&nbsp; <span class='resume-date'>2025 &ndash; 2026</span></p>");
            sb.AppendLine("  <ul>");
            sb.AppendLine("    <li>Continued leading mobile development efforts on Circa Sports&rsquo; MAUI Blazor Mobile and Desktop real money Sports Betting app, issuing large quarterly updates.</li>");
            sb.AppendLine("    <li>Continued to concept and create new innovative features in the Sports Betting industry which resulted in increased user engagement and reception &mdash; including a stats tracker on win/losses, create quick parlay by shaking your phone, and the ability to track bets by various categories.</li>");
            sb.AppendLine("  </ul>");

            sb.AppendLine("  <p class='resume-role'>Mobile Developer &nbsp;&middot;&nbsp; <span class='resume-date'>2024 &ndash; 2025</span></p>");
            sb.AppendLine("  <ul>");
            sb.AppendLine("    <li>Responsible for leading mobile development efforts on Circa Sports&rsquo; MAUI Blazor Mobile and Desktop Sports Betting app from conception to release.</li>");
            sb.AppendLine("    <li>Creator of the &ldquo;Round Robin Parlay Inspector&rdquo; &mdash; a new and first-of-its-kind feature in Sports Betting focusing on transparency and unraveling confusing aspects of round robins by showing bet item groupings per &ldquo;by&rdquo; and the potential win on each parlay.</li>");
            sb.AppendLine("  </ul>");

            sb.AppendLine("  <h3>ReactorNet Technologies &mdash; San Antonio</h3>");
            sb.AppendLine("  <p class='resume-role'>Software Developer &nbsp;&middot;&nbsp; <span class='resume-date'>2021 &ndash; 2024</span></p>");
            sb.AppendLine("  <ul>");
            sb.AppendLine("    <li>Lead developer on &ldquo;EPRO Mobile&rdquo; &mdash; a .NET MAUI XAML Mobile procure-to-pay application on the Apple App Store and Google Play Store, brought from concept phase to live production with constant new features and fixes.</li>");
            sb.AppendLine("    <li>Created innovative features including a liquid level detector using the phone&rsquo;s camera and AI for auto re-ordering based on predicted consumption levels.</li>");
            sb.AppendLine("    <li>Adhered to the company&rsquo;s stack by developing a Delphi Berlin API to communicate between EPRO Mobile and the backend.</li>");
            sb.AppendLine("    <li>Assisted with development of ReactorNet&rsquo;s procure-to-pay application built in Delphi.</li>");
            sb.AppendLine("  </ul>");

            // Skills
            sb.AppendLine("  <h2>Skills</h2>");
            sb.AppendLine("  <p class='resume-role'>By Domain</p>");
            sb.AppendLine("  <ul>");
            sb.AppendLine("    <li>Sports Betting and Prediction Markets &mdash; Strong industry level knowledge</li>");
            sb.AppendLine("    <li>Frontend &mdash; Subject Matter Expert</li>");
            sb.AppendLine("    <li>Backend &mdash; Mid Level Knowledge</li>");
            sb.AppendLine("    <li>Database &mdash; Jr Level Knowledge</li>");
            sb.AppendLine("  </ul>");
            sb.AppendLine("  <p class='resume-role'>Breakdown</p>");
            sb.AppendLine("  <ul>");
            sb.AppendLine("    <li><strong>UI, UX, and Asset Design</strong> &mdash; From concepting to implementation (Photoshop, Figma, HTML, CSS, JS)</li>");
            sb.AppendLine("    <li><strong>.NET MAUI</strong> &mdash; Working with MAUI since RC2 1.0; prior Xamarin experience for several years</li>");
            sb.AppendLine("    <li><strong>Blazor</strong> &mdash; Highly experienced creating scalable frontend code via components and MVVM architecture</li>");
            sb.AppendLine("    <li><strong>C#</strong> &mdash; Highly confident C# developer focused on performant, reusable code</li>");
            sb.AppendLine("    <li><strong>Delphi Berlin (10.1, Pascal)</strong> &mdash; Learnt to meet ReactorNet&rsquo;s stack expectations; responsible for mobile product and supporting backend</li>");
            sb.AppendLine("    <li><strong>AI Agentic Workflows</strong> &mdash; Custom agents for planning features, coding assistance, documentation, and unit tests</li>");
            sb.AppendLine("    <li><strong>Sports Betting, Prediction Markets, Options Markets</strong> &mdash; 2+ years in a highly technical sports betting role; prior options trading experience</li>");
            sb.AppendLine("    <li><strong>Leadership</strong> &mdash; Led a team of 6 overseas developers; later transitioned to local talent to increase Sports Betting domain knowledge</li>");
            sb.AppendLine("  </ul>");

            // Education
            sb.AppendLine("  <h2>Education</h2>");
            sb.AppendLine("  <p>San Antonio College &mdash; AAS in Computer Science &nbsp;&middot;&nbsp; 2020</p>");

            sb.AppendLine("</div>");
            return sb.ToString();
        }
    }

    private string ContactText
    {
        get
        {
            var linkedin = BaseViewModel.Person?.LinkedinUrl ?? "N/A";
            var github   = BaseViewModel.AboutMe?.GithubUrl  ?? "N/A";

            var sb = new StringBuilder();
            sb.AppendLine("  Best way to reach me: LinkedIn");
            sb.AppendLine();
            sb.AppendLine($"  LinkedIn >> {linkedin}");
            sb.AppendLine($"  GitHub   >> {github}");
            sb.AppendLine($"  Email    >> jackrlehman05@gmail.com");
            sb.AppendLine($"  Location >> Las Vegas, NV — Open to Remote");
            sb.AppendLine();
            sb.AppendLine("  +---------------------------------------------+");
            sb.AppendLine("  | Feel free to reach out for opportunities,   |");
            sb.AppendLine("  | collaborations, or just to say hello!       |");
            sb.AppendLine("  +---------------------------------------------+");
            return sb.ToString();
        }
    }
}
