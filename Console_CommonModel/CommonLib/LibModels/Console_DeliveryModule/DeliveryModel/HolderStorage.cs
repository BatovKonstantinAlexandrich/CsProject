using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Полка на складе")]
    public class HolderStorage : BaseEntity
    {
        public int HolderId { get; set; }
        public Holder Holder { get; set; }
        public string StorageCode { get; set; }
        public string ProductCode { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

    }
}
