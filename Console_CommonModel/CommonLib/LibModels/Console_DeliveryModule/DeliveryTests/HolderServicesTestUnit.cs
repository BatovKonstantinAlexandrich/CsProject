using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryRolesServices;

using pickpoint_delivery_service;


namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests
{

    [Label("Тестирования функции учёта на точке выдачи")]
    [Description("Сценарии Holder: приём и выдача заказов на точке хранения (DeliveryRolesServices).")]
    public class HolderServicesTestUnit : TestingElement
    {

        public HolderServicesTestUnit(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("Holder: формирование автозаказа на точке выдачи");
            OnTestCreateOrder();
        }

        [Label("Тест. Формирование автозаказа")]
        public void OnTestCreateOrder()
        {
            using (var db = provider.Get<DeliveryDbContext>())
            {
                var holderService = provider.Get<HolderService>();
                holderService.SetDbContext(db);

                var holder = db.Holders.ToList().FirstOrDefault();
                if (holder is null)
                {
                    throw new Exception($"Не зарегистрирована ни одна запись в {nameof(Holder)}");
                }
                var products = holderService.GetProductsCounts(holder.Id);
                new
                {
                    holder,
                    products
                }.ToJsonOnScreen().WriteToConsole();

                foreach (var key in products.Keys)
                {
                    products[key] = products[key] + 1;
                }

                var items = holderService.CreateOrder(holder.Id, products);
                Assert(el => items.All(p => p.Value == 1),
                    $"Формирование автозаказа на оптовый склад по адресу {holder.HolderLocation} работает корректно",
                    $"Формирование автозаказа на оптовый склад по адресу {holder.HolderLocation} работает не корректно");

                ;
            }


        }

    }
}