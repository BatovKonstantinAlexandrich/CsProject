using Console_CommonModel.CommonLib.LibViews.SharedViewCollections;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.TableView
{
    public interface ITableView<T> : ITitled
    {
        public ChangingList<T> DataRows { get; set; }
    }
}
