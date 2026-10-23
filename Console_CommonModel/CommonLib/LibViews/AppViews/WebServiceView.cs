using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    [Label("Представление веб-сервиса")]
    public class WebServiceView : AppViewBase
    {
        [Label("Наименование службы")]
        [InputDbContextDictionary<ServiceDbContextBase<ServiceResource>>(nameof(ServiceModel) + ",Name")]
        public int WebServiceId { get; set; }

        [NotInput]
        public ServiceBuilder<ServiceDbContextBase<ServiceResource>, ServiceResource> ServiceBuilder { get; set; }

        [NotInput]
        public ServiceResource WebService { get; set; }


        public WebServiceView()
        {
            ServiceBuilder = new(new());
        }



        [OnChange(nameof(WebService))]
        public void OnWebServiceNameChanged()
        {
            if (WebService is not null)
            {
                this.InfoTime("Создание модели веб-сервиса", () =>
                {
                    try
                    {
                        ServiceBuilder.CreateServiceModel(WebService);
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex);
                    }
                });
            }
        }
    }
}