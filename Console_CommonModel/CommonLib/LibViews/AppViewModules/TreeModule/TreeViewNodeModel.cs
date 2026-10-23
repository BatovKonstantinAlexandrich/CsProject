using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.ComponentModel.DataAnnotations.Schema;

[Label("Узел иерархии")]
public class TreeViewNodeModel : AppViewBase
{
    public override string Trace()
    {
        return this.Trace(1, this);
    }

    protected virtual string Trace(int level, ViewNode p)
    {
        string text = "";
        for (int i = 0; i < level; i++)
        {
            System.Console.Write("  ");
            text += "  ";
        }
        text += p.GetTypeName() + " " + p.GetHashCode() + " " + p.Dropable + " " + $"[{p.Cast<AppViewBase>().Width},{p.Cast<AppViewBase>().Height}]\n";
        System.Console.WriteLine(text);
        if (p is PanelsView pv)
        {
            if (pv.Left is not null)
            {
                text += Trace(level + 1, pv.Left);
            }
            if (pv.Top is not null)
            {
                text += Trace(level + 1, pv.Top);
            }
            if (pv.Bottom is not null)
            {
                text += Trace(level + 1, pv.Bottom);
            }
            if (pv.Right is not null)
            {
                text += Trace(level + 1, pv.Right);
            }
        }

        foreach (var child in ((TreeViewNodeModel)p).Children)
        {
            text += Trace(level + 1, child);
        }
        return text;
   
    }

    
    [NotInput]
    [Label("Tree Node")]
    public object TreeNode { get; set; }
    public TreeControlView GetTreeControlView()
    {
        ViewNode p = this;
        while(p is not null)
        {
            if(p is TreeControlView c)
            {
                return c;
            }
            p = p.Parent;
        }
        throw new Exception("Не удалось найти TreeControlView");
    }

    
    [NotMapped]
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("Parent Node")]
    public TreeViewNodeModel ParentNode { get; set; }


    [Label("Иконка")]
    [InputIcon()]
    public string Icon { get; set; }

    [Label("Заголовок")]
    [InputRusText()]
    public string Title { get; set; }

    [InputConstructor<BaseEntity>()]
    [Label("Данные")]
    public object Item { get; set; }


    [NotInput]
    private ChangingList<TreeViewNodeModel> _Children; 
    public ChangingList<TreeViewNodeModel> Children
    {
        get
        {
            return _Children;
        } 
        set
        {
            _Children = value;
            _Children.OnAdded((i, p) => {
                ((TreeViewNodeModel)p).ParentNode = this;
                ((TreeViewNodeModel)p).Parent = this;
            });
            foreach(var pchild in value)
            {
                ((TreeViewNodeModel)pchild).ParentNode = this;
                ((TreeViewNodeModel)pchild).Parent = this;
            }
        }
    }

    public TreeViewNodeModel(): base()
    {
        this.Children = new ChangingList<TreeViewNodeModel>();  
    }

    public override void DoBroadcastToHierElements(Action<ViewNode> handle)
    {
        handle(this);
        foreach (ViewNode pchild in Children.ToList())
        {
            pchild.DoBroadcastToHierElements(handle);
        }
    }

    public void AppendTreeViewNode(IEnumerable<TreeViewNodeModel> children)
    {
        foreach (var child in children)
        {
            AppendTreeViewNode(child);
        }
    }

    public TreeViewNodeModel AppendTreeViewNode(TreeViewNodeModel pchild)
    {

        try
        {
            if (pchild.ParentNode == this)
            {
                return pchild;
            }
            if (pchild.ParentNode != null)
            {
                pchild.RemoveFromParent();
            }
            pchild.ParentNode = this;

            this.Children.Add(pchild);
            this.Changed = true;
        }
        catch (Exception ex)
        {
            this.Error(ex);
        }

        return pchild;
    }


}
