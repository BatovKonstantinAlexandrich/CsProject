using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Organization : NamedObject
    {
        public string Inn { get; set; }
        public string Ogrn { get; set; }
        public string Kpp { get; internal set; }
    }
}
