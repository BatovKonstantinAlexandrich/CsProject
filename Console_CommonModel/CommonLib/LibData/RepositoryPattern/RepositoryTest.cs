using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;

namespace Console_CommonModel.CommonLib.LibData.RepositoryPattern
{
    public class RepositoryTest : TestingElement
    {
        public RepositoryTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            
            using (var db = new DeliveryDbContext())
            {
                var initiallizer = new DeliveryDbContextInitiallizer();
                initiallizer.Init(db);
            }
                           
            var repository = new Repository<Product, DeliveryDbContext>();
            var products = repository.Search("");
            var options = repository.GetRepositoryOptions("d");
            TestInputFilters();
        }

        private static void TestInputFilters()
        {
            var repository = new Repository<Product, DeliveryDbContext>();
            var filtersModel = repository.GetFilters();
            repository.Info(filtersModel.ToJsonOnScreen());
            var filters = new InputSearchFilters(filtersModel);
            //var ctrl = filters.ToControl();
            //((IControlBase)ctrl).UseModernStyle = false;
            //ctrl.RunAtForm();
        }
    }
}
