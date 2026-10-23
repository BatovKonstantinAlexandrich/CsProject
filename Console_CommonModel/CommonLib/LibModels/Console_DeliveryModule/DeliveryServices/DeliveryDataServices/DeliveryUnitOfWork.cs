using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryDataServices
{
    public class DeliveryUnitOfWork
    {
        public readonly IEntityFasade<Product> Products;
        public readonly IEntityFasade<Holder> Holders;
        public readonly IEntityFasade<Order> Orders;
        public readonly IEntityFasade<OrderItem> OrderItems;
        public readonly IEntityFasade<ProductImage> ProductImages;

        public DeliveryUnitOfWork(IEntityFasade<Product> products, IEntityFasade<Holder> holders, IEntityFasade<Order> orders, IEntityFasade<OrderItem> orderItems, IEntityFasade<ProductImage> productImages)
        {
            Products = products;
            Holders = holders;
            Orders = orders;
            OrderItems = orderItems;
            ProductImages = productImages;
        }



    }

}
