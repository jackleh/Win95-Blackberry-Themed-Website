using System.Text.Json;
using WebsiteBuilder.Models;

namespace WebsiteBuilder.Services;

public class WebsiteDataService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string? WebsiteRootPath { get; set; }

    public string DataFolderPath => System.IO.Path.Combine(WebsiteRootPath ?? string.Empty, "Website", "wwwroot", "data");
    public string MediaFolderPath => System.IO.Path.Combine(WebsiteRootPath ?? string.Empty, "Website", "wwwroot", "media");
    public string ImageFolderPath => System.IO.Path.Combine(MediaFolderPath, "image");

    public Person Person { get; set; } = new();
    public AboutMe AboutMe { get; set; } = new();
    public Contact Contact { get; set; } = new();
    public ProjectsInfo ProjectsInfo { get; set; } = new();
    public Resume Resume { get; set; } = new();
    public SiteConfig SiteConfig { get; set; } = new();

    public async Task<bool> LoadFromFolderAsync(string rootPath)
    {
        WebsiteRootPath = rootPath;

        if (!Directory.Exists(DataFolderPath))
            return false;

        Person = await LoadJsonAsync<Person>("person.json") ?? new();
        AboutMe = await LoadJsonAsync<AboutMe>("aboutMe.json") ?? new();
        Contact = await LoadJsonAsync<Contact>("contact.json") ?? new();
        ProjectsInfo = await LoadJsonAsync<ProjectsInfo>("projects.json") ?? new();
        Resume = await LoadJsonAsync<Resume>("resume.json") ?? new();
        SiteConfig = await LoadJsonAsync<SiteConfig>("siteConfig.json") ?? new();

        return true;
    }

    public async Task SaveAllAsync()
    {
        if (string.IsNullOrEmpty(WebsiteRootPath))
            return;

        Directory.CreateDirectory(DataFolderPath);

        await SaveJsonAsync("person.json", Person);
        await SaveJsonAsync("aboutMe.json", AboutMe);
        await SaveJsonAsync("contact.json", Contact);
        await SaveJsonAsync("projects.json", ProjectsInfo);
        await SaveJsonAsync("resume.json", Resume);
        await SaveJsonAsync("siteConfig.json", SiteConfig);
    }

    public async Task<string?> CopyImageToWebsiteAsync(string sourceFilePath)
    {
        Directory.CreateDirectory(ImageFolderPath);
        var fileName = System.IO.Path.GetFileName(sourceFilePath);
        var destPath = System.IO.Path.Combine(ImageFolderPath, fileName);
        await Task.Run(() => File.Copy(sourceFilePath, destPath, overwrite: true));
        return $"media/image/{fileName}";
    }

    public async Task<string?> CopyMediaFileToWebsiteAsync(string sourceFilePath)
    {
        Directory.CreateDirectory(MediaFolderPath);
        var fileName = System.IO.Path.GetFileName(sourceFilePath);
        var destPath = System.IO.Path.Combine(MediaFolderPath, fileName);
        await Task.Run(() => File.Copy(sourceFilePath, destPath, overwrite: true));
        return $"media/{fileName}";
    }

    private async Task<T?> LoadJsonAsync<T>(string fileName)
    {
        var path = System.IO.Path.Combine(DataFolderPath, fileName);
        if (!File.Exists(path))
            return default;

        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<T>(json, ReadOptions);
    }

    private async Task SaveJsonAsync<T>(string fileName, T data)
    {
        var path = System.IO.Path.Combine(DataFolderPath, fileName);
        var json = JsonSerializer.Serialize(data, JsonOptions);
        await File.WriteAllTextAsync(path, json);
    }
}
