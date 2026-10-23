namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices
{
    public interface ITreeFactory
    {
        TreeControlView CreateClassTree(Type rootType, Action<Type> onclicked);
        TreeControlView CreateClassTreeView<TBase>(Action<Type> onclicked);
        TreeControlView CreateClassTreeViewWithGeneration(Type rootType, Action<Type> onclicked);
        TreeControlView CreateClassTreeViewWithGeneration<TBase>(Action<Type> onclicked);
        List<TreeViewNodeModel> CreateNodes<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked);
        TreeControlView CreateTree<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked);
    }
}