namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices
{
    public interface ITreeService
    {
        public TreeControlView CreateTreeView<TEntity>(IQueryable<TEntity> dbset, Action<TEntity> onclicked)
            where TEntity : HierTable<TEntity>;
    }



    /// <summary>
    /// 
    /// </summary>
    public class TreeService : ITreeService
    {



        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dbset"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public TreeControlView CreateTreeView<TEntity>(IQueryable<TEntity> dbset, Action<TEntity> onclicked = null)
            where TEntity : HierTable<TEntity>
        {
            TreeControlView result = new();
            var root = dbset.FirstOrDefault(p => p.ParentId == null);
            if (root is null)
            {
                throw new Exception("Не найден корневой узел");
            }
            result.root = CreateTreeViewNode(dbset, root, onclicked);
            return result;
        }


        public TreeViewNodeModel CreateTreeViewNode<TEntity>(IQueryable<TEntity> dbset, TEntity item, Action<TEntity> onclicked)
            where TEntity : HierTable<TEntity>
        {
            TreeViewNodeModel result = new()
            {
                Title = item is null ? "" : item.Label,
                Item = item
            };
            result.OnClick += (s, e) =>
            {
                if (onclicked is not null)
                {
                    onclicked(item);
                }
            };
            if (item is not null)
                foreach (var childItem in dbset.Where(p => p.ParentId == item.Id).ToList())
                {
                    result.Children.Add(CreateTreeViewNode(dbset, childItem, onclicked));
                }
            return result;
        }


    }
}
