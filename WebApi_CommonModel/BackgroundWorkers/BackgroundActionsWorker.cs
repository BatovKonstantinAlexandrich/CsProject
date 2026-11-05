namespace WebApi_CommonModel.BackgroundWorkers
{
    public class BackgroundActionsWorker: BackgroundQueueWorker<ActionRequest>
    { 
        protected override Task HandleMessage(ActionRequest message)
        {
            throw new NotImplementedException();
        }
    }
}
