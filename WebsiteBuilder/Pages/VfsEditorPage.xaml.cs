using WebsiteBuilder.Models;
using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class VfsEditorPage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public VfsEditorPage(WebsiteDataService dataService)
    {
        InitializeComponent();
        _dataService = dataService;
        RebuildUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RebuildUI();
    }

    private void RebuildUI()
    {
        EntriesList.Children.Clear();
        var entries = _dataService.SiteConfig.Win95VfsEntries;

        for (int i = 0; i < entries.Count; i++)
        {
            var entry = entries[i];
            var index = i;

            var frame = new Frame
            {
                BorderColor = entry.IsDir ? Colors.CornflowerBlue : Colors.LightGray,
                Padding = new Thickness(12),
                CornerRadius = 6,
                HasShadow = false
            };

            var layout = new VerticalStackLayout { Spacing = 8 };

            var headerLayout = new HorizontalStackLayout { Spacing = 8 };
            headerLayout.Children.Add(new Label
            {
                Text = entry.IsDir ? $"📁 #{index + 1} Directory" : $"📄 #{index + 1} File",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.Center
            });
            layout.Children.Add(headerLayout);

            // Path
            var pathRow = new HorizontalStackLayout { Spacing = 8 };
            pathRow.Children.Add(new Label { Text = "Path:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
            var pathEntry = new Entry { Text = entry.Path, Placeholder = @"C:\Desktop", HorizontalOptions = LayoutOptions.FillAndExpand };
            pathEntry.TextChanged += (_, e) => entry.Path = e.NewTextValue ?? string.Empty;
            pathRow.Children.Add(pathEntry);
            layout.Children.Add(pathRow);

            // Name
            var nameRow = new HorizontalStackLayout { Spacing = 8 };
            nameRow.Children.Add(new Label { Text = "Name:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
            var nameEntry = new Entry { Text = entry.Name, Placeholder = "filename.ext", HorizontalOptions = LayoutOptions.FillAndExpand };
            nameEntry.TextChanged += (_, e) => entry.Name = e.NewTextValue ?? string.Empty;
            nameRow.Children.Add(nameEntry);
            layout.Children.Add(nameRow);

            // IsDir
            var isDirRow = new HorizontalStackLayout { Spacing = 8 };
            isDirRow.Children.Add(new Label { Text = "Is Directory:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
            var isDirSwitch = new Switch { IsToggled = entry.IsDir };
            isDirSwitch.Toggled += (_, e) => { entry.IsDir = e.Value; RebuildUI(); };
            isDirRow.Children.Add(isDirSwitch);
            layout.Children.Add(isDirRow);

            // File-specific fields (only show for files)
            if (!entry.IsDir)
            {
                // FileUrl
                var urlRow = new HorizontalStackLayout { Spacing = 8 };
                urlRow.Children.Add(new Label { Text = "File URL:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
                var urlEntry = new Entry { Text = entry.FileUrl ?? string.Empty, Placeholder = "media/image/file.png", HorizontalOptions = LayoutOptions.FillAndExpand };
                urlEntry.TextChanged += (_, e) => entry.FileUrl = string.IsNullOrWhiteSpace(e.NewTextValue) ? null : e.NewTextValue;
                urlRow.Children.Add(urlEntry);

                var browseBtn = new Button { Text = "Browse...", FontSize = 12 };
                browseBtn.Clicked += async (_, _) => await PickFileForEntry(entry, urlEntry);
                urlRow.Children.Add(browseBtn);
                layout.Children.Add(urlRow);

                // FileType
                var typeRow = new HorizontalStackLayout { Spacing = 8 };
                typeRow.Children.Add(new Label { Text = "File Type:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
                var typePicker = new Picker
                {
                    Title = "Select type",
                    ItemsSource = new List<string> { "text", "image", "binary", "exe", "desktop" },
                    WidthRequest = 150
                };
                if (!string.IsNullOrEmpty(entry.FileType))
                    typePicker.SelectedItem = entry.FileType;
                typePicker.SelectedIndexChanged += (_, _) => entry.FileType = typePicker.SelectedItem?.ToString();
                typeRow.Children.Add(typePicker);
                layout.Children.Add(typeRow);

                // Action
                var actionRow = new HorizontalStackLayout { Spacing = 8 };
                actionRow.Children.Add(new Label { Text = "Action:", FontAttributes = FontAttributes.Bold, WidthRequest = 80, VerticalOptions = LayoutOptions.Center });
                var actionPicker = new Picker
                {
                    Title = "Select action (optional)",
                    ItemsSource = new List<string> { "", "open-resume", "open-projects", "open-contact", "run-cmd", "run-paint", "run-notepad" },
                    WidthRequest = 200
                };
                if (!string.IsNullOrEmpty(entry.Action))
                    actionPicker.SelectedItem = entry.Action;
                actionPicker.SelectedIndexChanged += (_, _) =>
                {
                    var selected = actionPicker.SelectedItem?.ToString();
                    entry.Action = string.IsNullOrEmpty(selected) ? null : selected;
                };
                actionRow.Children.Add(actionPicker);
                layout.Children.Add(actionRow);
            }

            // Remove button
            var removeBtn = new Button { Text = "Remove Entry", BackgroundColor = Colors.Red, FontSize = 12 };
            var capturedIndex = index;
            removeBtn.Clicked += (_, _) =>
            {
                _dataService.SiteConfig.Win95VfsEntries.RemoveAt(capturedIndex);
                RebuildUI();
            };
            layout.Children.Add(removeBtn);

            frame.Content = layout;
            EntriesList.Children.Add(frame);
        }
    }

    private async Task PickFileForEntry(Win95VfsEntry entry, Entry urlEntry)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions { PickerTitle = "Select a file" });
            if (result != null && !string.IsNullOrEmpty(_dataService.WebsiteRootPath))
            {
                // Determine if it's an image or other file
                var ext = System.IO.Path.GetExtension(result.FileName).ToLowerInvariant();
                string? relativePath;
                if (ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".svg" or ".webp" or ".bmp")
                    relativePath = await _dataService.CopyImageToWebsiteAsync(result.FullPath);
                else
                    relativePath = await _dataService.CopyMediaFileToWebsiteAsync(result.FullPath);

                entry.FileUrl = relativePath;
                urlEntry.Text = relativePath ?? string.Empty;
                StatusLabel.Text = $"Copied: {result.FileName}";
            }
            else if (result != null)
            {
                urlEntry.Text = result.FullPath;
                StatusLabel.Text = "File selected (open website folder first to copy)";
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    private void OnAddEntry(object? sender, EventArgs e)
    {
        _dataService.SiteConfig.Win95VfsEntries.Add(new Win95VfsEntry { Path = @"C:\", IsDir = true });
        RebuildUI();
    }

    private async void OnOpenFolder(object? sender, EventArgs e)
    {
        try
        {
            var result = await FolderPicker.Default.PickAsync(default);
            if (result.IsSuccessful)
            {
                var loaded = await _dataService.LoadFromFolderAsync(result.Folder.Path);
                if (loaded) { RebuildUI(); StatusLabel.Text = $"Loaded from: {result.Folder.Path}"; }
                else StatusLabel.Text = "No Website/wwwroot/data folder found.";
            }
        }
        catch (Exception ex) { StatusLabel.Text = $"Error: {ex.Message}"; }
    }

    private async void OnSaveAll(object? sender, EventArgs e)
    {
        try { await _dataService.SaveAllAsync(); StatusLabel.Text = "Saved successfully!"; }
        catch (Exception ex) { StatusLabel.Text = $"Save failed: {ex.Message}"; }
    }
}
