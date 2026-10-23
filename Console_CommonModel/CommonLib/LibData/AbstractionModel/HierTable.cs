using System.ComponentModel;
using System.Text.Json.Serialization;

public class HierTable: DictionaryTable
{
    [Label("Корневой каталог")]
    [DisplayName("Корневой каталог")]
    [NotInputAttribute("")]
    public virtual int? ParentId { get; set; }
}

[Label("Иерархический справочник")]
public class HierTable<T> : HierTable, IHierTable<T>
{
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInputAttribute()]
    [InputHidden]
    [Label("Ссылка на родителя")]
    public virtual T Parent { get; set; }

    public virtual string GetPath(string separator)
    {

        HierTable<T> parentHier = ((HierTable<T>)((object)Parent));
        return (Parent != null) ? parentHier.GetPath(separator) + separator + Name : Name;
    }

}
