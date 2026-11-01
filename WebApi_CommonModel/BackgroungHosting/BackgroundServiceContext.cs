using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using System.Collections.Concurrent;

namespace WebApi_CommonModel.BackgroungHosting
{
    [Label("Операции со службами")]
    public interface IBackgroundServiceContext
    {
        public void AddBackgroundServiceTypes<TBackgroundServiceType>()
            where TBackgroundServiceType : IWorkerService;
        public void RemoveBackgroundServiceTypes<TBackgroundServiceType>()
            where TBackgroundServiceType : IWorkerService;
    }


    [Label("Контекст управления службами")]
    public class BackgroundServiceContext: IBackgroundServiceContext
    {
        public ChangingList<Type> BackgroundServiceTypes { get; set; } = new();
        public ConcurrentDictionary<Type, IWorkerService> BackgroundServices { get; set; } = new();

        public void AddBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
        {
            BackgroundServiceTypes.Add(typeof(TBackgroundServiceType));
        }

        public void RemoveBackgroundServiceTypes<TBackgroundServiceType>() where TBackgroundServiceType : IWorkerService
        {
            BackgroundServiceTypes.Remove(typeof(TBackgroundServiceType));
        }
    }
}
