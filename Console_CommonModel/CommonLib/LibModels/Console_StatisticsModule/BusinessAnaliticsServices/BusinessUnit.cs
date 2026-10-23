using System.Collections.Generic;
using System.Linq;

namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessAnaliticsServices
{
    [Label("Statistics: CRUD BusinessIndicator через EntityFasade")]
    [Description("Создание и чтение BusinessIndicator в BusinessDbContext с выводом JSON в консоль.")]
    public class BusinessUnit : TestingElement
    {
        public override void OnTest()
        {
            LogMessage("EntityFasade<BusinessIndicator>: Create → Get → Update → Remove → Search");
            BusinessIndicator p = null;
            using (var db = new BusinessDbContext())
            {
                var fasade = new EntityFasade<BusinessIndicator>(null);
                LogMessage("Create BusinessIndicator Name=A");
                int id = fasade.Create(p = new BusinessIndicator()
                {
                    Name = "A"
                });
                IEnumerable<BusinessIndicator> indicators = fasade.Get(p.Id);
                LogMessage($"Get по Id={p.Id}: записей {indicators.Count()}");
                indicators.ToJsonOnScreen().WriteToConsole();
                var indicator = indicators.ToList().First();
                indicator.Description = "test";
                LogMessage("Update Description=test");
                fasade.Update(indicator);
                LogMessage($"Remove Id={id}");
                fasade.Remove(id);
                LogMessage("Search \" A \"");
                fasade.Search(" A ");
                LogMessage("CRUD BusinessIndicator завершён");

            }
        }
    }
}