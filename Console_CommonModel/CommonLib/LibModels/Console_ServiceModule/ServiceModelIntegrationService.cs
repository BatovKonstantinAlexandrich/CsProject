using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{

    [Label("Интеграция ServiceResource c файловыми ресурсами")]
    [Description("При добавлении ServiceResource должен создаться файл с исходником для клиента сервиса")]
    public class ServiceModelIntegrationService:
        IntegrationService<ServiceDbContext, ServiceResource>
    {

        private ServiceResourceCompiler compiler = new();

        public ServiceModelIntegrationService() : base()
        {
        }
        public ServiceModelIntegrationService(ServiceDbContext dbContext) : base(dbContext)
        {
        }

        protected FileCatalog FindDataObject(ServiceResource integrated)
        {
                       
            var serviceCatalog = this.dbContext.GetFileCatalog(new List<string>()
            {
                nameof(ServiceDbContext),
                integrated.Code                
            });   
            serviceCatalog.Label = integrated.Label;
            serviceCatalog.Files = new(dbContext.FileResources.Where(f => f.ParentId == serviceCatalog.Id).ToList());
            
            return serviceCatalog;
        }
         

        protected override void OnUpdated(Dictionary<string, PropertyChangedMessage> changes, ServiceResource beforeChanges, ServiceResource afterChanges)
        {
            if(beforeChanges.Schema == afterChanges.Schema)
                return;
            var integrated = afterChanges;
            var source = compiler.CreateOpenApiSourceFile(integrated.Schema);
            var catalog = FindDataObject(integrated);
            dbContext.RemoveRange(catalog.Files);
            dbContext.SaveChangesWithLateIntegration();
            FileResource clientSource = null;
            dbContext.FileResources.Add(clientSource = new()
            {
                Name = integrated.Code + ".cs",
                Data = Encoding.UTF8.GetBytes(source),
                Mime = "text/cs"
            });
            dbContext.SaveChangesWithLateIntegration();
            catalog.Files.Clear();
            catalog.Files.Add(clientSource);
        }

        protected override void OnDeleted(ServiceResource integrated)
        {
            var source = compiler.CreateOpenApiSourceFile(integrated.Schema);
            var catalog = FindDataObject(integrated);
            dbContext.RemoveRange(catalog.Files);
            dbContext.Remove(catalog);
            dbContext.SaveChanges();            
        }

        protected override void OnAdded(ServiceResource integrated)
        {
            var source = compiler.CreateOpenApiSourceFile(integrated.Schema);
            var catalog = FindDataObject(integrated);
            dbContext.RemoveRange(catalog.Files);
            dbContext.SaveChangesWithLateIntegration();
            FileResource clientSource = null;
            dbContext.FileResources.Add(clientSource = new()
            {
                Name = integrated.Code + ".cs",
                Data = Encoding.UTF8.GetBytes(source),
                Mime = "text/cs",
                ParentId = catalog.Id
            });
            dbContext.SaveChangesWithLateIntegration();
            catalog.Files.Clear();
            catalog.Files.Add(clientSource);            
        }
    }
}
