using WebsiteBuilder.Models;
using WebsiteBuilder.Services;

namespace WebsiteBuilder.Pages;

public partial class ResumePage : ContentPage
{
    private readonly WebsiteDataService _dataService;

    public ResumePage(WebsiteDataService dataService)
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
        var r = _dataService.Resume;
        TaglineEntry.Text = r.Tagline;
        EmailEntry.Text = r.Email;
        PhoneEntry.Text = r.Phone;
        LocationEntry.Text = r.Location;
        AboutEditor.Text = r.About;

        RebuildCompanies();
        RebuildSkillGroups();
        RebuildEducation();
    }

    private void SaveBasicFields()
    {
        var r = _dataService.Resume;
        r.Tagline = TaglineEntry.Text ?? string.Empty;
        r.Email = EmailEntry.Text ?? string.Empty;
        r.Phone = PhoneEntry.Text ?? string.Empty;
        r.Location = LocationEntry.Text ?? string.Empty;
        r.About = AboutEditor.Text ?? string.Empty;
    }

    // ── Companies & Roles ──────────────────────────────────
    private void RebuildCompanies()
    {
        CompaniesList.Children.Clear();
        for (int i = 0; i < _dataService.Resume.Companies.Count; i++)
        {
            var company = _dataService.Resume.Companies[i];
            var ci = i;
            var frame = new Frame { BorderColor = Colors.LightGray, Padding = 16, CornerRadius = 8, HasShadow = false };
            var layout = new VerticalStackLayout { Spacing = 10 };

            layout.Children.Add(new Label { Text = $"Company #{ci + 1}", FontSize = 16, FontAttributes = FontAttributes.Bold });

            layout.Children.Add(new Label { Text = "Company Name", FontAttributes = FontAttributes.Bold });
            var nameEntry = new Entry { Text = company.Name, Placeholder = "Company — Location" };
            nameEntry.TextChanged += (_, e) => company.Name = e.NewTextValue ?? string.Empty;
            layout.Children.Add(nameEntry);

            // Roles
            layout.Children.Add(new Label { Text = "Roles", FontSize = 15, FontAttributes = FontAttributes.Bold });
            var addRoleBtn = new Button { Text = "+ Add Role", BackgroundColor = Color.FromArgb("#3E8EED"), FontSize = 12 };
            addRoleBtn.Clicked += (_, _) => { company.Roles.Add(new Resume.Role()); RebuildCompanies(); };
            layout.Children.Add(addRoleBtn);

            for (int j = 0; j < company.Roles.Count; j++)
            {
                var role = company.Roles[j];
                var rj = j;
                var roleFrame = new Frame { BorderColor = Colors.CornflowerBlue, Padding = 12, CornerRadius = 4, HasShadow = false };
                var roleLayout = new VerticalStackLayout { Spacing = 8 };

                roleLayout.Children.Add(new Label { Text = $"Role #{rj + 1}", FontAttributes = FontAttributes.Bold });

                roleLayout.Children.Add(new Label { Text = "Title" });
                var titleEntry = new Entry { Text = role.Title, Placeholder = "Job Title" };
                titleEntry.TextChanged += (_, e) => role.Title = e.NewTextValue ?? string.Empty;
                roleLayout.Children.Add(titleEntry);

                roleLayout.Children.Add(new Label { Text = "Dates" });
                var datesEntry = new Entry { Text = role.Dates, Placeholder = "2024 – Present" };
                datesEntry.TextChanged += (_, e) => role.Dates = e.NewTextValue ?? string.Empty;
                roleLayout.Children.Add(datesEntry);

                roleLayout.Children.Add(new Label { Text = "Bullet Points (one per line)" });
                var bulletsEditor = new Editor
                {
                    Text = string.Join("\n", role.Bullets),
                    Placeholder = "Accomplishment 1\nAccomplishment 2",
                    AutoSize = EditorAutoSizeOption.TextChanges,
                    MinimumHeightRequest = 60
                };
                bulletsEditor.TextChanged += (_, e) => role.Bullets = ParseLines(e.NewTextValue);
                roleLayout.Children.Add(bulletsEditor);

                var removeRoleBtn = new Button { Text = "Remove Role", BackgroundColor = Colors.OrangeRed, FontSize = 12 };
                removeRoleBtn.Clicked += (_, _) => { company.Roles.RemoveAt(rj); RebuildCompanies(); };
                roleLayout.Children.Add(removeRoleBtn);

                roleFrame.Content = roleLayout;
                layout.Children.Add(roleFrame);
            }

            var removeCompanyBtn = new Button { Text = "Remove Company", BackgroundColor = Colors.Red };
            removeCompanyBtn.Clicked += (_, _) => { _dataService.Resume.Companies.RemoveAt(ci); RebuildCompanies(); };
            layout.Children.Add(removeCompanyBtn);

            frame.Content = layout;
            CompaniesList.Children.Add(frame);
        }
    }

    // ── Skill Groups ───────────────────────────────────────
    private void RebuildSkillGroups()
    {
        SkillGroupsList.Children.Clear();
        for (int i = 0; i < _dataService.Resume.SkillGroups.Count; i++)
        {
            var group = _dataService.Resume.SkillGroups[i];
            var gi = i;
            var frame = new Frame { BorderColor = Colors.LightGray, Padding = 16, CornerRadius = 8, HasShadow = false };
            var layout = new VerticalStackLayout { Spacing = 10 };

            layout.Children.Add(new Label { Text = $"Skill Group #{gi + 1}", FontSize = 16, FontAttributes = FontAttributes.Bold });

            layout.Children.Add(new Label { Text = "Heading", FontAttributes = FontAttributes.Bold });
            var headingEntry = new Entry { Text = group.Heading, Placeholder = "Group Heading" };
            headingEntry.TextChanged += (_, e) => group.Heading = e.NewTextValue ?? string.Empty;
            layout.Children.Add(headingEntry);

            layout.Children.Add(new Label { Text = "Skills", FontSize = 15, FontAttributes = FontAttributes.Bold });
            var addSkillBtn = new Button { Text = "+ Add Skill Item", BackgroundColor = Color.FromArgb("#3E8EED"), FontSize = 12 };
            addSkillBtn.Clicked += (_, _) => { group.Items.Add(new Resume.SkillItem()); RebuildSkillGroups(); };
            layout.Children.Add(addSkillBtn);

            for (int j = 0; j < group.Items.Count; j++)
            {
                var item = group.Items[j];
                var sj = j;
                var itemLayout = new HorizontalStackLayout { Spacing = 8 };
                var labelEntry = new Entry { Text = item.Label, Placeholder = "Skill Label", HorizontalOptions = LayoutOptions.FillAndExpand };
                labelEntry.TextChanged += (_, e) => item.Label = e.NewTextValue ?? string.Empty;
                var detailEntry = new Entry { Text = item.Detail, Placeholder = "Detail", HorizontalOptions = LayoutOptions.FillAndExpand };
                detailEntry.TextChanged += (_, e) => item.Detail = e.NewTextValue ?? string.Empty;
                var removeSkillBtn = new Button { Text = "X", BackgroundColor = Colors.Red, FontSize = 12, WidthRequest = 40 };
                removeSkillBtn.Clicked += (_, _) => { group.Items.RemoveAt(sj); RebuildSkillGroups(); };

                itemLayout.Children.Add(labelEntry);
                itemLayout.Children.Add(detailEntry);
                itemLayout.Children.Add(removeSkillBtn);
                layout.Children.Add(itemLayout);
            }

            var removeGroupBtn = new Button { Text = "Remove Skill Group", BackgroundColor = Colors.Red };
            removeGroupBtn.Clicked += (_, _) => { _dataService.Resume.SkillGroups.RemoveAt(gi); RebuildSkillGroups(); };
            layout.Children.Add(removeGroupBtn);

            frame.Content = layout;
            SkillGroupsList.Children.Add(frame);
        }
    }

    // ── Education ──────────────────────────────────────────
    private void RebuildEducation()
    {
        EducationList.Children.Clear();
        for (int i = 0; i < _dataService.Resume.Education.Count; i++)
        {
            var edu = _dataService.Resume.Education[i];
            var ei = i;
            var frame = new Frame { BorderColor = Colors.LightGray, Padding = 16, CornerRadius = 8, HasShadow = false };
            var layout = new VerticalStackLayout { Spacing = 10 };

            layout.Children.Add(new Label { Text = $"Education #{ei + 1}", FontSize = 16, FontAttributes = FontAttributes.Bold });

            layout.Children.Add(new Label { Text = "School", FontAttributes = FontAttributes.Bold });
            var schoolEntry = new Entry { Text = edu.School, Placeholder = "School Name" };
            schoolEntry.TextChanged += (_, e) => edu.School = e.NewTextValue ?? string.Empty;
            layout.Children.Add(schoolEntry);

            layout.Children.Add(new Label { Text = "Degree", FontAttributes = FontAttributes.Bold });
            var degreeEntry = new Entry { Text = edu.Degree, Placeholder = "AAS in Computer Science" };
            degreeEntry.TextChanged += (_, e) => edu.Degree = e.NewTextValue ?? string.Empty;
            layout.Children.Add(degreeEntry);

            layout.Children.Add(new Label { Text = "Year", FontAttributes = FontAttributes.Bold });
            var yearEntry = new Entry { Text = edu.Year, Placeholder = "2020" };
            yearEntry.TextChanged += (_, e) => edu.Year = e.NewTextValue ?? string.Empty;
            layout.Children.Add(yearEntry);

            var removeBtn = new Button { Text = "Remove Education", BackgroundColor = Colors.Red };
            removeBtn.Clicked += (_, _) => { _dataService.Resume.Education.RemoveAt(ei); RebuildEducation(); };
            layout.Children.Add(removeBtn);

            frame.Content = layout;
            EducationList.Children.Add(frame);
        }
    }

    private void OnAddCompany(object? sender, EventArgs e)
    {
        _dataService.Resume.Companies.Add(new Resume.Company());
        RebuildCompanies();
    }

    private void OnAddSkillGroup(object? sender, EventArgs e)
    {
        _dataService.Resume.SkillGroups.Add(new Resume.SkillGroup());
        RebuildSkillGroups();
    }

    private void OnAddEducation(object? sender, EventArgs e)
    {
        _dataService.Resume.Education.Add(new Resume.EducationEntry());
        RebuildEducation();
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
        SaveBasicFields();
        try { await _dataService.SaveAllAsync(); StatusLabel.Text = "Saved successfully!"; }
        catch (Exception ex) { StatusLabel.Text = $"Save failed: {ex.Message}"; }
    }
}
