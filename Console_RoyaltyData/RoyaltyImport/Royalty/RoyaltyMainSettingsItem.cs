namespace WebApiBuhReport.Models.Royalty
{
    public class RoyaltyMainSettingsItem
    {
        public int division_id { get; internal set; }
        public string division_name { get; internal set; }
        public string owner_name { get; internal set; }
        public double? koef_roylty { get; internal set; }
        public double? koef_calc { get; internal set; }
        public double? nds_st { get; internal set; }
    }
}
