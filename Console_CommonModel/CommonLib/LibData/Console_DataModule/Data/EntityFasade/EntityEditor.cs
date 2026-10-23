using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.EntityFasade
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityEditor<T> : IEntityEditor<T> where T : class
    {
        private TableMetadataBuilder builder = new();




        public void AddColumn(ColumnMetadata column)
        {

        }

        public void AddOne<TEntity>()
        {
        }

        public void AddMany<TEntity>()
        {
        }

        public void AddManyToMany<TEntity>()
        {
        }
    }
}
