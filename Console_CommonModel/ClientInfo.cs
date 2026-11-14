using ClassModelLib.ActionsClient;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

public class ClientInfo: ActiveObject, IActionsClient
{     
    public void OnPublish(MethodModel method)
    {
    }

    public void OnUnPublish(MethodModel method)
    {
    }

    public void OnConnected()
    {
        
    }

    public void OnDisconnected(Exception ex)
    {
        
    }
}
