using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    public class CreateAndGetRoyaltyReportRequest
    {
        [Required]
        public List<int> div_ids { get; set; }

        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }
    }
}
