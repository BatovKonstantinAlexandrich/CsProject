using Newtonsoft.Json;
using System.Collections;
/*
[Label("Комбинированные представления")]
public class ViewItemSet: ViewItem
{
    public string View { get; set; } = "Tabs";

    [JsonIgnore()]
    public List<ViewItemSetColumn> Columns { get; set; } = new List<ViewItemSetColumn>();

    [JsonIgnore()]
    public List<FormViewModel> Items { get; set; } = new List<FormViewModel>();

    /// <summary>
    /// 
    /// </summary>
    public class ViewItemSetColumn : FormField
    {
        [JsonIgnore()]
        public ViewItemSet ViewItemSet { get; set; }
        public List<object> GetValues(IEnumerable items)
        {
            List<object> values = new List<object>();
            foreach (var item in items)
            {
                object value = ReflectionService.GetValueFor(item, this.Name);
                values.Add(value);
            }
            return values;
        }
    }
    public ViewItemSet() { }


    public object GetItem( int index )
    {
        return Items.ToArray()[index];
    }

    public object Add(global::FormViewModel item)
    {
        Items.Add(item);
        return item;
    }

    public ViewItemSet ShowFieldsByName( params string[] names )
    {
        var visibleColumnsNamesList = new List<string>(names);
        Columns.ForEach((field) => { field.Visible = visibleColumnsNamesList.Contains(field.Name); });
        return this;
    }



    public ViewItemSet ToTableView()
    {
        View = "Table";
        return this;
    }


    public override ViewItem IsNotForUpdate()
    {
        base.IsNotForUpdate();
        Changed = false;
        return this;
    }

    public override ViewOptions SetEdited(bool edited)
    {
        base.SetEdited(edited);
        Items.ForEach(f=> {
            f.SetEdited(edited);
        });        
        return this;
    }
}*/
 
