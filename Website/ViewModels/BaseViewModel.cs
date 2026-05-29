using System.Net.Http.Json;
using Website.Models;

namespace Website.ViewModels;

public class BaseViewModel(HttpClient httpClient)
{
    public Person? Person { get; private set; }
    public SiteConfig? SiteConfig { get; private set; }
    public ProjectsInfo? ProjectsInfo { get; private set; }
    public Resume? Resume { get; private set; }
    public Contact? Contact { get; private set; }

    public async Task InitializeAsync()
    {
        Person       = await httpClient.GetFromJsonAsync<Person>("data/person.json");
        SiteConfig   = await httpClient.GetFromJsonAsync<SiteConfig>("data/siteConfig.json");
        ProjectsInfo = await httpClient.GetFromJsonAsync<ProjectsInfo>("data/projects.json");
        Resume       = await httpClient.GetFromJsonAsync<Resume>("data/resume.json");
        Contact      = await httpClient.GetFromJsonAsync<Contact>("data/contact.json");
    }
}
