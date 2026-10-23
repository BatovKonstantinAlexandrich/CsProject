using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

[Icon("account_tree")]
[Label("Модель иерархического преставления")]
public class TreeViewModel: AppViewBase
{
    
    
    [Label("Item")]
    public NamedObject Item { get; set; }

    [Label("Заголовок")]
    [InputRusText()]
    public string Title { get; set; }

    public bool Changed
    {
        get
        {
            return false;
        }
        set
        {

        }
    }
    public int GetLevel() { return 1;  }

    [InputUrl()]
    [Label("Image")]
    public string Image { get; set; }


    public TreeViewModel():base() 
    {
       // this.Info(GetType().Name + " Created " + GetHashCode());
        Item = new NamedNode() { Name = "Новый каталог" };
        Selectable = true;
        Expanded = true;
        Init();        
    }

    public class NamedNode: NamedObject
    {

    }

    public TreeViewModel(string name) : base() {
        // this.Info(GetType().Name + " Created " + GetHashCode());
        Item = new NamedNode() { Name = name };
        Selectable = true;
        Expanded = true;
        Init();
    }


    public TreeViewModel(TreeViewModel parent, NamedObject item)
    {
        //this.Info(GetType().Name + " Created" + GetHashCode());
        Item = item;
        Selectable = true;
        Expanded = true;
        SetParent(parent);
        Init();
    }

    public override void Init() {

        base.Init();
        var ctrl = this;
    }


    public List<List<object>> ToOrgChart()
    {        
        List<List<object>> items = new List<List<object>>() {   };
        string pname = (Parent != null) ?
            ((NamedObject)(((TreeViewModel)Parent).Item)).Name :
            (((NamedObject)Item).Name);
        
        items.Add(new List<object>() { (((NamedObject)Item).Name), pname, "" });
        Controls.ToList().ForEach(p => {
            items.AddRange(((TreeViewModel)p).ToOrgChart());
        });
        return items;


    }

    public List<List<object>> ToOrgHighChart() {
        List<List<object>> items = new List<List<object>>() { };
        string pname = (Parent != null && ((TreeViewModel)Parent).Item!= null && ((NamedObject)(((TreeViewModel)Parent).Item)).Name != null) ?
            ((NamedObject)(((TreeViewModel)Parent).Item)).Name :
            Item.GetType().GetLabel();

        items.Add(new List<object>() { (((NamedObject)Item).Name), pname });
        Controls.ToList().ForEach(p => {
            items.AddRange(((TreeViewModel)p).ToOrgHighChart());
        });
        return items;


    }
    protected void On(dynamic message)
    {
        switch (message.Type)
        {
            case "Select": 
                if( message.Value == true && MultiSelect == false )
                {
                    GetSelected().ForEach(p => p.Selected = false);
                    SelectionModel.Clear();
                    SelectionModel.Add(message.Source);
                }
                break;
            default: throw new Exception("События типа "+message.Type+" не поддерживаются");
        }
    }


    public object GetItem()
    {
        return Item;
    }


    public List<ViewNode> GetSelected()
    {
        List<ViewNode> selected = new List<ViewNode>();
        this.Do((p) => {
            if (p.Selected)
            {
                selected.Add(p);
            }
        });
        return selected;
    }



    public List<ViewNode> GetChecked()
    {
        List<ViewNode> selected = new List<ViewNode>();
        this.Do((p) => {
            if (p.Checked)
            {
                selected.Add(p);
            }
        });
        return selected;
    }


    




    public void Create()
    {
        this.Info(GetType().Name + " Children Node Created " + GetHashCode());
        new TreeViewModel(this, new NamedNode() { Name = "Новый элемент" });
    }



    public void Search( string query )
    {
        this.Info(GetType().Name + " Children Node Created " + GetHashCode());
        new TreeViewModel(this, new NamedNode() { Name = "Новый элемент" });
    }

    public void AppendRange(System.Collections.Generic.ICollection<TreeViewModel> children )
    {
        foreach(var p in children)
        {
            base.Append(p);
        }
    }

     
    public override string ToString() {
        string s = "";
        Controls.ToList().ForEach(c => s += c.ToString() + ",");
        return Item.Name + "[" +s+ "]";
    }
   
}