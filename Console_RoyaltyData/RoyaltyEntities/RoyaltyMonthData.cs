using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    /// <summary>
    /// Результат расчёта роялти за месяц и суммы, введённые бухгалтером.
    /// Аналог строки webapibuhreport.Royalty_data.
    /// </summary>
    public class RoyaltyMonthData : BaseEntity
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public int OwnerId { get; set; }
        public decimal KoefRoyalty { get; set; }
        public decimal KoefCalc { get; set; }
        public decimal NdsSt { get; set; }
        public decimal SumProceeds { get; set; }
        public decimal SumPurEa { get; set; }
        public decimal SumPurOther { get; set; }
        public decimal SumBuyEa { get; set; }
        public decimal SumBuyOther { get; set; }
        public decimal RatioPurEa { get; set; }
        /// <summary>Количество обоснованных претензий, не доля.</summary>
        public decimal RatioPretensions { get; set; }
        public decimal SumRoyalty { get; set; }
        public decimal SumRoyaltyNds { get; set; }
        public decimal SumSmb { get; set; }
        public decimal SumWorkBuh { get; set; }
        public decimal SumPartBuh { get; set; }
        public int CountRun { get; set; }
        public decimal IndexLoyalty { get; set; }
        public decimal K2 { get; set; }
        public decimal K3 { get; set; }
        public decimal K4 { get; set; }
        public decimal RoyaltyCalc { get; set; }
    }
}
