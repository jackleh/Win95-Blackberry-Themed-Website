namespace PortfolioWebsite.Models;

public class AboutMe
{
    public string Summary { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = [];
    public List<string> Languages { get; set; } = [];
    public string Location { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
}
