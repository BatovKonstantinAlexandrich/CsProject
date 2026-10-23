using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    public interface ISearch<TEntity> where TEntity : BaseEntity, IChangeSupport
    {
        public string SearchQuery { get; set; }
        public ChangingList<string> SearchOptions { get; set; }
        public ChangingList<string> SearchWords { get; set; }
        public ChangingList<TEntity> visibleEntities { get; set; }


        public void OnInput(string query);
        public void OnSearch(string query);
    }
}
