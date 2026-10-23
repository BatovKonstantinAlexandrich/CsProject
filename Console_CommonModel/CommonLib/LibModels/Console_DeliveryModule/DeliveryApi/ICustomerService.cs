using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi
{
    public interface ICustomerService
    {
        /*void AddItemToOrder(Product first, int orderId);
        Order CreateOrder(int customerId, IDictionary<Product, int> productCounts, int holderId);
        List<Order> GetOrders(int customerId);
        void OrderCheckout(Order order);
        Dictionary<int, int> SearchProductHolders(int productId, int count);
        IEnumerable<Product> SearchProducts(string query);
        int SetProductReserved(int productId, int count, int stockId);
        CustomerContext GetCustomerByPhone(string phoneNumber);*/


        /// <summary>
        /// Добавление товара в корзину заказа в кол-ве 1 шт.
        /// </summary>
        void AddItemToOrder(int customerId, Product first, int orderId);
        //int CreateCustomer(string firstName, string lastName, string phoneNumber);
        Order CreateOrder(int customerId);
        Order CreateOrder(int customerId, IDictionary<Product, int> productCounts, int holderId);
        Customer GetCustomerByPhone(int customerId, string phoneNumber);
        //List<Holder> GetHolders();
        List<Order> GetOrders(int customerId);
        void OrderCheckout(int customerId, Order order);
        Dictionary<int, int> SearchProductHolders(int customerId, int productId, int count);
        IEnumerable<Product> SearchProducts(int customerId, string query);
        IEnumerable<Product> SearchProducts(int customerId, string query, double maxPrice);
        IEnumerable<Product> SearchProducts(int customerId, string query, List<string> categories);
        IEnumerable<Product> SearchProducts(int customerId, string query, List<string> categories, double maxPrice);
        int SetProductReserved(int customerId, int productId, int count, int stockId);
    }
}