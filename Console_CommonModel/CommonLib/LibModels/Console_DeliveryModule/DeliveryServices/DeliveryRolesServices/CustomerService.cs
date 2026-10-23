using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using pickpoint_delivery_service;

using System.Reflection;


/// <summary>
/// Операции выполняемые клиентом, оформление заказов, поиск товаров
/// </summary>
public class CustomerService : ICustomerService
{

    private DeliveryDbContext _deliveryUnit;
    public void SetDbContext(DeliveryDbContext ctx) => _deliveryUnit = ctx;

    public CustomerService(DeliveryDbContext deliveryUnit)
    {
        _deliveryUnit = deliveryUnit;
    }


    /// <summary>
    /// Получить список заказов выполненных клиентом
    /// </summary>
    /// <param name="customerId">ид-клиента</param>        
    public List<Order> GetOrders(int customerId)
    {
        return _deliveryUnit.Orders.Include(o => o.OrderItems).Where(o => o.CustomerId == customerId).ToList();
    }

    /// <summary>
    /// Создать заказ 
    /// </summary>
    /// <param name="customerId">ид-клиента </param>
    /// <param name="productCounts">товары в заданном кол-ве</param>
    /// <param name="holderId">точка самовывоза</param>        
    public Order CreateOrder(int customerId, IDictionary<Product, int> productCounts, int holderId)
    {
        var order = new Order() { CustomerId = customerId };
        order.OnOrderCreated();
        _deliveryUnit.Orders.Add(order);
        _deliveryUnit.SaveChanges();
        foreach (var products in productCounts)
        {
            var item = new OrderItem()
            {
                Product = products.Key,
                ProductId = products.Key.Id,
                ProductCount = products.Value,
                OrderId = order.Id
            };
            _deliveryUnit.OrderItems.Add(item);
            order.OrderItems.Add(item);
        }
        _deliveryUnit.SaveChanges();

        order.HolderId = holderId;
        _deliveryUnit.SaveChanges();
        order.Holder = _deliveryUnit.Holders.FirstOrDefault(holder => holder.Id == holderId);
        return order;
    }

    public IEnumerable<Product> SearchProducts(int customerId, string query, double maxPrice)
    {
        var fasade = new EfEntityFasade<Product>(_deliveryUnit);
        var products = fasade.Search(query);
        return products.Where(product => product.ProductCost <= maxPrice).ToList();
    }

    public IEnumerable<Product> SearchProducts(int customerId, string query, List<string> categories, double maxPrice)
    {

        //TODO
        var fasade = new EfEntityFasade<Product>(_deliveryUnit);
        var products = fasade.Search(query);

        return products;
    }

    public IEnumerable<Product> SearchProducts(int customerId, string query)
    {
        var fasade = new EfEntityFasade<Product>(_deliveryUnit);
        return fasade.Search(query);
    }

    public IEnumerable<Product> SearchProducts(int customerId, string query, List<string> categories)
    {
        var fasade = new EfEntityFasade<Product>(_deliveryUnit);
        return fasade.Search(query);
    }

    public void AddItemToOrder(int customerId, Product first, int orderId)
    {
        try
        {
            _deliveryUnit.OrderItems.Add(new OrderItem()
            {
                ProductId = first.Id,
                ProductCount = 1,
                OrderId = orderId
            });
            _deliveryUnit.SaveChanges();
        }
        catch (Exception ex)
        {
            this.Error("Ошибка при добавлении товара в заказ", ex);
            throw new Exception("Ошибка при добавлении товара в заказ", ex);
        }
    }

    public void OrderCheckout(int customerId, Order order)
    {

        var current = _deliveryUnit.Orders.Find(order.Id);
        current.SetProperties(order);
        current.OnOrderCreated();
        _deliveryUnit.SaveChanges();


    }


    /// <summary>
    /// Получение сведений о наличии товара в заданном кол-ве
    /// Возвращает справочник типа <HolderId,Count>
    /// </summary>
    public Dictionary<int, int> SearchProductHolders(int customerId, int productId, int count)
    {
        var instocks = _deliveryUnit.ProductsInStock.Where(instock => instock.ProductId == productId && instock.ProductCount - instock.ProductsInReserve >= count);
        var result = instocks.Select(instock => new KeyValuePair<int, int>(instock.HolderId, instock.ProductCount - instock.ProductsInReserve)).ToList();
        return new(result);
    }

    public int SetProductReserved(int customerId, int productId, int count, int stockId)
    {
        ProductsInStock instock = _deliveryUnit.ProductsInStock.FirstOrDefault(instock => instock.HolderId == stockId && productId == productId);
        /*if (instock.ProductsInReserve + count > instock.ProductCount)
        {
            throw new ArgumentException("count");
        }*/
        instock.ProductsInReserve += count;
        _deliveryUnit.SaveChanges();
        return instock.ProductsInReserve;
    }

    public Customer GetCustomerByPhone(int customerId, string phoneNumber)
    {
        return _deliveryUnit.Customers.FirstOrDefault(customer => customer.PhoneNumber == phoneNumber);
    }

    /*public int CreateCustomer(string firstName, string lastName, string phoneNumber)
    {
        var customer = new CustomerContext()
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber
        };
        _deliveryUnit.Customers.Add(customer);
        _deliveryUnit.SaveChanges();
        return customer.Id;
    }*/

    public List<Holder> GetHolders()
    {
        return _deliveryUnit.Holders.ToList();
    }


    /// <summary>
    /// Клиент создаёт новую корзину заказа
    /// </summary>
    /// <param name="customerId">ид-клиента</param>      
    public Order CreateOrder(int customerId)
    {
        try
        {
            MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { customerId });
            var order = new Order() { CustomerId = customerId };
            _deliveryUnit.Orders.Add(order);

            _deliveryUnit.SaveChanges();

            var customer = _deliveryUnit.Customers.Find(customerId);
            customer.CurrentOrderId = order.Id;
            _deliveryUnit.SaveChanges();

            this.Info($"Клиент {customerId} создал новую корзину заказа {customer.CurrentOrderId}");
            return order;
        }
        catch (Exception ex)
        {
            this.Error($"Ошибка при создании корзины заказа клиентом {customerId}");
            throw new Exception($"Ошибка при создании корзины заказа клиентом { customerId }", ex);
        }
    }
     
}