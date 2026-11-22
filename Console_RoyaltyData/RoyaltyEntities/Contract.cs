using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Contract : BaseEntity
    {
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string ContractNumber { get; set; }
        public DateTime ContractDate { get; set; }
    }
}
