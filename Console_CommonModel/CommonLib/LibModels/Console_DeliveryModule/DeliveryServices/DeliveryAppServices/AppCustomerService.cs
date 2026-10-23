using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryAppServices
{
    public class AppCustomerService<TDeliveryDbContext>(SigninUser<TDeliveryDbContext> signinUser, ICustomerService customerService)
        where TDeliveryDbContext : DeliveryDbContext
    {
        private int GetCustomerId()
        {
            using (var db = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
            {
                var customer = db.Customers.FirstOrDefault(c => c.UserId == signinUser.Verify().Id);
                if (customer is null)
                    throw new Exception("Не удалось найти карточку клиента для данного пользователя");
                return customer.Id;
            }
        }

        private int? GetCurrentOrderId()
        {
            using (var db = new DeliveryDbContext())
            {
                var customer = db.Customers.FirstOrDefault(c => c.UserId == signinUser.Verify().Id);
                if (customer is null)
                    throw new Exception("Не удалось найти карточку клиента для данного пользователя");
                return customer.CurrentOrderId;
            }
        }

        public void AddItemToOrder(Product first)
        {
            var customerId = GetCustomerId();
            int? orderId = GetCurrentOrderId();
            if (orderId is null)
            {
                orderId = customerService.CreateOrder(customerId).Id;
            }
            customerService.AddItemToOrder(customerId, first, (int)orderId);
        }

        public Order CreateOrder()
        {
            var customerId = GetCustomerId();
            return customerService.CreateOrder(customerId);
        }

        public Order CreateOrder(IDictionary<Product, int> productCounts, int holderId)
        {
            var customerId = GetCustomerId();
            return customerService.CreateOrder(customerId, productCounts, holderId);
        }

        public Customer GetCustomerByPhone(string phoneNumber)
        {
            var customerId = GetCustomerId();
            return customerService.GetCustomerByPhone(customerId, phoneNumber);
        }

        public List<Order> GetOrders()
        {
            var customerId = GetCustomerId();
            return customerService.GetOrders(customerId);
        }

        public void OrderCheckout(Order order)
        {
            var customerId = GetCustomerId();
            customerService.OrderCheckout(customerId, order);
        }

        public Dictionary<int, int> SearchProductHolders(int productId, int count)
        {
            var customerId = GetCustomerId();
            return customerService.SearchProductHolders(customerId, productId, count);
        }

        public IEnumerable<Product> SearchProducts(string query)
        {
            var customerId = GetCustomerId();
            return customerService.SearchProducts(customerId, query);
        }

        public IEnumerable<Product> SearchProducts(string query, double maxPrice)
        {
            var customerId = GetCustomerId();
            return customerService.SearchProducts(customerId, query, maxPrice);
        }

        public IEnumerable<Product> SearchProducts(string query, List<string> categories)
        {
            var customerId = GetCustomerId();
            return customerService.SearchProducts(customerId, query, categories);
        }

        public IEnumerable<Product> SearchProducts(string query, List<string> categories, double maxPrice)
        {
            var customerId = GetCustomerId();
            return customerService.SearchProducts(customerId, query, categories, maxPrice);
        }

        public int SetProductReserved(int productId, int count, int stockId)
        {
            var customerId = GetCustomerId();
            return customerService.SetProductReserved(customerId, productId, count, stockId);
        }
    }
}
