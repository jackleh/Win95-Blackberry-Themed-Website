namespace PortfolioWebsite.Models;

public class Resume
{
    public string TagLine { get; set; } = string.Empty;
    public List<JobEntry> JobHistory { get; set; } = [];
    public List<string> SkillsByDomain { get; set; } = [];
    public List<SkillDetail> SkillsBreakdown { get; set; } = [];
    public List<EducationEntry> Education { get; set; } = [];

    public class JobEntry
    {
        public string Company { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public List<Role> Roles { get; set; } = [];
    }

    public class Role
    {
        public string Title { get; set; } = string.Empty;
        public string DateRange { get; set; } = string.Empty;
        public List<string> Bullets { get; set; } = [];
    }

    public class SkillDetail
    {
        public string Name { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    public class EducationEntry
    {
        public string School { get; set; } = string.Empty;
        public string Degree { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
    }
}
