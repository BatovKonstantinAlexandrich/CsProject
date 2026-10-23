using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests
{
    [Label("Тестирование функций покупателя")]
    [Description("Тестирование процесса оформления заказа со стороны клиента компании")]
    public class CustomerOrderCheckoutUnit : TestingElement
    {

        public CustomerOrderCheckoutUnit(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("Сценарий покупателя: оформление заказа и создание заказа");
            CustomerOrderCheckoutTest();
            CustomerCreateOrderTest();
        }



        public void OnTestData()
        {
            CustomerCreateOrderTest();

        }

        private void CustomerCreateOrderTest()
        {
            AssertService<DeliveryDbContext>((db) =>
            {
                Customer customer = db.Customers.FirstOrDefault();
                if (!db.Customers.Any())
                {

                    new DeliveryDbContextInitiallizer().InitFromFiles(db, "D:\\System-Config\\ProjectsConsole\\Console_AuthModel");
                    Messages.Add("Инициализация первичных данных выполнена успешно");

                    customer = new Customer()
                    {

                        FirstName = "FirstName",
                        LastName = "LastName",
                        PhoneNumber = "PhoneNumber",
                    };
                    db.Add(customer);
                    db.SaveChanges();
                }

                var signin = provider.Get<SigninUser<DeliveryDbContext>>();
                signin.SetDbContext(db);
                var user = db.UserContexts.Include(u => u.Account).FirstOrDefault(u => u.Id == customer.UserId);
                var signinResult = signin.Signin(user.Account.Email, user.Account.Password);
                if (!signinResult.Succeeded)
                    throw new Exception("Не удалось выполнить авторизацию");

                var order = new Order()
                {
                    CustomerId = customer.Id
                };
                order.HolderId = db.Holders.First().Id;
                db.Add(order);
                db.SaveChanges();

                var orders = new OrdersService<DeliveryDbContext>(db, signin);
                Order value = orders.CreateOrder(customer.Id);
                orders.GetOrders().ToJsonOnScreen().WriteToConsole();
                orders.GetOrderItems(orderId: value.Id).ToJsonOnScreen().WriteToConsole();
                Messages.Add("Добавлен новый заказ");
                return true;
            },
            "Клиент может содавать заказы",
            "Клиенту не удалось создать заказ");
        }



        private void CustomerOrderCheckoutTest()
        {
            AssertService<DeliveryDbContext>(
                db =>
                {
                    var productsInStockService = provider.Get<IProductsInStockService>();
                    var holdersService = provider.Get<IHolderService>();
                    var service = provider.Get<ICustomerService>();


                    Customer customer = null;
                    if (db.Customers.Count() == 0)
                    {
                        customer = new Customer()
                        {
                            FirstName = "Batov",
                            LastName = "Konstantin",
                            PhoneNumber = "79210903572",
                        };
                        db.Customers.Add(customer);
                        db.SaveChanges();
                    }
                    else
                    {
                        customer = db.Customers.First();
                    }

                    var products = service.SearchProducts(customer.Id, "");

                    Messages.Add($"Ищю товары по запросу: dell");
                    products = service.SearchProducts(customer.Id, "dell");
                    //this.Messages.Add(products.ToJsonOnScreen());

                    Messages.Add($"Ищю товары по запросу: venue");
                    products = service.SearchProducts(customer.Id, "venue");
                    //this.Messages.Add(products.ToJsonOnScreen());

                    if (db.Holders.Count() == 0)
                    {
                        var initiallizer = new DeliveryDbContextInitiallizer();
                        initiallizer.InitHolders(db);
                    }
                    var customerId = db.Customers.First().Id;
                    var productCounts = new Dictionary<Product, int>(
                        products.Select(item => new KeyValuePair<Product, int>(item, 1))
                    );
                    var holderId = db.Holders.First().Id;
                    var order = service.CreateOrder(customerId, productCounts, holderId);
                    service.OrderCheckout(customer.Id, order);
                    Messages.Add("Заказ оформлен");


                    /*var orders = this.provider.Get<IOrdersService>();
                    if (orders.Count() == 0)
                    {
                        var products = _customerService.SearchProducts("dell");
                        var products = _customerService.SearchProducts("dell");
                        if (products.Count() == 0)
                            throw new Exception("Необходимо зарегистрировать товары");
                        var customer = _customerService.GetCustomerByPhone("79210903572");
                        _customerService.CreateCustomer("Константин", "Батов", "79210903572");
                        var holderId = _customerService.GetHolders().First().Id;
                        var porder = _customerService.CreateOrder(customer.Id, new Dictionary<Product, int>()
                            {
                                { products.First(), 1}
                            }, holderId);

                    }*/
                    return true;
                },
                "Клиент может успешно оформлять заказы",
                "Клиент не может оформлять заказы"
            );
        }
    }
}