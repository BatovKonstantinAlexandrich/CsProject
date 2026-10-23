using Console_CommonModel.CommonLib.LibData.AbstractionModel;

[Label("Список выбора")]
public class TableView<TEntity> : TableView
    where TEntity : BaseEntity
{
    public TableView()
    {
        this.Height = this.Width = 125;
        this.MinHeight = this.MinWidth = 125;
    }
}
