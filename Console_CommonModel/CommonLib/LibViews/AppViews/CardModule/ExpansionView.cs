using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Microsoft.EntityFrameworkCore.Metadata;

[Label("Панель раскрытия сущности")]
[Description("Карточка редактирования объекта с навигацией по свойствам")]
public class ExpansionView: AppViewBase
{


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    public IEnumerable<INavigation> Navigation { get; set; }

    [InputConstructor<BaseEntity>()]
    public object Item { get => Get<object>(nameof(Item)); set => Set<object>(nameof(Item), value); }



    public string ActivePropertyName { get => Get<string>(nameof(ActivePropertyName)); set => Set<string>(nameof(ActivePropertyName), value); }

    [Label("Свойства")]
    [InputPrimitiveCollection(nameof(System.String))]
    public ChangingList<string> Properties { get => Get<ChangingList<string>>(nameof(Properties)); set => Set<ChangingList<string>>(nameof(Properties), value); }

    [OnChange(nameof(Item))]
    public void OnItemChanged()
    {
        this.Properties = this.Item is not null ? new(this.Item.GetType().GetOwnInputPropertyNames()) : new();
    }


    [InputBool]
    public bool IsCollection { get; set; } = false;

    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    public object Active
    {
        get
        {
            if (ActivePropertyName == null)
            {
                ActivePropertyName = Navigation.First().Name;
            }
            return (Item == null || ActivePropertyName == null) ? null :
                    new ReflectionService().GetValue(Item, ActivePropertyName);
        }
    }

    public bool SetActive(string Name)
    {
        ActivePropertyName = Name;
        Changed = true;
        IsCollection = AttrsUtil.IsCollection(Item.GetType(), Name);
        return true;
    }



    public ExpansionView() : base()
    {
        this.Changed = false;
        this.Properties = new();
        this.ActivePropertyName = null;
        this.Item = new object();
        this.MinHeight = 320;
        this.MinWidth = 320;
    }


    public ExpansionView(IEnumerable<string> properties, object item)
    {
        Item = item;
        Navigation = AttrsUtil.GetNavigation(Item.GetType());
        ActivePropertyName = Navigation.First().Name;
        this.Properties = new(properties);
        Changed = false;
        this.MinHeight = 320;
        this.MinWidth = 320;
    }

    public void Add(ViewNode item)
    {
        this.Controls.Add(item);
    }

    public void Clear()
    {
        this.Controls.Clear();
    }

    public int Count()
    {
        return this.Controls.Count();
    }

    public void Remove(ViewNode item)
    {
        this.Controls.Remove(item);
    }

    internal TextView CreateTextView(string text)
    {
        var textView = new Console_CommonModel.CommonLib.LibViews.AppViews.TextModule.TextView();
        return textView;
    }
}
