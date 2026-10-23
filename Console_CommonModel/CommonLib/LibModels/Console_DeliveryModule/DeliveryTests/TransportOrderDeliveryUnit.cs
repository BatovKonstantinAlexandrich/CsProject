using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryRolesServices;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests
{
    [Label("Функции транспортной компании")]
    [Description("Служба доставки просматривает созданные заказы, " +
        "назначает на конкретного исполнителя(курьера), исполнитель доставляет и заказ готов к выдаче.")]
    public class TransportOrderDeliveryUnit : TestingElement
    {
        private readonly CustomerService _customerService;
        private readonly TransportService _transportService;
        private readonly HolderService _holderService;
        private readonly DeliveryDbContext _deliveryDbContext;

        public TransportOrderDeliveryUnit(IServiceProvider provider) : base(provider)
        {
            _customerService = provider.Get<CustomerService>();
            _transportService = provider.Get<TransportService>();
            _holderService = provider.Get<HolderService>();
            _deliveryDbContext = provider.Get<DeliveryDbContext>();
        }

        public override void OnTest()
        {
            LogMessage("Сценарий транспортной компании: заказ → курьер → точка выдачи");
            Assert((test) =>
            {
                if (_deliveryDbContext.Customers.Count() == 0)
                {
                    _deliveryDbContext.Add(CustomerGenerator.CreateCustomer());
                    _deliveryDbContext.SaveChanges();
                }

                if (_deliveryDbContext.Transports.Count() == 0)
                {
                    _deliveryDbContext.Transports.Add(new Transport()
                    {
                        Latitude = 60.000000f,
                        Longitude = 30.000000f,
                        UserId = _deliveryDbContext.UserContexts.First().Id
                    });
                }
                if (_deliveryDbContext.Products.Count() == 0)
                {
                    var init = new DeliveryDbContextInitiallizer();
                    init.InitProducts(_deliveryDbContext);
                }

                if (_deliveryDbContext.Holders.Count() == 0)
                {
                    var init = new DeliveryDbContextInitiallizer();
                    init.InitHolders(_deliveryDbContext);
                }
                var db = _deliveryDbContext;

                _customerService.SetDbContext(db);
                var order = _customerService.CreateOrder(
                    db.Customers.First().Id,
                    new Dictionary<Product, int>() {
                    { db.Products.First(), 1 }
                    },
                    db.Holders.First().Id);
                _holderService.SetDbContext(db);
                _holderService.SetOrderStored(db.Holders.First().Id, order);
                order.OnOrderStored();

                _transportService.SetDbContext(db);
                var orders = _transportService.GetAvailableOrders(_deliveryDbContext.Transports.First().Id);
                var result = orders.Any(o => o.Id == order.Id);
                return result;
            },
            "Функция получение заказов для службы доставки работает корректно",
            "Функция получение заказов для службы доставки работает не корректно");


            Assert((test) =>
            {
                var order = _customerService.CreateOrder(
                    _deliveryDbContext.Customers.First().Id,
                    new Dictionary<Product, int>() {
                        { _deliveryDbContext.Products.First(), 1 }
                    },
                    _deliveryDbContext.Holders.First().Id);

                _holderService.SetOrderStored(_deliveryDbContext.Holders.First().Id, order);
                order.OnOrderStored();

                _deliveryDbContext.SaveChanges();

                if (_deliveryDbContext.Transports.Count() == 0)
                {
                    _deliveryDbContext.Transports.Add(new Transport()
                    {
                        Latitude = 60.000000f,
                        Longitude = 30.000000f,
                        UserId = _deliveryDbContext.UserContexts.First().Id
                    });
                }

                var orders = _transportService.GetAvailableOrders(_deliveryDbContext.Transports.First().Id);

                return orders.Any(o => o.Id == order.Id);
            },
            "Функция назначения заказов для службы доставки работает корректно",
            "Функция получение заказов для службы доставки работает не корректно");

        }

        public IEnumerable<Order> GetAvailableOrders()
        {
            return _deliveryDbContext.Orders.Where(order => order.OrderStatus == 2);
        }

        public void DoOrder(int orderId, int transportId)
        {
            _deliveryDbContext.Orders.First(order => order.Id == orderId).OnOrderIsDelivering();
            _deliveryDbContext.Orders.First(order => order.Id == orderId).TransportId = transportId;
            _deliveryDbContext.SaveChanges();
            Messages.Add("Транспортная компания взяла в работу заказ на доставку");
        }

        public void DoneOrder(int orderId, int transportId)
        {
            _deliveryDbContext.Orders.First(order => order.Id == orderId).OnOrderDelivered();
            _deliveryDbContext.Orders.First(order => order.Id == orderId).TransportId = transportId;
            _deliveryDbContext.SaveChanges();
            Messages.Add("Транспортная компания выполнила заказ на доставку");
        }
    }
}