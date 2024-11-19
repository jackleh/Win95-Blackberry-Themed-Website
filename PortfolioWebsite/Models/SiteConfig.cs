namespace PortfolioWebsite.Models;

public class SiteConfig
{
    public int OpeningSectionHoldDurationInMs { get; set; }

    private int _openingSectionFadeDurationInMs { get; set; }
    [Limitation("Only works if 1k,1.5k,2k,2.5k,3k from JSON due to how CSS struggles with dynamic values")]
    public int OpeningSectionFadeDurationInMs
    {
        get => _openingSectionFadeDurationInMs;
        set
        {
           int[] allowedValues = [1000, 1500, 2000, 2500, 3000];
           _openingSectionFadeDurationInMs = allowedValues.Contains(value) 
               ? value 
               : allowedValues.OrderBy(x => Math.Abs(x - value)).First();
        }
    }
    public string SiteTitle { get; set; }
}