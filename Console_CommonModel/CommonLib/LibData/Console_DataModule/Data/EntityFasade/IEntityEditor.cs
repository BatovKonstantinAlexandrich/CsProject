using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IEntityEditor<TEntity> where TEntity : class
{
    public void AddColumn(ColumnMetadata column);
    public void AddOne<TEntity>();
    public void AddMany<TEntity>();
    public void AddManyToMany<TEntity>();

}