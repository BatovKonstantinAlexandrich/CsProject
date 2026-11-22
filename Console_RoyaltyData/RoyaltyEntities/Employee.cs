using Console_RoyaltyData.DataAbstraction;

namespace Console_RoyaltyData.RoyaltyEntities
{
    public class Employee: BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string AdLogin { get; set; }     
        
        public string GetFullName()
        {
            return $"{Name} {Patronymic} {Surname}";
        }
    }
    
}
