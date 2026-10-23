using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.SharedViewCollections;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections
{
    public class AppFormTests : TestingElement
    {
        public AppFormTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            var card = new EntityFormCard();
            card.OnEntity(new Product()
            {
                ProductName = "IPhone",
                ProductCost = 11000,
                ProductDesc = "High quality"

            }, new List<string>() {
                nameof(Product.ProductName),
                nameof(Product.ProductDesc),
                nameof(Product.ProductCost)
            });
        }
    }
}
