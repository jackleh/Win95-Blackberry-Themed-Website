using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortfolioWebsite.Models;

namespace PortfolioWebsite.ViewModels;

public class BaseViewModel(HttpClient httpClient)
{
    public Person? Person { get; private set; }
    public SiteConfig? SiteConfig { get; private set; }
    public AboutMe? AboutMe { get; private set; }
    public ProjectsInfo? ProjectsInfo { get; private set; }
    public Resume? Resume { get; private set; }

    public async Task InitializeAsync()
    {
        Person       = await httpClient.GetFromJsonAsync<Person>("data/person.json");
        SiteConfig   = await httpClient.GetFromJsonAsync<SiteConfig>("data/siteConfig.json");
        AboutMe      = await httpClient.GetFromJsonAsync<AboutMe>("data/aboutMe.json");
        ProjectsInfo = await httpClient.GetFromJsonAsync<ProjectsInfo>("data/projects.json");
        Resume       = await httpClient.GetFromJsonAsync<Resume>("data/resume.json");
    }
}
