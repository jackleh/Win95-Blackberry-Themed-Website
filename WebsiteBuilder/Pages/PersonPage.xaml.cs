using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class PersonPage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public PersonPage(WebsiteDataService dataService)
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
        NameEntry.Text = _dataService.Person.Name;
        TitleEntry.Text = _dataService.Person.Title;
        LinkedinEntry.Text = _dataService.Person.LinkedinUrl;
    }

    private void SaveFields()
    {
        _dataService.Person.Name = NameEntry.Text ?? string.Empty;
        _dataService.Person.Title = TitleEntry.Text ?? string.Empty;
        _dataService.Person.LinkedinUrl = LinkedinEntry.Text ?? string.Empty;
    }

    private async void OnOpenFolder(object? sender, EventArgs e)
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful)
            {
                var loaded = await _dataService.LoadFromFolderAsync(result.Folder.Path);
                if (loaded)
                {
                    LoadFields();
                    StatusLabel.Text = $"Loaded from: {result.Folder.Path}";
                }
                else
                {
                    StatusLabel.Text = "No Website/wwwroot/data folder found.";
                }
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnSaveAll(object? sender, EventArgs e)
    {
        SaveFields();
        try
        {
            await _dataService.SaveAllAsync();
            StatusLabel.Text = "Saved successfully!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Save failed: {ex.Message}";
        }
    }
}
