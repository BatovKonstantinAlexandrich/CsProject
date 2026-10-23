using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{
    [Label("Тeстирование ServiceModelIntegrationService")]
    public class ServiceModelIntegrationServiceTest: TestingElement
    {
        public ServiceModelIntegrationServiceTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            try
            {
                using (var db = typeof(ServiceDbContext).New<ServiceDbContext>())
                {                    
                    db.ServiceModels.Add(new()
                    {
                        Name = "WebApiFiles",
                        Code = "WebApiFiles",
                        Schema = "http://localhost:5284/swagger/v1/swagger.json",
                        Type = "OpenAPI"
                    });
                    db.SaveChangesWithLateIntegration();
                }
            }
            catch (Exception ex)
            {
                this.Error(ex);
            }
        }
    }
}
