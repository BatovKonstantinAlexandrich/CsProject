namespace WebApiBuhReport.Models.Royalty
{
    /// <summary>
    /// Суммы закупок EA и сторонних (RoyaltyReportSelect @code=5).
    /// </summary>
    public class RoyaltyPurSummsItem
    {
        public int division_id { get; internal set; }
        public double sum_pur_ea { get; internal set; }
        public double sum_pur_other { get; internal set; }
    }
}
