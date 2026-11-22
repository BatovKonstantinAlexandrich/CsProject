using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Report : BaseEntity
    {
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public decimal WorksAmount { get; set; }
        public decimal PartsAmount { get; set; }
        public decimal PurchaseEa { get; set; }
        public decimal PurchaseOthers { get; set; }
        public int CountRunOrders { get; set; }
        public int CountPretentionOrders { get; set; }
    }
}
