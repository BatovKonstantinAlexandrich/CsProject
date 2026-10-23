using System.Globalization;

using static Dapper.SqlMapper;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeServices
{
    public class TreeFactory : ITreeFactory
    {
        public TreeControlView CreateClassTreeView<TBase>(Action<Type> onclicked)
        {
            return CreateClassTree(typeof(TBase), onclicked);
        }
        public TreeControlView CreateClassTree(Type rootType, Action<Type> onclicked)
        {
            ServiceFactory.Get().AddTypes(rootType.Assembly);
            var all = rootType.Assembly.GetTypes().Where(c => !c.IsAbstract).Select(type => type.GetTypeName()).ToList();
            all.Sort();
            //all.ToJsonOnScreen().WriteToConsole();
            var types = rootType.Assembly.GetTypes().Where(p => p.BaseType == rootType).ToList().SortItems(
                (t1, t2) =>
                {
                    var comparer = StringComparer.Create(CultureInfo.CurrentCulture, true);
                    return comparer.Compare(t1.GetTypeName(), t2.GetTypeName());
                }
            );

            Func<Type, IEnumerable<Type>> getChildren = entity => rootType.Assembly.GetTypes().Where(p => p.BaseType == entity).ToList().SortItems(
                (t1, t2) =>
                {
                    var comparer = StringComparer.Create(CultureInfo.CurrentCulture, true);
                    return comparer.Compare(t1.GetTypeName(), t2.GetTypeName());
                }
            );
            Func<Type, TreeViewNodeModel> createNode = (entity) => new()
            {
                Title = $"{entity.GetLabel()}.[{entity.GetTypeName()}]",
                Item = entity,
                Expanded = true
            };
            var tree = new TreeControlView()
            {
                Expanded = true
            };
            tree.root.Title = $"{rootType.GetLabel()}.[{rootType.GetTypeName()}]";
            tree.root.Expanded = true;
            var children = CreateNodes(types, createNode, getChildren, onclicked);
            tree.root.AppendTreeViewNode(children);
            return tree;
        }

        public TreeControlView CreateClassTreeViewWithGeneration<TBase>(Action<Type> onclicked)
        {
            return CreateClassTree(typeof(TBase), onclicked);
        }

        public TreeControlView CreateClassTreeViewWithGeneration(Type rootType, Action<Type> onclicked)
        {
            ServiceFactory.Get().AddTypes(rootType.Assembly);
            var all = rootType.Assembly.GetTypes().Where(c => !c.IsAbstract).Select(type => type.Name).ToList();
            all.Sort();
            all.ToJsonOnScreen().WriteToConsole();
            var types = rootType.Assembly.GetTypes().Where(p => p.BaseType == rootType).ToList().SortItems(
                (t1, t2) =>
                {
                    var comparer = StringComparer.Create(CultureInfo.CurrentCulture, true);
                    return comparer.Compare(t1.GetTypeName(), t2.GetTypeName());
                }
            );

            Func<Type, IEnumerable<Type>> generatedTypes = entity =>
            {
                List<Type> res = new();
                if (entity.GetGenericArguments().Count() > 0)
                {
                    foreach (var parg in entity.GetGenericArguments().Select(p => ServiceFactory.Get().GetTypesExtended(p)))
                    {

                        foreach (var p in parg)
                        {
                            List<Type> children = new List<Type>();
                            children.Add(p);
                            res.Add(entity.MakeGenericType(children.ToArray()));
                        }

                    }
                }
                return res;
            };
            Func<Type, IEnumerable<Type>> getChildren = entity =>
            {
                var c1 = generatedTypes(entity).ToHashSet();
                var c2 = rootType.Assembly.GetTypes().Where(p => p.BaseType == entity).ToList().SortItems(
                    (t1, t2) =>
                    {

                        var comparer = StringComparer.Create(CultureInfo.CurrentCulture, true);
                        return comparer.Compare(t1.GetTypeName(), t2.GetTypeName());
                    }
                );
                List<Type> types = new();
                types.AddRange(c1);
                types.AddRange(c2);
                return types;
            };

            Func<Type, TreeViewNodeModel> createNode = (entity) => new()
            {
                Title = entity.GetTypeName(),
                NodeName = entity.GetTypeName(),
                Item = entity
            };
            var tree = new TreeControlView();
            tree.root.Title = $"{rootType.GetLabel()}.[{rootType.GetTypeName()}]";
            tree.root.Expanded = true;
            var children = CreateNodes(types, createNode, getChildren, onclicked);


            tree.root.AppendTreeViewNode(children);
            return tree;
        }

        public TreeControlView CreateTree<TEntity>(IEnumerable<TEntity> items,
            Func<TEntity, TreeViewNodeModel> createNode,
            Func<TEntity, IEnumerable<TEntity>> getChildren,
            Action<TEntity> onclicked)
        {
            var treeView = new TreeControlView();
            treeView.root.AppendRange(CreateNodes(items, createNode, getChildren, onclicked));
            return treeView;
        }


        public List<TreeViewNodeModel> CreateNodes<TEntity>(
            IEnumerable<TEntity> items,
            Func<TEntity, TreeViewNodeModel> createNode,
            Func<TEntity, IEnumerable<TEntity>> getChildren,
            Action<TEntity> onclicked)
        {
            List<TreeViewNodeModel> nodes = new();
            foreach (var item in items)
            {
                var pitem = item;
                var p = createNode(item);
                p.OnClick += (s, e) =>
                {
                    if (onclicked is not null)
                        onclicked(pitem);
                };
                nodes.Add(p);
                var children = CreateNodes(getChildren(item), createNode, getChildren, onclicked);
                children.ForEach(c => p.AppendTreeViewNode(c));
            }
            return nodes;
        }
    }
}
