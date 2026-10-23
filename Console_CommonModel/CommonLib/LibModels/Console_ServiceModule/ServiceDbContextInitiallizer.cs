using System.Collections.Generic;
using System.Linq;
using System.Text;

using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{
    [Label("Инициализация моделей сетевых служб")]
    public class ServiceDbContextInitiallizer : DbContextInitiliazer<ServiceDbContext>
    {
        public override Dictionary<string, int> Init(ServiceDbContext context)
        {
            return new Dictionary<string, int>
            {
                [nameof(ServiceModel)] = InitDemoServices(context)
            };
        }

        [Label("Демо WSDL/OpenAPI службы")]
        public int InitDemoServices(ServiceDbContext context)
        {
            if (context.ServiceModels.Any())
                return 0;

            context.ServiceModels.Add(new ServiceResource
            {
                Name = "PayGwManager",
                Schema = DemoOpenApiSchema("PayGwManager"),
                Type = "OpenAPI",
                Dll = Encoding.UTF8.GetBytes("// compiled proxy placeholder")
            });
            context.ServiceModels.Add(new ServiceResource
            {
                Name = "VendorsSetup",
                Schema = DemoOpenApiSchema("VendorsSetup"),
                Type = "OpenAPI",
                Dll = Encoding.UTF8.GetBytes("// compiled proxy placeholder")
            });
            context.ServiceModels.Add(new ServiceResource
            {
                Name = "Booking",
                Schema = DemoWsdlSchema("Booking"),
                Type = "WSDL",
                Dll = null
            });

            return context.SaveChanges();
        }

        private static string DemoOpenApiSchema(string title) =>
            $$"""
              {
                "openapi": "3.0.1",
                "info": { "title": "{{title}}", "version": "1.0.0" },
                "paths": {}
              }
              """;

        private static string DemoWsdlSchema(string name) =>
            $$"""<?xml version="1.0"?><definitions name="{{name}}" targetNamespace="urn:demo"/>""";
    }
}
