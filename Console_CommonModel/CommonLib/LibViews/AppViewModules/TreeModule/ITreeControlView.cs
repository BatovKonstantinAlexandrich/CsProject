using Console_CommonModel.CommonLib.LibData.Console_DataModule;

public interface ITreeControlView
{
    string Description { get; set; }
    string Help { get; set; }
    string Icon { get; set; }
    string Label { get; set; }
    Action<object, EventArgs> OnClick { get; set; }
    TreeViewNodeModel root { get; set; }

    void BindHierServices(Action<TreeViewNodeModel> value);
    void BindHierServices(TreeViewNodeModel proot, Action<TreeViewNodeModel> value);
    void BindHierTable<TDbContext, TEntity>(Action<TEntity> onclicked = null)
        where TDbContext : DbContextBase
        where TEntity : HierTable<TEntity>;
    void BindHierTable<TEntity>(Action<TEntity> onclicked = null) where TEntity : HierTable<TEntity>;
    void BindSearchLine(SearchView searchLine);
    TreeControlView CreateClassTree(Type rootType, Action<Type> onclicked);
    TreeControlView CreateClassTreeView<TBase>(Action<Type> onclicked);
    TreeControlView CreateClassTreeViewWithGeneration(Type rootType, Action<Type> onclicked);
    TreeControlView CreateClassTreeViewWithGeneration<TBase>(Action<Type> onclicked);
    List<TreeViewNodeModel> CreateNodes<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked);
    TreeControlView CreateTree<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked);
    TreeViewNodeModel Find(List<int> path);
}