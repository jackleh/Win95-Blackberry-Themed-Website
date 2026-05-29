using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class ContactPage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public ContactPage(WebsiteDataService dataService)
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
        var data = _dataService.Contact;
        ContactNoteEntry.Text = data.ContactNote;
        CtaEditor.Text = string.Join("\n", data.CallToActionLines);
    }

    private void SaveFields()
    {
        var data = _dataService.Contact;
        data.ContactNote = ContactNoteEntry.Text ?? string.Empty;
        data.CallToActionLines = ParseLines(CtaEditor.Text);
    }

    private static List<string> ParseLines(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];
        // WinUI's Editor separates lines with '\r' (not '\n'), so split on all
        // newline variants — otherwise multi-line input collapses into one entry.
        return text
            .Split(["\r\n", "\r", "\n"], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
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
