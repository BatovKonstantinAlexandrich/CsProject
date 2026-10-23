using System;
using System.Linq;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputComponents;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using System.ComponentModel.DataAnnotations.Schema;

[Label("Иерархическое представление сущностей")]
public class TreeControlView<TEntity> : TreeControlView
{
    public Action AddDesignerMenu()
    {
        return this.ContextMenu.AddContextServiceMethods("Настройки",new Dictionary<string, Action>()
        {
            { "Классы сущностей", ()=>{

            } },
            { "Иерархическая модель", ()=>{
            } },
        });
    }



}


public interface ITreeControl : ITreeControlView, IControlBase
{
    Action<List<int>> OnNodeSelected { get; set; }
}

/// <summary>
/// Иерархическое представление данных
/// </summary>
[Label("Иерархическое представление")]
public class TreeControlView : AppViewBase, IDescriptableObject, ITreeFactory, ITreeControlView
{
    private TreeFactory _treeFactory = new();

    [NotInput()]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public override Action<object, EventArgs> OnClick { get; set; } = (s, e) => { };


    [NotInput()]
    private TreeViewNodeModel _root = new()
    {
        Title = "Каталог"

    };


    [Label("Корень иерархии")]
    [InputCustom("")]
    public TreeViewNodeModel root
    {
        get
        {
            return _root;
        }
        set
        {
            _root = value;
            _root.Parent = this;
        }
    }

    public TreeControlView()
    {
        _root.Parent = this;
        this.SearchQuery = "";
        this.Searchable = true;

        this.OnClick = (s, e) =>
        {
            //this.Info(s);
        };
        this.ViewDesigner = new TreeViewDesigner(this);
        this.BorderWidth = 1;
        this.Width = 100;
        this.WidthUnit = "%";
        this.MinHeight = 320;
        this.MinWidth = 480;
        this.oninput += text => {
            this.Info(text);
        };
        this.oninput += OnTreeSearchTextChanged;
        this.onsearch += OnTreeSearchTextChanged;
    }

    /// <summary>
    /// Строка поиска в <see cref="SearchControl"/> по умолчанию содержит подсказку «Поиск».
    /// </summary>
    private static string NormalizeTreeSearchText(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";
        var t = raw.Trim();
        if (t.Equals("Поиск", StringComparison.OrdinalIgnoreCase))
            return "";
        return t;
    }

    private void OnTreeSearchTextChanged(string rawQuery)
    {
        RunOnUiThread(() => ApplyTreeSearchFilter(rawQuery));
    }

    /// <summary>
    /// Фильтрация узлов: показываются узлы, чей заголовок содержит любое из слов запроса,
    /// либо имеют такого потомка. Корень остаётся видимым как контейнер.
    /// </summary>
    private void ApplyTreeSearchFilter(string rawQuery)
    {
        var query = NormalizeTreeSearchText(rawQuery);
        if (string.IsNullOrWhiteSpace(query))
        {
            this.root.DoBroadcastToHierElements(pnode =>
            {
                if (pnode is TreeViewNodeModel node)
                    node.Visible = true;
            });
            return;
        }

        var words = query.ToLowerInvariant()
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length == 0)
        {
            this.root.DoBroadcastToHierElements(pnode =>
            {
                if (pnode is TreeViewNodeModel node)
                    node.Visible = true;
            });
            return;
        }

        bool TitleMatches(TreeViewNodeModel node)
        {
            var title = (node.Title ?? "").ToLowerInvariant();
            return words.Any(w => title.Contains(w));
        }

        bool ApplyVisibility(TreeViewNodeModel node)
        {
            var self = TitleMatches(node);
            var anyChildVisible = false;
            foreach (var child in node.Children.ToList())
            {
                if (child is TreeViewNodeModel childNode)
                    anyChildVisible |= ApplyVisibility(childNode);
            }

            var show = self || anyChildVisible;
            if (node == this.root)
                node.Visible = true;
            else
                node.Visible = show;
            return show;
        }

        ApplyVisibility(this.root);
    }


    public TreeViewNodeModel Find(List<int> path)
    {
        var list = new List<int>(path);
        list.RemoveAt(0);
        TreeViewNodeModel p = root;
        foreach (var i in list)
        {
            p = p.Children[i];
        }
        return p;
    }

    public override void BindSearchLine(SearchView searchLine)
    {
        base.BindSearchLine(searchLine);
    }

    protected override void OnDesignModeChanged(PropertyChangedMessage message)
    {
        if (message.Before.ToString().ToBool() != message.After.ToString().ToBool())
        {
            switch (message.After.ToString().ToBool())
            {
                case true:

                    var designer = new TreeViewDesigner(this);
                    designer.Activates(this);

                    break;
                case false: break;
            }
        }

    }

    public static TreeControlView Create(string title)
    {
        var p = new TreeControlView();
        p.root.Children.Add(new()
        {
            Title = "1"
        });
        p.root.Children.Clear();
        p.root.Title = title;
        return p;
    }




    public void BindHierTable<TEntity>(Action<TEntity> onclicked = null) where TEntity : HierTable<TEntity>
    {
        var service = new TreeService();
        var dbset = typeof(TEntity).GetEntityFasade<TEntity>().GetDbSet();
        var rootItem = dbset.FirstOrDefault(p => p.ParentId == null);
        this.root = service.CreateTreeViewNode<TEntity>(dbset, rootItem, onclicked);
        /*this.root = service.CreateTreeViewNode<TEntity>(dbset, (TEntity p) => {
            if (OnClicked is not null)
            {
                OnClicked(p);
            }
        });*/

        this.ToControl();
    }




    public void BindHierTable<TDbContext, TEntity>(Action<TEntity> onclicked = null)
        where TDbContext : DbContextBase
        where TEntity : HierTable<TEntity>
    {
        var service = new TreeService();
        var dbset = typeof(TEntity).GetEntityFasade<TDbContext, TEntity>().GetDbSet();
        var rootItem = dbset.FirstOrDefault(p => p.ParentId == null);
        this.root = service.CreateTreeViewNode<TEntity>(dbset, rootItem, onclicked);
        /*this.root = service.CreateTreeViewNode<TEntity>(dbset, (TEntity p) => {
            if (OnClicked is not null)
            {
                OnClicked(p);
            }
        });*/

        this.ToControl();
    }

    public void BindHierServices(TreeViewNodeModel proot, Action<TreeViewNodeModel> value)
    {
        value(proot);
        foreach (var p in proot.Children)
        {
            BindHierServices(p, value);
        }
    }
    public void BindHierServices(Action<TreeViewNodeModel> value)
    {
        value(root);
        foreach (TreeViewNodeModel p in root.Children)
        {
            BindHierServices(p, value);
        }
    }

    [NotInput]
    [InputOrder(4)]
    [NotMapped]
    [Label("Подсказка при вводе")]
    [InputIcon()]
    public virtual string? Help { get; set; }

    [NotInput]
    [Label("Иконка")]
    [NotMapped]
    [InputOrder(3)]
    public virtual string? Icon { get; set; }

    [Label("Надпись")]
    [InputText]
    [InputOrder(2)]
    public virtual string? Label { get; set; }


    [Label("Описание")]
    [InputOrder(5)]
    [InputMultilineText()]
    //[InputRusText()]
    public virtual string? Description { get; set; }
     

    public TreeControlView CreateClassTree(Type rootType, Action<Type> onclicked)
    {
        var tree = ((ITreeFactory)_treeFactory).CreateClassTree(rootType, onclicked);
        this.root = tree.root;
        return this;
    }

    public TreeControlView CreateClassTreeView<TBase>(Action<Type> onclicked)
    {
        var tree = ((ITreeFactory)_treeFactory).CreateClassTreeView<TBase>(onclicked);
        this.root = tree.root;
        return this;
    }

    public TreeControlView CreateClassTreeViewWithGeneration(Type rootType, Action<Type> onclicked)
    {
        var tree = ((ITreeFactory)_treeFactory).CreateClassTreeViewWithGeneration(rootType, onclicked);
        this.root = tree.root;
        return this;
    }

    public TreeControlView CreateClassTreeViewWithGeneration<TBase>(Action<Type> onclicked)
    {
        var tree = ((ITreeFactory)_treeFactory).CreateClassTreeViewWithGeneration<TBase>(onclicked);
        this.root = tree.root;
        return this;
    }

    public List<TreeViewNodeModel> CreateNodes<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked)
    {
        var nodes = ((ITreeFactory)_treeFactory).CreateNodes(items, createNode, getChildren, onclicked);

        return nodes;
    }

    public TreeControlView CreateTree<TEntity>(IEnumerable<TEntity> items, Func<TEntity, TreeViewNodeModel> createNode, Func<TEntity, IEnumerable<TEntity>> getChildren, Action<TEntity> onclicked)
    {
        var tree = ((ITreeFactory)_treeFactory).CreateTree(items, createNode, getChildren, onclicked);
        this.root = tree.root;
        return this;
    }
}
