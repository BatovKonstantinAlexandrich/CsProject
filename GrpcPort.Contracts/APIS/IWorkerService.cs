namespace WebApi_CommonModel.BackgroungHosting
{
    public interface IWorkerService
    {
        public Task Start();
        public Task Stop();
    }
}
