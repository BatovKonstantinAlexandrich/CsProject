using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices
{
    [Label("Delivery: сервис остатков и резервирования")]
    [Description("IProductsInStockService.GetReservationStocksIds для заказов в DeliveryDbContext.")]
    public class ProductsInStockServiceTest : TestingElement
    {
        public ProductsInStockServiceTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("IProductsInStockService.GetReservationStocksIds для всех заказов");
            AssertService<IProductsInStockService>(service =>
            {
                var orders = provider.Get<DeliveryDbContext>().Orders.ToList();
                LogMessage($"Заказов в контексте: {orders.Count}");
                foreach (var item in orders)
                {
                    var ids = service.GetReservationStocksIds(item.Id);
                    LogMessage($"Заказ Id={item.Id}: вариантов резервирования — {ids.Count()}");
                }
                return true;
            },
            "Получение вариантов резервирования работает успешно",
            "Получение вариантов резервирования работает некорректно");
        }
    }
}
