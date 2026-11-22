namespace WebApiBuhReport.Models.Royalty
{

    public class RoyaltyExecuteRequest
    {
        public string franchiseType { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public List<DepartmentInfo> departments { get; set; }
    }
}
