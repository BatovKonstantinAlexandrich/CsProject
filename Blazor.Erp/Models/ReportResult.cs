using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class ReportResult
{
    public string ReportTitle { get; set; } = string.Empty;
    public ReportDiagramType DiagramType { get; set; }
    public string? ChartXField { get; set; }
    public string? ChartYField { get; set; }
    public List<ReportColumnInfo> Columns { get; set; } = [];
    public List<Dictionary<string, object?>> Rows { get; set; } = [];
}

public class ReportColumnInfo
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Format { get; set; }
    public bool ShowSum { get; set; }
}

public class ReportListItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public ReportDiagramType DiagramType { get; set; }
    public bool IsFavorite { get; set; }
}

public class ReportGroupItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
