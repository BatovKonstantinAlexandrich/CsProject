using BlazorErp.Models.Enums;

namespace BlazorErp.Models;

public class ReportParameter
{
    public int Id { get; set; }
    public int ReportId { get; set; }
    public ReportProcedure Report { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public ReportParameterType Type { get; set; } = ReportParameterType.Enter;
    public string? DefaultValue { get; set; }
    public string? LookupValues { get; set; }
    public int SortOrder { get; set; }
}
