using pickpoint_delivery_service;

namespace WebApi_CommonModel.MiddlewareComponents
{
    public class ServiceMethodsNavigation<TDbContext>: IControlService<ServiceMethodsMiddleware<TDbContext>>
        where TDbContext: DeliveryDbContext
    {
        public ServiceMethodsMiddleware<TDbContext> Control { get; set; }

        public void SetControl(ServiceMethodsMiddleware<TDbContext> Control)
        {
            this.Control = Control;
        }


        public List<ILink> GetNavMenus(string uri)
        {
            //var names = uri.Split("/").Where(name => !String.IsNullOrWhiteSpace(name)).ToList();
            //var urls = Control.Actions.Keys.ToList();
            var startedwith  = Control.Actions.Keys.ToList().Where(key => key.StartsWith(uri)).ToList();
            return startedwith.Select(url => (ILink)new LinkViewModel()
            {
                Label = url
            }).ToList();
        }
    }
}
