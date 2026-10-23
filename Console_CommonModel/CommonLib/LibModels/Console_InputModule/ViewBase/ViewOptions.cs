
/// <summary>
/// Свойства управляющие связыванием с управлением
/// </summary>
[Label("Свойства представления")]
public class ViewOptions: ActiveItem
{

    [Label("Разрешить фокусирование")]
    public bool Loaded { get; set; } = false;


    [Label("Разрешить фокусирование")]
    public bool Focusable { get => Get<bool>("Focusable"); set => Set<bool>("Focusable", value); }

    [Label("Фокусирование включено")]
    public virtual bool Focused { get => Get<bool>("Focused"); set => Set<bool>("Focused", value); }

    [Label("Разрешить выделение")]
    public bool Selectable { get => Get<bool>("Selectable"); set => Set<bool>("Selectable", value); }

    [Label("Выделение включено")]
    public bool Selected { get => Get<bool>("Selected"); set => Set<bool>("Selected", value); }
    public bool MultiSelect { get => Get<bool>("Selected"); set => Set<bool>("Selected", value); }

    [Label("Разрешить помещать перетаскиваемые елементы")]
    public bool Dropable { get => Get<bool>("Dropable"); set => Set<bool>("Dropable", value); }



    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action<object, int,int> DoDrop = (text,x,y) => { };

    public void SetDroppable(Action<object, int, int> drop)
    {
        if (drop is null)
            throw new ArgumentNullException(nameof(drop));
        this.Dropable = true;
        this.DoDrop = drop;
    }


    [Label("Разрешить перетаскивание")]
    public bool Draggable { get => Get<bool>("Draggable"); set => Set<bool>("Draggable", value); }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Func<object, object> DoDrag = (item) => item.GetTypeName();
    public void SetDraggable(Func<object, object> drag)
    {
        if (drag is null)
            throw new ArgumentNullException(nameof(drag));
        this.Draggable = true;
        this.DoDrag = drag;
    }
    public void SetDropable(Action<object, int,int> dodrop)
    {
        if (dodrop is null)
            throw new ArgumentNullException(nameof(dodrop));
        this.Dropable = true;
        this.DoDrop += dodrop;
    }

    [Label("Разрешить редактирование")]
    public virtual bool Editable { get => Get<bool>("Editable"); set => Set<bool>("Editable", value); }

    [Label("Разрешить поиск")]
    public virtual bool Searchable { get => Get<bool>("Searchable"); set => Set<bool>("Searchable", value); }
    public void Search(string query)
    {
        if (Searchable)
        {
            
        }
    }

    [Label("Разрешить выделение галками")]
    public virtual bool Checkable { get => Get<bool>("Checkable"); set => Set<bool>("Checkable", value); }

    [Label("Выделено галками")]
    public virtual bool Checked { get => Get<bool>("Checked"); set => Set<bool>("Checked", value); }

    [Label("Видимость")]
    public virtual bool Visible { get => Get<bool>("Visible"); set => Set<bool>("Visible", value); }

    [Label("Развернуто")]
    public virtual bool Expanded { get => Get<bool>("Expanded"); set => Set<bool>("Expanded", value); }

    [Label("Редактируется")]
    public virtual bool Edited { get => Get<bool>("Edited"); set => Set<bool>("Edited", value); }


    public ViewOptions(): base()
    {
        this.MultiSelect = false;
        this.Selectable = false;
        this.Dropable = false;
        this.Draggable = false;
        this.Focusable = true;
        this.Searchable = false;
        this.Checkable = false;
        this.Editable = true;
        this.Focused = false;
        this.Selected = false;
        this.Edited = false;
        this.Expanded = false;
        this.Checked = false;
        this.Visible = true; 
        this.AddPropertyChangeListener(nameof(Draggable), OnDraggableChanged);
        this.AddPropertyChangeListener(nameof(Dropable), OnDropableChanged);
        this.OnLoad += (p, e) => { 
            this.Loaded = true;
        };
    }
    private void OnDropableChanged(PropertyChangedMessage message)
    {
       
    }

    private void OnDraggableChanged(PropertyChangedMessage message)
    {

    }


    public string GetOptionsClasses() {
        var classList = new List<string>();
        if (Dropable) classList.Add("dropable");
        if (Draggable) classList.Add("draggable");
        if (Selectable) classList.Add("selectable");
        if (Focusable) classList.Add("focusable");
        if (Searchable) classList.Add("searchable");
        if (Focusable) classList.Add("focusable");
        if (Focusable) classList.Add("focusable");
        if (Editable) classList.Add("editable");
        if (Focusable) classList.Add("focusable");
        if (Focused) classList.Add("focused");
        if (Selected) classList.Add("selected");
        if (Edited) classList.Add("edited");
        if (Expanded) classList.Add("expanded");
        if (Checked) classList.Add("checked");
        if (Visible) classList.Add("visible");
        string _class = "";
        classList.ForEach(c => { _class += " " + c; });
        if (_class.Length > 0) _class = _class.Substring(1);
        return _class;
    }

    public virtual void Focus()
    {
        if( Focusable)
        {
            Focused = Focused == true ? false : true;
            Changed = true;
        }
        
    }


    public virtual void Check()
    {
        if (Checkable)
        {
            Checked = Checked == true ? false : true;
            Changed = true;
        }        
    }


    public virtual void Edit()
    {
        if( Editable)
        {
            Edited = Edited == true ? false : true;
            Changed = true;
        }        
    }
    

    public virtual void Select()
    {
        if (Selectable)
        {
            Selected = Selected == true ? false : true;
            Changed = true;
            SendEvent(new { 
                Type="Select",
                Source=this,
                Value=Selected
            });            
        }            
    }

    public virtual void Expand()
    {        
        Expanded = Expanded == true ? false : true;
        Changed = true;
    }

    public virtual ViewOptions SetSelectable(bool value)
    {
        Selectable = value;
        return this;
    }

    public virtual ViewOptions SetSelectionMode(bool multi)
    {
        MultiSelect = multi;
        return this;
    }

    public virtual ViewOptions SetCheckable(bool value)
    {
        Checkable = value;
        return this;
    }

    public virtual void ToggleEditable()
    {
        Edited = Edited ? false : true;
    }

    public virtual ViewOptions SetEdited(bool edited)
    {
        this.Edited = edited;
        
        return this;
    }

    public virtual ViewOptions SetEditable(bool editable)
    {
        this.Editable = editable;
        return this;
    }
    public virtual ViewOptions SetNotEditable()
    {        
        Editable = false;
        Changed = true;
        return this;
    }   
    public virtual ViewOptions SetEditMode()
    {
    
        Edited = true;
        Changed = false;
        return  this;
    }
    public virtual ViewOptions Hide()
    {
        Visible = false;
        return this;
    }     
}

