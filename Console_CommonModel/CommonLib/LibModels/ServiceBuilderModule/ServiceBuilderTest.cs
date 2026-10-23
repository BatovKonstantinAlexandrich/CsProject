using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{
    public class ServiceBuilderTest
    {
        public static void Run()
        {
            ServiceBuilder<ServiceDbContext, ServiceResource> service = new(new ServiceDbContext());
            service.AddOpenApi("https://localhost:7049/swagger/v1/swagger.json");
            service.Init();
            service.Print();
        }
    }
}
