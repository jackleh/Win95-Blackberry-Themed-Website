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

        public async Task InitializeAsync()
        {
            Person = await GetPerson();
            SiteConfig = await GetSiteConfig();
        }
        
        private async Task<Person?> GetPerson()
        {
            const string url = "data/person.json";
            return await httpClient.GetFromJsonAsync<Person>(url);
        }
        private async Task<SiteConfig?> GetSiteConfig()
        {
            const string url = "data/siteConfig.json";
            return  await httpClient.GetFromJsonAsync<SiteConfig>(url);
        }
    }
}