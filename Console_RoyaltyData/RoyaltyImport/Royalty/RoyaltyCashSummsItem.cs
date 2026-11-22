namespace WebApiBuhReport.Models.Royalty
{
    public class RoyaltyCashSummsItem
    {
        public int division_id { get; internal set; }
        public string division_name { get; internal set; }
        public string owner_name { get; internal set; }
        public double sum_work_buh { get; internal set; }
        public double sum_part_buh { get; internal set; }
        public bool not_edit { get; internal set; }
    }
}
