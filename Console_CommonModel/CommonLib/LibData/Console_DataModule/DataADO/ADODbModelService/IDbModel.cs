using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService
{
    public interface IdbModel : ISqlExecutor
    {
        public ChangingList<Type> EntityTypes { get; }
        public Type[] GetEntityClasses();
        public void AddEntityType(Type entity);

        public string GetCurrentVersion();
        public string GetTargetVersion();
    }
}
