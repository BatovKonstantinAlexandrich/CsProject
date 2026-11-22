namespace WebApiBuhReport.Models.Royalty
{
    /// <summary>
    /// Строка RoyaltyDocumentSelect @code=2 (справка).
    /// </summary>
    public class RoyaltyCertificateDocumentItem
    {
        public int division_id { get; internal set; }
        public string? division_name { get; internal set; }
        public string? owner_name { get; internal set; }
        public string? owner_inn { get; internal set; }
        public string? org_name { get; internal set; }
        public string? org_inn { get; internal set; }
        public string? org_ogrn { get; internal set; }
        public string? org_address { get; internal set; }
        public string? contract_num { get; internal set; }
        public int period_month { get; internal set; }
        public int period_year { get; internal set; }
        public DateTime? cert_date { get; internal set; }
        public int cert_day { get; internal set; }
        public int cert_month { get; internal set; }
        public int cert_year { get; internal set; }
        public double sum_proceeds { get; internal set; }
        public double koef_roylty { get; internal set; }
        public double ratio_pur_ea { get; internal set; }
        public double? sum_buy_ea { get; internal set; }
        public double? sum_buy_other { get; internal set; }
        public double? sum_pur_ea { get; internal set; }
        public double? sum_pur_other { get; internal set; }
        public int count_run { get; internal set; }
        public int pretensions { get; internal set; }
        /// <summary>Количество обоснованных претензий (как в Royalty_data.ratio_pretensions).</summary>
        public double ratio_pretensions { get; internal set; }
        public double K2 { get; internal set; }
        public double K3 { get; internal set; }
        public double K4 { get; internal set; }
        public double sum_roylty { get; internal set; }
        public double nds_st { get; internal set; }
    }
}
