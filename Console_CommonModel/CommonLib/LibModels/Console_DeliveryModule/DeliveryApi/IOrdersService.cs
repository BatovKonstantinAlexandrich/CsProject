using System.ComponentModel.DataAnnotations;
using static System.Console;
using static Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi
{
    public interface IOrdersService
    {
        public Order GetOrder(int id);
        public IEnumerable<Order> GetOrders();
        public IEnumerable<OrderItem> GetOrderItems(int orderId);
        public void CancellOrder(int orderId);
        public void CreateReservationOrder(int holderId, Dictionary<int, int> products, int customerId);
        public void CreateDeliveryOrder(int holderId, Dictionary<int, int> products, int customerId);
        public void CompleteOrder(int orderId);
    }

}
