using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class AboutMePage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public AboutMePage(WebsiteDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        LoadFields();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadFields();
    }

    private void LoadFields()
    {
        var data = _dataService.AboutMe;
        SummaryEditor.Text = data.Summary;
        LocationEntry.Text = data.Location;
        GithubEntry.Text = data.GithubUrl;
        SkillsEditor.Text = string.Join("\n", data.Skills);
        LanguagesEditor.Text = string.Join("\n", data.Languages);
    }

    private void SaveFields()
    {
        var data = _dataService.AboutMe;
        data.Summary = SummaryEditor.Text ?? string.Empty;
        data.Location = LocationEntry.Text ?? string.Empty;
        data.GithubUrl = GithubEntry.Text ?? string.Empty;
        data.Skills = ParseLines(SkillsEditor.Text);
        data.Languages = ParseLines(LanguagesEditor.Text);
    }

    private static List<string> ParseLines(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];
        return text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private async void OnOpenFolder(object? sender, EventArgs e)
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful)
            {
                var loaded = await _dataService.LoadFromFolderAsync(result.Folder.Path);
                if (loaded) { LoadFields(); StatusLabel.Text = $"Loaded from: {result.Folder.Path}"; }
                else StatusLabel.Text = "No Website/wwwroot/data folder found.";
            }
        }
        catch (Exception ex) { StatusLabel.Text = $"Error: {ex.Message}"; }
    }

    private async void OnSaveAll(object? sender, EventArgs e)
    {
        SaveFields();
        try { await _dataService.SaveAllAsync(); StatusLabel.Text = "Saved successfully!"; }
        catch (Exception ex) { StatusLabel.Text = $"Save failed: {ex.Message}"; }
    }
}
