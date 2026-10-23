namespace BlazorErp.Models;

public class ReportColumn
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public ReportProcedure Report { get; set; } = null!;
    public string ColumnName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Format { get; set; }
    public bool ShowSum { get; set; }
    public int SortOrder { get; set; }
}
