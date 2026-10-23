namespace BlazorErp.Models;

public class ReportUserFavorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int ReportId { get; set; }
    public ReportProcedure Report { get; set; } = null!;
}
