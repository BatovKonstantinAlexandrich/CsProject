
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;

/// <summary>
/// Карточка представления сущности
/// </summary>
/// <typeparam name="TEntity">тип сущности</typeparam>
public class Card<TEntity> : SingleView<TEntity>
    where TEntity : BaseEntity
{
    [Label("Заголовок")]
    [InputRusText()]
    [NotNullNotEmpty()]
    public string Title { get; set; }

    [Label("URL изображения")]
    [InputUrl()]
    [NotNullNotEmpty()]
    public string Image { get; set; }

    [Label("Действия")]
    [InputPrimitiveCollection()]
    public List<string> Actions { get; set; } = new() {};

    [Label("Ссылка на сущность")]
    private TEntity Item { get; set; }

    public Card(): base(){}
    public Card(TEntity item): base(item)
    {
        if(item is null) throw new ArgumentNullException("item");
        this.dataModel = this.Item = item;
    }

    protected override void ApplyViewModel(TEntity after)
    {
    }
}