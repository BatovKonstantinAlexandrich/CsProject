using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices
{
    public class HolderProductsFasade : EfEntityFasade<Product>
    {
        private readonly Holder _holder;

        public HolderProductsFasade(DeliveryDbContext context, Holder holder) : base(context)
        {
            _holder = holder;
        }
        public override IQueryable<Product> GetDbSet() => _context.Set<Product>().Where(product => ((DeliveryDbContext)_context).ProductsInStock.Where(instock => instock.HolderId == _holder.Id).Select(instock => instock.ProductId).Contains(product.Id));

    }
}
