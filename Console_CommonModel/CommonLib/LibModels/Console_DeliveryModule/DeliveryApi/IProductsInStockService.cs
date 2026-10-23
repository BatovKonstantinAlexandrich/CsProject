using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi
{
    public interface IProductsInStockService
    {
        public IEnumerable<int> GetReservationStocksIds(int orderId);
        public IEnumerable<Holder> GetReservationStocks(int orderId);
        public bool HasProducts(int holderId, Dictionary<int, int> products);
    }
}
