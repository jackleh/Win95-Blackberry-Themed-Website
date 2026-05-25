namespace WebsiteBuilder.Models;

public class Resume
{
    public string Tagline { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string About { get; set; } = string.Empty;
    public List<Company> Companies { get; set; } = [];
    public List<SkillGroup> SkillGroups { get; set; } = [];
    public List<EducationEntry> Education { get; set; } = [];

    public class Company
    {
        public string Name { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = [];
    }

    public class Role
    {
        public string Title { get; set; } = string.Empty;
        public string Dates { get; set; } = string.Empty;
        public List<string> Bullets { get; set; } = [];
    }

    public class SkillGroup
    {
        public string Heading { get; set; } = string.Empty;
        public List<SkillItem> Items { get; set; } = [];
    }

    public class SkillItem
    {
        public string Label { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    public class EducationEntry
    {
        public string School { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
}
