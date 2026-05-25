using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class SiteConfigPage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public SiteConfigPage(WebsiteDataService dataService)
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
        var c = _dataService.SiteConfig;
        SiteTitleEntry.Text = c.SiteTitle;
        Win95EnabledSwitch.IsToggled = c.Win95Enabled;
        DonateLinkEntry.Text = c.DonateLink;
        DonateTextEntry.Text = c.DonateText;

        OsNameEntry.Text = c.Win95OsName;
        OsVersionEntry.Text = c.Win95OsVersion;
        OsCopyrightEntry.Text = c.Win95OsCopyright;
        OsDirNameEntry.Text = c.Win95OsDirName;

        PaintExeEntry.Text = c.Win95PaintExeName;
        PaintDisplayEntry.Text = c.Win95PaintDisplayName;
        NotepadExeEntry.Text = c.Win95NotepadExeName;
        NotepadDisplayEntry.Text = c.Win95NotepadDisplayName;
        CmdExeEntry.Text = c.Win95CmdExeName;
        ResumeFileEntry.Text = c.Win95ResumeFileName;
        ProjectsFileEntry.Text = c.Win95ProjectsFileName;
        ContactFileEntry.Text = c.Win95ContactFileName;

        Video1UrlEntry.Text = c.Win95Video1Url;
        Video1TitleEntry.Text = c.Win95Video1Title;
        Video2UrlEntry.Text = c.Win95Video2Url;
        Video2TitleEntry.Text = c.Win95Video2Title;
        Video3UrlEntry.Text = c.Win95Video3Url;
        Video3TitleEntry.Text = c.Win95Video3Title;

        DesktopPicEntry.Text = c.Win95DesktopPictureUrl;
        PaintInitImageEntry.Text = c.Win95PaintInitImageUrl;

        UpdateImagePreviews();
    }

    private void SaveFields()
    {
        var c = _dataService.SiteConfig;
        c.SiteTitle = SiteTitleEntry.Text ?? string.Empty;
        c.Win95Enabled = Win95EnabledSwitch.IsToggled;
        c.DonateLink = DonateLinkEntry.Text ?? string.Empty;
        c.DonateText = DonateTextEntry.Text ?? string.Empty;

        c.Win95OsName = OsNameEntry.Text ?? string.Empty;
        c.Win95OsVersion = OsVersionEntry.Text ?? string.Empty;
        c.Win95OsCopyright = OsCopyrightEntry.Text ?? string.Empty;
        c.Win95OsDirName = OsDirNameEntry.Text ?? string.Empty;

        c.Win95PaintExeName = PaintExeEntry.Text ?? string.Empty;
        c.Win95PaintDisplayName = PaintDisplayEntry.Text ?? string.Empty;
        c.Win95NotepadExeName = NotepadExeEntry.Text ?? string.Empty;
        c.Win95NotepadDisplayName = NotepadDisplayEntry.Text ?? string.Empty;
        c.Win95CmdExeName = CmdExeEntry.Text ?? string.Empty;
        c.Win95ResumeFileName = ResumeFileEntry.Text ?? string.Empty;
        c.Win95ProjectsFileName = ProjectsFileEntry.Text ?? string.Empty;
        c.Win95ContactFileName = ContactFileEntry.Text ?? string.Empty;

        c.Win95Video1Url = Video1UrlEntry.Text ?? string.Empty;
        c.Win95Video1Title = Video1TitleEntry.Text ?? string.Empty;
        c.Win95Video2Url = Video2UrlEntry.Text ?? string.Empty;
        c.Win95Video2Title = Video2TitleEntry.Text ?? string.Empty;
        c.Win95Video3Url = Video3UrlEntry.Text ?? string.Empty;
        c.Win95Video3Title = Video3TitleEntry.Text ?? string.Empty;

        c.Win95DesktopPictureUrl = DesktopPicEntry.Text ?? string.Empty;
        c.Win95PaintInitImageUrl = PaintInitImageEntry.Text ?? string.Empty;
    }

    private void UpdateImagePreviews()
    {
        if (!string.IsNullOrEmpty(_dataService.WebsiteRootPath))
        {
            TrySetImagePreview(DesktopPicPreview, DesktopPicEntry.Text);
            TrySetImagePreview(PaintInitPreview, PaintInitImageEntry.Text);
        }
    }

    private void TrySetImagePreview(Image imageControl, string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath) || string.IsNullOrEmpty(_dataService.WebsiteRootPath))
        {
            imageControl.Source = null;
            return;
        }

        var fullPath = Path.Combine(_dataService.WebsiteRootPath, "Website", "wwwroot", relativePath);
        if (File.Exists(fullPath))
            imageControl.Source = ImageSource.FromFile(fullPath);
        else
            imageControl.Source = null;
    }

    // ── Image/Video pickers ────────────────────────────────

    private async void OnPickDesktopPicture(object? sender, EventArgs e) =>
        await PickAndCopyImage(DesktopPicEntry, DesktopPicPreview);

    private async void OnPickPaintImage(object? sender, EventArgs e) =>
        await PickAndCopyImage(PaintInitImageEntry, PaintInitPreview);

    private async Task PickAndCopyImage(Entry targetEntry, Image? preview)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select an image",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                if (!string.IsNullOrEmpty(_dataService.WebsiteRootPath))
                {
                    var relativePath = await _dataService.CopyImageToWebsiteAsync(result.FullPath);
                    targetEntry.Text = relativePath;
                    StatusLabel.Text = $"Copied: {result.FileName}";
                }
                else
                {
                    targetEntry.Text = result.FullPath;
                    StatusLabel.Text = "Image selected (open website folder first to copy into site)";
                }

                if (preview != null)
                    preview.Source = ImageSource.FromFile(result.FullPath);
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    private async void OnPickVideo1(object? sender, EventArgs e) => await PickVideo(Video1UrlEntry, Video1TitleEntry);
    private async void OnPickVideo2(object? sender, EventArgs e) => await PickVideo(Video2UrlEntry, Video2TitleEntry);
    private async void OnPickVideo3(object? sender, EventArgs e) => await PickVideo(Video3UrlEntry, Video3TitleEntry);

    private async Task PickVideo(Entry urlEntry, Entry titleEntry)
    {
        try
        {
            var customVideoType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".mp4", ".webm", ".avi", ".mkv", ".mov" } },
                { DevicePlatform.macOS, new[] { "public.movie" } },
                { DevicePlatform.MacCatalyst, new[] { "public.movie" } }
            });

            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select a video file",
                FileTypes = customVideoType
            });

            if (result != null)
            {
                if (!string.IsNullOrEmpty(_dataService.WebsiteRootPath))
                {
                    var relativePath = await _dataService.CopyMediaFileToWebsiteAsync(result.FullPath);
                    urlEntry.Text = relativePath;
                }
                else
                {
                    urlEntry.Text = result.FullPath;
                }

                titleEntry.Text = result.FileName;
                StatusLabel.Text = $"Video selected: {result.FileName}";
            }
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    // ── Open / Save ────────────────────────────────────────

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
                    if (Shell.Current is AppShell appShell)
                        appShell.UpdateDonateButtonText();
                    StatusLabel.Text = $"Loaded from: {result.Folder.Path}";
                }
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
