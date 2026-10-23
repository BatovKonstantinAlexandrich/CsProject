using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class ReportProcedure
{
    public int Id { get; set; }
    public int GroupId { get; set; }
    public ReportGroup Group { get; set; } = null!;
    public string ProcedureName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ReportDiagramType DiagramType { get; set; } = ReportDiagramType.None;
    public string? ChartXField { get; set; }
    public string? ChartYField { get; set; }
    public bool IsHidden { get; set; }
    public List<ReportParameter> Parameters { get; set; } = new();
    public List<ReportColumn> Columns { get; set; } = new();
    public List<ReportUserFavorite> Favorites { get; set; } = new();
}
