using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Department: NamedObject
    {
        public string Address { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public int DivisionId { get; set; }
        public int OwnerId { get; set; }
        /// <summary>Тип подразделения из v_base_obj_divisions_all.division_type.</summary>
        public string DivisionType { get; set; } = "";
        /// <summary>Внутренняя франшиза EA: префикс EA_ или euroauto в типе подразделения.</summary>
        public bool IsEa { get; set; }
    }
}
