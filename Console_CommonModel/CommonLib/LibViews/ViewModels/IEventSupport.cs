
public interface IEventSupport
{
    void BeforeSendEvent(object messageEvent);
    void FireEvent(CommonEventMessage<string> message);
    void FireEvent(PropertyChangedMessage propertyChangedMessage);
    void FireEvent<T>(CommonEventMessage<T> message);
    void FireEvent<T>(T message);
    List<string> GetEventListeners();
    void SendChanges();
    void SendEvent(object messageEvent);
}