namespace WebApiBuhReport.Models.Royalty
{
    /// <summary>
    /// Реквизиты получателя / правообладателя. Необязательны: процедура их отдаёт сама.
    /// </summary>
    public class RoyaltyIssuerRequisites
    {
        public string? bank { get; set; }
        public string? bik { get; set; }
        public string? ks { get; set; }
        public string? rs { get; set; }
        public string? recipient_name { get; set; }
        public string? recipient_inn { get; set; }
        public string? recipient_kpp { get; set; }
        public string? recipient_address { get; set; }
        public string? recipient_phone { get; set; }
        public string? owner_name { get; set; }
        public string? owner_inn { get; set; }
    }
}
