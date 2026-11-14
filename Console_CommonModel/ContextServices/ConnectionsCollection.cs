using Blazor_CommonModel.Application;

namespace Blazor_MyApp.Services.ContextServices
{
    public class ConnectionsCollection : BackgroundCollection<ClientInfo>
    {
        public ConnectionsCollection(AuthorizationOptions options, ApplicationEventsService applicationEventsService) : base(options, applicationEventsService)
        {
        }
    }
}
