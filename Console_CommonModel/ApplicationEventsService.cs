
namespace Blazor_CommonModel.Application
{
    public class ApplicationEventsService: EventSupport
    {
        public Action<EventArgs> Subscribers = p => { };

        public ApplicationEventsService()
        {
        }

        public void Publish(EventArgs evt)
        {
            Subscribers(evt);
        }
    }
}
