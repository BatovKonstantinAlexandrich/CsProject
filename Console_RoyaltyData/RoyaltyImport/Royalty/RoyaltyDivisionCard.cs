using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    /// <summary>
    /// Карточка подразделения
    /// </summary>
    public class RoyaltyDivisionCard
    {
        [Required]
        public int division_id { get; set; }

        [Required]
        public int owner_id { get; set; }

        public string owner_name { get; set; } = string.Empty;

        public string address_div { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;

        public string owner_inn { get; set; } = string.Empty;

        public string owner_kpp { get; set; } = string.Empty;

        public string owner_ogrn { get; set; } = string.Empty;

        public string contract_num { get; set; } = string.Empty;

        /// <summary>Дата договора для клиента, yyyy-MM-dd. В Royalty_division хранится строка дд.мм.гггг.</summary>
        public string contract_date { get; set; } = string.Empty;
    }
}
