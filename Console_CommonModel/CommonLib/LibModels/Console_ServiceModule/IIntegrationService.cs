using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{
    public interface IIntegrationService<TDbContext, TIntegratedObject>
        where TIntegratedObject : class
        where TDbContext : DbContext
    {
        void OnDbSetChanged([NotNullNotEmpty] EntityState state, Dictionary<string, object> before, Dictionary<string, object> after);
    }
}