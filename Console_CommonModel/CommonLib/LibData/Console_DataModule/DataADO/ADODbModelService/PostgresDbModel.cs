using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService
{
    [Label("Модель данных Postgres")]

    public class PostgresDbModel : PostgresDbMetadata, IdbModel
    {
        public ChangingList<Type> EntityTypes { get; set; }

        public PostgresDbModel() : base()
        {
            EntityTypes = new ChangingList<Type>();
        }
        public PostgresDbModel(string dataSource, int port, string database, string userId, string password)
          : base(dataSource, port, database, userId, password)
        {
        }
        public Type[] GetEntityClasses() => EntityTypes.ToArray();

        public void AddEntityType<TEntity>() => EntityTypes.Add(typeof(TEntity));

        public void AddEntityType(Type entity) => EntityTypes.Add(entity);

        public string GetCurrentVersion()
        {
            throw new NotImplementedException();
        }

        public string GetTargetVersion()
        {
            throw new NotImplementedException();
        }


    }
}
