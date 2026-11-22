namespace WebApiBuhReport.Models.Royalty
{
    /// <summary>
    /// Строка RoyaltyDocumentSelect @code=1 (счёт).
    /// </summary>
    public class RoyaltyInvoiceDocumentItem
    {
        public int division_id { get; internal set; }
        public string? division_name { get; internal set; }
        public string? bank { get; internal set; }
        public string? bik { get; internal set; }
        public string? ks { get; internal set; }
        public string? rs { get; internal set; }
        public string? recipient_name { get; internal set; }
        public string? recipient_inn { get; internal set; }
        public string? recipient_kpp { get; internal set; }
        public string? recipient_address { get; internal set; }
        public string? recipient_phone { get; internal set; }
        public string? user_name { get; internal set; }
        public string? user_ogrn { get; internal set; }
        public string? user_inn { get; internal set; }
        public string? user_address { get; internal set; }
        public string? contract_num { get; internal set; }
        public DateTime? contract_date { get; internal set; }
        public string? invoice_num { get; internal set; }
        public DateTime? invoice_date { get; internal set; }
        public int period_month { get; internal set; }
        public int period_year { get; internal set; }
        public double sum_roylty { get; internal set; }
        public double nds_st { get; internal set; }
    }
}
