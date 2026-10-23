using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests
{
    /// <summary>
    /// Учет товаров
    /// </summary>
    [Label("Проверка функции бронирования товарных позиций")]
    [Description("Бронирование товарных позиций на складе до оформления заказа клиентом.")]
    public class ReservationOrderCheckoutUnit : TestingElement
    {

        public ReservationOrderCheckoutUnit(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("Бронирование товарной позиции (streak) на складе");
            this.Info($"начинаю тестирование ... ");
            using (var db = new DeliveryDbContext())
            {
                var customerService = new CustomerService(db);
                var customer = db.Customers.First();
                var products = customerService.SearchProducts(customer.Id, "");
                if (products is null || products.Count() == 0)
                {
                    return;
                }
                var product = products.First(p => p.ProductName.IndexOf("streak") != -1);
                Dictionary<int, int> instocks = customerService.SearchProductHolders(customer.Id, product.Id, 1);
                this.Info(instocks.ToJsonOnScreen());
                if (instocks.Count() == 0)
                    return;
                var holderId = instocks.Keys.First();
                int count = instocks.Values.First();
                customerService.SetProductReserved(customer.Id, product.Id, 1, holderId);
                var instocks2 = customerService.SearchProductHolders(customer.Id, product.Id, 1);


                this.Info(instocks[holderId]);
                this.Info(instocks2[holderId]);
                if (instocks[holderId] - instocks2[holderId] != 1)
                {

                    Messages.Add($"При получении сведений о наличии товаров на складах не учитывает резерв товара.");
                }
                else
                {
                    Messages.Add($"Функция учёта товара в резерве работает корректно");
                }
            }


        }
    }
}