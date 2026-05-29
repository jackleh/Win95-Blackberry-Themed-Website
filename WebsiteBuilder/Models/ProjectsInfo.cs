namespace WebsiteBuilder.Models;

public class ProjectsInfo
{
    public List<Project> Projects { get; set; } = [];

    public class Project
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Technologies { get; set; } = [];
        public string Url { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
