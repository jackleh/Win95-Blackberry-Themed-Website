using WebsiteBuilder.Services;

namespace WebsiteBuilder;

public partial class AppShell : Shell
{
    private const string DefaultDonateLink = "https://ko-fi.com/jackleh";
    private const string DefaultDonateText = "☕ Tip me a Coffee";
    private readonly WebsiteDataService _dataService;

    public AppShell(WebsiteDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        UpdateDonateButtonText();
    }

    internal void UpdateDonateButtonText()
    {
        TipButton.Text = string.IsNullOrWhiteSpace(_dataService.SiteConfig.DonateText)
            ? DefaultDonateText
            : $"☕ {_dataService.SiteConfig.DonateText}";
    }

    private async void OnTipMeACoffeeClicked(object? sender, EventArgs e)
    {
        var link = string.IsNullOrWhiteSpace(_dataService.SiteConfig.DonateLink)
            ? DefaultDonateLink
            : _dataService.SiteConfig.DonateLink;

        try
        {
            await Launcher.Default.OpenAsync(new Uri(link));
        }
        catch
        {
            // Silently ignore if browser cannot be opened
        }
    }
}
