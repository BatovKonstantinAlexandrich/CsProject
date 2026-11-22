using System.ComponentModel.DataAnnotations;

namespace WebApiBuhReport.Models.Royalty
{
    public class RoyaltyDocumentRequest
    {
        [Required]
        public int? year { get; set; }

        [Required]
        public int? month { get; set; }

        /// <summary>
        /// Как в процедуре: "0", "613,621" или JSON {"divisions":[{"division_id":613}]}.
        /// Если задан division_ids — используется он.
        /// </summary>
        public string? division_id { get; set; }

        public List<int>? division_ids { get; set; }

        /// <summary>
        /// Реквизиты получателя / правообладателя с клиента. Если нет — берутся из процедуры.
        /// </summary>
        public RoyaltyIssuerRequisites? issuer { get; set; }

        /// <summary>
        /// Вторичный пользователь / ИП с клиента. Если нет — из процедуры.
        /// </summary>
        public RoyaltyDocumentCounterparty? user { get; set; }
    }

    public class RoyaltyDocumentCounterparty
    {
        public string? name { get; set; }
        public string? inn { get; set; }
        public string? ogrn { get; set; }
        public string? address { get; set; }
    }
}
