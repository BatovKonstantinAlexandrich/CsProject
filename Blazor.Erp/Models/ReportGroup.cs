namespace BlazorErp.Models;

public class ReportGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ReportProcedure> Reports { get; set; } = [];
}
