using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    public class SetRolayltyMainSettingsRequest
    {
        [Required]
        public float? koef_calc { get; set; }

        [Required]
        public float? nds_st { get; set; }

        [Required]
        public float? koef_roylty { get; set; }

        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }

        [Required]
        public List<int> division_ids { get; set; }
    }
}
