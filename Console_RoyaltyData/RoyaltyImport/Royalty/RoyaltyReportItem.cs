namespace WebApiBuhReport.Models.Royalty
{
    public class RoyaltyReportItem
    {
        public int division_id { get; internal set; }
        public string? division_name { get; internal set; }
        public string? owner_name { get; internal set; }
        public double? koef_roylty { get; internal set; }
        public double? koef_calc { get; internal set; }
        public double? nds_st { get; internal set; }
        /// <summary>Совокупный оборот.</summary>
        public double? sum_proceeds { get; internal set; }
        public double sum_work_buh { get; internal set; }
        public double sum_part_buh { get; internal set; }
        /// <summary>Затраты на закупку товаров у ЕА.</summary>
        public double? sum_pur_ea { get; internal set; }
        /// <summary>Затраты на закупку товаров у сторонних поставщиков.</summary>
        public double? sum_pur_other { get; internal set; }
        /// <summary>Закупки EA из royaltyData. Для доли закупок.</summary>
        public double? sum_buy_ea { get; internal set; }
        /// <summary>Закупки сторонних из royaltyData. Для доли закупок.</summary>
        public double? sum_buy_other { get; internal set; }
        /// <summary>Кол-во заказ-нарядов.</summary>
        public int? count_run { get; internal set; }
        /// <summary>Количество обоснованных претензий (не доля).</summary>
        public double? ratio_pretensions { get; internal set; }
        public double? ratio_pur_ea { get; internal set; }
        public double? sum_roylty { get; internal set; }
        public double? sum_roylty_nds { get; internal set; }
        public double? sum_smb { get; internal set; }
        public double? roylty_calc { get; internal set; }
        public double? index_loality { get; internal set; }
        public double? discount_for_buy { get; internal set; }
        public double? discount_for_buy_other { get; internal set; }
        public double? discount_loyalty { get; internal set; }
        public double? K2 { get; internal set; }
        public double? K3 { get; internal set; }
        public double? K4 { get; internal set; }
    }
}
