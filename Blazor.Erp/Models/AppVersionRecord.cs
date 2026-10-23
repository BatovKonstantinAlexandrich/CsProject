namespace BlazorErp.Models;

public class AppVersionRecord
{
    public int Id { get; set; } = 1;
    public int ReleaseCount { get; set; } = 1;
    public int BuildNumber { get; set; }
    public int PassedTests { get; set; }
    public int TotalTests { get; set; }
    public DateTime LastRunAt { get; set; }
    public string LastRunDetails { get; set; } = "[]";
}
