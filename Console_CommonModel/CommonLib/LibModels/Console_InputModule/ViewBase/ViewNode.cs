using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Label("Узел в иерархии представлений")]
public class ViewNode : ViewOptions, IViewNode
{
    public static string ROOT_INDEX = "*";
    public List<string> GetNamesPath()
    {
        List<string> path = new List<string>();
        var p = this;
        while (p != null)
        {
            path.Add(p.GetTypeName());
            p = p.Parent;

        }
        path.Reverse();
        return path;
    }


    public string NodeName { get; set; }

    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public ViewNode Parent { get; set; } = null;

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]    
    public virtual ChangingList<ViewNode> Controls { get; set; } = new();

    /// <summary>
    /// Когда текущее представление расмещается на другом
    /// </summary>
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]    
    public Action<ViewNode> OnLayout { get; set; } = parent => { };

    /// <summary>
    /// Когда текущее представление расмещается на другом
    /// </summary>
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<ViewNode> OnLayoutLeft { get; set; } = parent => { };

    /// <summary>
    /// Когда в текущее представление расмещаются на другие
    /// </summary>
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<ViewNode> OnAppendedChild { get; set; } = parent => { };


    /// <summary>
    /// Когда из текущего представления извлекают другое
    /// </summary>
    [NotInput]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<ViewNode> OnRemovedChild { get; set; } = parent => { };



    /// <summary>
    /// Сервисы активируются на каждого потомка при событии OnLayout
    /// </summary>    
    public virtual IEnumerable<DesignerBase> GetLayoutDesignServices()
    {
        return new List<DesignerBase>()
        {
            //new ViewItemContextMenuService((ViewItem)this),
        };


    }

    [Description("Активируются ниже по иерахии по событию OnLaoyut")]
    public ChangingList<DesignerBase> ViewServices { get; set; } = new();
    public string Name { get; set; }

    public ViewNode() : base()
    {
        this.Controls = new();
        this.Controls.OnAdded((s, evt) => { OnAppendedChild((ViewNode)evt); });
        this.Controls.OnRemoved((s, evt) => { OnRemovedChild((ViewNode)evt); });
        
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    protected override void OnDesignModeChanged(PropertyChangedMessage message)
    {
        foreach (var ctrl in this.Controls)
        {
            ctrl.DesignMode = message.After.ToString().ToBool();
        }
    }


    /// <summary>
    /// Рекурсивный вызов 
    /// </summary>
    /// <param name="action"></param>
    public void DoUpper(Action<ViewNode> action)
    {
        foreach (ViewNode p in Controls)
        {
            p.DoUpper(action);
        }
        action(this);
    }

    /// <summary>
    /// Рекурсивный вызов 
    /// </summary>
    /// <param name="action"></param>
    public void Do(Action<ViewNode> action)
    {

        action(this);
        foreach (ViewNode p in Controls)
        {
            p.Do(action);
        }
    }


    //public event EventHandler OnAppended;
    public void AppendRange(IEnumerable<ViewNode> children)
    {
        foreach(var p in children)
        {
            Append(p);
        }        
    }
    
    public ViewNode Append(ViewNode pchild)
    {
        try
        {
            if (pchild.Parent == this)
            {
                return pchild;
            }
            if (pchild.Parent != null)
            {
                pchild.RemoveFromParent();
            }
            pchild.Parent = this;
            
            Controls.Add(pchild);
            this.Changed = true;
        }
        catch (Exception ex)
        {
            this.Error(ex);
        }

        return pchild;
    }
    public ViewNode Append(string name, ViewNode pchild)
    {
        if (pchild.Parent == this)
        {
            return pchild;
        }
        if (pchild.Parent != null)
        {
            pchild.RemoveFromParent();
        }
        pchild.Parent = this;
        Controls.Add(pchild);
        this.Changed = true;

        return pchild;
    }

    //public event EventHandler OnRemoved;
    public virtual void RemoveFromParent()
    {
        if (Parent == null)
        {
            return;
        }
        this.Info(GetType().Name + " Children Node Removed " + GetHashCode());
        if (this.Parent != null)
        {
            var p = this.Parent;
            this.Parent.Controls.Remove(this);
            p.Changed = true;
            this.Parent = null;
        }
    }


    //public event EventHandler OnDropped;
    public void Drop(ViewNode draggable)
    {
        this.Info(GetType().Name + " Children Node Dropped " + GetHashCode());
        draggable.RemoveFromParent();
        Append(draggable);
    }


    public void SetParent(ViewNode parent)
    {
        if (this.Parent != null)
        {
            this.RemoveFromParent();
        }
        this.Parent = parent;
        if (this.Parent.Controls == null)
        {
            this.Parent.Controls = new();
        }

        this.Parent.Controls.Add(this);
        this.Parent.Changed = true;
    }

    public ViewNode Find(List<string> path)
    {
        var p = GetRoot();

        foreach (string s in path)
        {
            if (string.IsNullOrEmpty(s) == false)
            {
                System.Console.WriteLine(s);
                int index = int.Parse(s);
                if (Controls.Count() > index)
                {
                    p = (ViewNode)this.Controls[index];
                }
                else
                {
                    throw new Exception($"Не найти {path}");
                }
            }
        }
        return p;
    }

    public ViewNode Find(string path)
    {
        var p = GetRoot();
        if (path == "root")
        {
            if (GetRoot() != this)
            {
                throw new Exception("Не правильно задан индекс");
            }
            else
            {
                return GetRoot();
            }
        }
        path = path.Replace("root-", "");
        foreach (string s in path.Split("[-]"))
        {
            if (string.IsNullOrEmpty(s) == false)
            {
                System.Console.WriteLine(s);
                int index = int.Parse(s);
                if (Controls.Count() > index)
                {
                    p = (ViewNode)Controls[index];
                }
                else
                {
                    throw new Exception($"Не найти {path}");
                }
            }
        }
        return p;
    }


    public void ForEach(Action<ViewNode> action)
    {
        action(this);
        foreach (ViewNode p in Controls)
        {
            p.ForEach(action);
        }
    }


    public ViewNode GetRoot()
    {
        var p = this;
        while (p.Parent != null)
        {
            p = p.Parent;
        }
        return p;
    }

    public virtual List<string> GetPath()
    {
        List<string> path = new List<string>();
        var p = this;
        while (p != null)
        {
            path.Add(p.GetLocalIndex());
            p = p.Parent;

        }
        path.Reverse();
        return path;
    }

    private string GetLocalIndex()
    {
        if (Parent != null)
        {
            return Parent.Controls.IndexOf(this) + "";
        }
        return "*";
    }


    /// <summary>
    /// Обработка узлов поддерева вертикально вниз
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="handle"></param>    
    public virtual void DoBroadcastToHierElements(Action<ViewNode> handle)
    {
        handle(this);
        foreach (ViewNode pchild in Controls)
        {
            pchild.DoBroadcastToHierElements(handle);
        }
    }




    /// <summary>
    /// Обработка узлов поддерева снизу вверх
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="handle"></param>
    public void DoBroadcastFromHierElements<TNode>(Action<TNode> handle) where TNode : ViewNode
    {
        foreach (ViewNode pchild in Controls)
        {
            pchild.DoBroadcastFromHierElements<TNode>(handle);
        }
        handle((TNode)this);
    }


    /// <summary>
    /// Обход всей иерархии
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="handle"></param>
    public void DoBroadcastToParent<TNode>(Action<TNode> handle) where TNode : ViewNode
    {
        if (this is TNode)
        {
            handle((TNode)this);
        }
        if (Parent != null)
        {
            //((AppViewBase)Parent).Trace();
            Parent.DoBroadcastToParent<TNode>(handle);
        }
    }


    /// <summary>
    /// Обработка узлов поддерева сверху вниз
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    /// <param name="handle"></param>
    public void DoBroadcastToBrothers<TNode>(Action<TNode> handle) where TNode : ViewNode
    {
        if (Parent == null)
        {
            if(this is TNode)
            {
                handle((TNode)this);
            }
            
        }
        foreach (ViewNode pchild in Controls)
        {
            if (pchild is TNode)
            {
                handle((TNode)pchild);
            }
            
        }
        foreach (ViewNode pchild in Controls)
        {
            pchild.DoBroadcastToBrothers<TNode>(handle);
        }
    }

    internal Point PointToClient(object mousePosition)
    {
        throw new NotImplementedException();
    }
}

