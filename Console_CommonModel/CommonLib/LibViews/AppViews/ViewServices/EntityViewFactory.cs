using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices
{
    public class EntityViewFactory
    {


        public TableView CreateTable<EntityType>(IEnumerable<EntityType> dataset) where EntityType : BaseEntity
        {
            TableView table = new TableView();
            table.Title = typeof(EntityType).GetLabel();
            table.Columns = typeof(EntityType).GetInputPropertiesSorted();
            table.DataRows = new ChangingList<object>(dataset.Select(p => (object)p).ToList());
            return table;
        }



        public TreeViewModel CreateTree<EntityType>(IEnumerable<HierTable<EntityType>> dataset)
        {
            TreeViewModel tree = new TreeViewModel();
            var rootItem = dataset.FirstOrDefault(p => p.ParentId == 0);
            tree.Item = rootItem;

            dataset.Where(p => p.ParentId == tree.Item.Id).Select(p => tree.Append(new TreeViewModel(tree, p)));
            return tree;
        }


    }
}
