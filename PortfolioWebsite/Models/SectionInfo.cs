namespace PortfolioWebsite.Models;

public class SectionInfo
{
    public string SectionPrepend { get; set; }
    public string SectionSelection { get; set; }
    public List<Section>? Sections { get; set; }
    
    public class Section
    {
        public string Title { get; set; }
        public string NavUrl { get; set; }
    }
}

