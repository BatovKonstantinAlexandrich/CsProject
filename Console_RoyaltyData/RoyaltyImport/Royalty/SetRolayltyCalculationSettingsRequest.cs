using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    public class SetRolayltyCalculationSettingsRequest
    {
        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }

        /// <summary>Скидка за закупки у ЕА.</summary>
        [Required]
        public float? discount_for_buy { get; set; }

        /// <summary>Скидка за закупки у сторонних поставщиков.</summary>
        [Required]
        public float? discount_for_buy_other { get; set;  }

        [Required]
        public float? discount_loyalty { get; set; }

        [Required]            
        public List<int> division_ids { get; set; } = new();
    }
}
