
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


[Icon("view_headline")]
[Label("Список выбора")]
public class ListSelectionView: StructuredCollectionView
{    

    
    [Label("Иконка")]
    public string Icon { get => Get<string>("Icon"); set => Set<string>("Icon", value); }


    public virtual List<ViewItem> ListItems 
    { 
        get
        {
            
            return ItemsList;
        }
        set
        {
            this.ItemsList = value;
        }
    }

    public ListSelectionView() : base()
    {
         
        InitDefaultStyle();
        Selectable = true;
        Searchable = true;
        Type = typeof(UserPerson);
        Chapter = "List";
        ListItems = new List<ViewItem>();
        Bindings = null;
        ListItems.Add(new ListItemView() { Title = "Item1" });
        ListItems.Add(new ListItemView() { Title = "Item2" });
        this.AddEditTools();
        Changed = false;
        this.MinHeight = 320;
        this.MinWidth = 320;
    }


    public ListSelectionView(params object[] items) : base() {
        ListItems.Clear();
        foreach(var item in items)
        {
            ListItems.Add(new ListItemView() { Title = item.ToString() });
        }
        this.MinHeight = 320;
        this.MinWidth = 320;
    }



    private void InitDefaultStyle()
    {
        Icon = "home";
             
    }

    public ListSelectionView AddAction(string Label, Action<global::ListView> OnClick )
    {
         
        return this;
    }
    
     




}