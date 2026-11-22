using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{    
    public class CreateRoyaltyReportRequest
    {
        [Required]
        public int? div_id { get; set; }

        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }
    }    
}
