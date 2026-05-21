namespace PortfolioWebsite.Models;

public class Contact
{
    public string ContactNote { get; set; } = string.Empty;
    public List<string> CallToActionLines { get; set; } = [];
}
