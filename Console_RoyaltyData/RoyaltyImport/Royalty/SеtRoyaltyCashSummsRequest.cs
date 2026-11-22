using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{    
    public class SеtRoyaltyCashSummsRequest
    {
        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }

        [Required]
        public int? div_id { get; set; }

        [Required]
        public float? sum_work_buh { get; set; }

        [Required]
        public float? sum_part_buh { get; set; }
    }    
}
