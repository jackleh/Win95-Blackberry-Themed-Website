using WebsiteBuilder.Models;
using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class ProjectsPage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public ProjectsPage(WebsiteDataService dataService)
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
        ProjectsList.Children.Clear();
        for (int i = 0; i < _dataService.ProjectsInfo.Projects.Count; i++)
        {
            var project = _dataService.ProjectsInfo.Projects[i];
            var index = i;

            var frame = new Frame
            {
                BorderColor = Color.FromArgb("#808080"),
                Padding = new Thickness(12),
                CornerRadius = 0,
                HasShadow = false,
                Content = BuildProjectEditor(project, index)
            };
            ProjectsList.Children.Add(frame);
        }
    }

    private View BuildProjectEditor(ProjectsInfo.Project project, int index)
    {
        var layout = new VerticalStackLayout { Spacing = 10 };

        layout.Children.Add(new Label { Text = $"Project #{index + 1}", FontSize = 16, FontAttributes = FontAttributes.Bold });

        layout.Children.Add(new Label { Text = "Name", FontAttributes = FontAttributes.Bold });
        var nameEntry = new Entry { Text = project.Name, Placeholder = "Project name" };
        nameEntry.TextChanged += (_, e) => project.Name = e.NewTextValue ?? string.Empty;
        layout.Children.Add(nameEntry);

        layout.Children.Add(new Label { Text = "Description", FontAttributes = FontAttributes.Bold });
        var descEditor = new Editor { Text = project.Description, Placeholder = "Project description...", AutoSize = EditorAutoSizeOption.TextChanges, MinimumHeightRequest = 60 };
        descEditor.TextChanged += (_, e) => project.Description = e.NewTextValue ?? string.Empty;
        layout.Children.Add(descEditor);

        layout.Children.Add(new Label { Text = "URL", FontAttributes = FontAttributes.Bold });
        var urlEntry = new Entry { Text = project.Url, Placeholder = "https://...", Keyboard = Keyboard.Url };
        urlEntry.TextChanged += (_, e) => project.Url = e.NewTextValue ?? string.Empty;
        layout.Children.Add(urlEntry);

        layout.Children.Add(new Label { Text = "Status", FontAttributes = FontAttributes.Bold });
        var statusEntry = new Entry { Text = project.Status, Placeholder = "active / completed / archived" };
        statusEntry.TextChanged += (_, e) => project.Status = e.NewTextValue ?? string.Empty;
        layout.Children.Add(statusEntry);

        layout.Children.Add(new Label { Text = "Technologies (one per line)", FontAttributes = FontAttributes.Bold });
        var techEditor = new Editor
        {
            Text = string.Join("\n", project.Technologies),
            Placeholder = "C#\nBlazor\n...",
            AutoSize = EditorAutoSizeOption.TextChanges,
            MinimumHeightRequest = 60
        };
        techEditor.TextChanged += (_, e) =>
        {
            project.Technologies = ParseLines(e.NewTextValue);
        };
        layout.Children.Add(techEditor);

        var removeBtn = new Button { Text = "Remove Project" };
        var capturedIndex = index;
        removeBtn.Clicked += (_, _) =>
        {
            _dataService.ProjectsInfo.Projects.RemoveAt(capturedIndex);
            RebuildUI();
        };
        layout.Children.Add(removeBtn);

        return layout;
    }

    private void OnAddProject(object? sender, EventArgs e)
    {
        _dataService.ProjectsInfo.Projects.Add(new ProjectsInfo.Project());
        RebuildUI();
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
