namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels.CoreMessaging
{
    /// <summary>
    /// Интерфейс служб, сервисов, клиентов и посредников.
    /// </summary>
    public interface MessageAPI
    {
        void OnMessage(HttpMessage request, ResponseMessage response);
    }
}
