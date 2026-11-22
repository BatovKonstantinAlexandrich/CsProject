using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    public class SеtRoyaltyCashSummsEditRequest
    {
        [Required]
        public int year { get; set; }

        [Required]
        public int month { get; set; }

        [Required]
        public bool not_edit { get; set; }

        [Required]
        public List<int> division_ids { get; set; } = new();
    }
}
