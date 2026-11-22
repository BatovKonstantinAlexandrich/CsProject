using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    /// <summary>
    /// Коэффициенты расчёта за месяц. Аналог строки webapibuhreport.Royalty_calculation.
    /// </summary>
    public class RoyaltyCalculation : BaseEntity
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public decimal KoefRoyalty { get; set; }
        public decimal KoefCalc { get; set; }
        public decimal NdsSt { get; set; }
        public int OwnerId { get; set; }
        public decimal DiscountForBuy { get; set; }
        public decimal DiscountLoyalty { get; set; }
    }
}
