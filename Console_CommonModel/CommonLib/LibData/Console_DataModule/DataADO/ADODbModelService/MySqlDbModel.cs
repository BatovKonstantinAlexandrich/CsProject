using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService
{
    public class MySqlDbModel : MySqlDbMetadata, IdbModel
    {
        public ChangingList<Type> EntityTypes { get; set; }

        public MySqlDbModel() : base()
        {
            EntityTypes = new ChangingList<Type>();
        }

        public MySqlDbModel(string server, string database) : base(server, database)
        {
            EntityTypes = new ChangingList<Type>();
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
