using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices
{
    internal class SparePart
    {
        public object Product_id { get; internal set; }
        public string Product_name { get; internal set; }
        public string Description { get; internal set; }
        public List<ProductImage> Images { get; internal set; }
    }
}