using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using System;

using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_ServiceModule;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{


    [Label("База сетевых служб")]
    public class ServiceDbContext : ServiceDbContextBase<ServiceResource>
    {
        [ServiceModelIntegrationService()]
        public override DbSet<ServiceResource> ServiceModels { get; set; }
    }
    public class ServiceDbContextBase<TServiceModel> : FileDbContext
        where TServiceModel : ServiceModel
    {
        public ServiceDbContextBase() { }
        public ServiceDbContextBase(DbContextOptions options) : base() { }

        [ServiceModelIntegrationService()]
        public virtual DbSet<TServiceModel> ServiceModels { get; set; }

        
    }
}




