using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using pickpoint_delivery_service;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters
{
    [Label("Delivery: фильтры каталога товаров")]
    [Description("Применение IFilter (например PriceFilter) к запросу Product через SqlServerWebApi.")]
    public class ProductsFiltersTests : TestingService<DeliveryDbContext>
    {
        public override bool OnServiceTest(DeliveryDbContext db)
        {
            LogMessage("PriceFilter Min=10, Max=2000 на каталог Product");
            List<IFilter> filters = new()
            {
                new PriceFilter()
                {
                    Min = 10, Max = 2000
                }
            };
            IQueryable<Product> products = db.Products;
            var webapi = new SqlServerWebApi();
            var fasade = webapi.GetFasade<Product>();
            foreach (var filter in filters)
            {

            }


            var inRange = products.Count(p => p.ProductCost >= 10 && p.ProductCost <= 2000);
            var total = products.Count();
            LogMessage($"Товаров в диапазоне цены: {inRange} из {total}");
            return products.All(p => p.ProductCost >= 10 && p.ProductCost <= 2000);

        }
    }
}
