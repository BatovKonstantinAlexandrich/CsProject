using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    public interface IOnModelCreating
    {
        protected void OnModelCreating(ModelBuilder modelBuilder);
    }

}
