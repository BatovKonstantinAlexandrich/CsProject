using Blazor_CommonModel.Application;

using System.Collections.Concurrent;
public class ConnectionTracker : AuthorizationCollection<ClientInfo>, IDisposable
{
    public ConnectionTracker(AuthorizationOptions options, 
        ApplicationEventsService applicationEventsService) : base(options, applicationEventsService)
    {
        Action<EventArgs> listener = evt => { 
            if(evt is ListChangedEvent lce)
            {
                //lce.Target.Item is UserContext
            }
        };
        applicationEventsService.Subscribers += listener;
    }
}
