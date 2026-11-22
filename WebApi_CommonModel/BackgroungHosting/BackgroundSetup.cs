using WebApi_CommonModel.WebApiModule;

namespace WebApi_CommonModel.BackgroungHosting
{

    public class BackgroundSetup: IControlService<WebApiHostingProgram>
    {
        public WebApiHostingProgram Control { get; set; }

        [NotInput]
        public void SetControl(WebApiHostingProgram program)
        {
            this.Control = program;
            
        }

        [Label("Добавить фоновую службу")]
        public void AddBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
            => Control.AddBackgroundServiceTypes<TBackgroundServiceType>();
        
    }
}
