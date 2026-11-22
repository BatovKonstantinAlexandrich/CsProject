using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Invoce : BaseEntity
    {
        public int ContractId { get; set; }
        public Contract Contract { get; set; }
        public int PaymentSumm { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
