using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PortfolioWebsite.Models;

namespace PortfolioWebsite.ViewModels
{
    public class BaseViewModel(HttpClient httpClient)
    {
        public Person? Person { get; private set; }
        public SiteConfig? SiteConfig { get; private set; }
        public SectionInfo? SectionInfo { get; private set; }
        public AboutMe? AboutMe { get; private set; }
        public ProjectsInfo? ProjectsInfo { get; private set; }
        public Resume? Resume { get; private set; }

        public async Task InitializeAsync()
        {
            Person = await GetPerson();
            SiteConfig = await GetSiteConfig();
            SectionInfo = await GetSectionInfo();
            AboutMe = await GetAboutMe();
            ProjectsInfo = await GetProjectsInfo();
            Resume = await GetResume();
        }
        
        private async Task<Person?> GetPerson()
        {
            const string url = "data/person.json";
            return await httpClient.GetFromJsonAsync<Person>(url);
        }

        private async Task<SiteConfig?> GetSiteConfig()
        {
            const string url = "data/siteConfig.json";
            return await httpClient.GetFromJsonAsync<SiteConfig>(url);
        }
        
        private async Task<SectionInfo?> GetSectionInfo()
        {
            const string url = "data/sectionInfo.json";
            return await httpClient.GetFromJsonAsync<SectionInfo>(url);
        }

        private async Task<AboutMe?> GetAboutMe()
        {
            const string url = "data/aboutMe.json";
            return await httpClient.GetFromJsonAsync<AboutMe>(url);
        }

        private async Task<ProjectsInfo?> GetProjectsInfo()
        {
            const string url = "data/projects.json";
            return await httpClient.GetFromJsonAsync<ProjectsInfo>(url);
        }

        private async Task<Resume?> GetResume()
        {
            const string url = "data/resume.json";
            return await httpClient.GetFromJsonAsync<Resume>(url);
        }
    }
}