namespace WebApiBuhReport.Models.Royalty
{
    public class RoyaltyCalculationSettingsItem : RoyaltyMainSettingsItem
    {
        /// <summary>Скидка за закупки у ЕА.</summary>
        public double? discount_for_buy { get; internal set; }
        /// <summary>Скидка за закупки у сторонних поставщиков.</summary>
        public double? discount_for_buy_other { get; internal set; }
        public double? discount_loyalty { get; internal set; }
    }
}

