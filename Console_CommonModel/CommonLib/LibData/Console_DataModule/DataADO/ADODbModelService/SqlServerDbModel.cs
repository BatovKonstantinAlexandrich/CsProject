using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbModelService
{
    [Label("Структура базы данных")]
    public class SqlServerDbModel : SqlServerDbMetadata, IdbModel
    {

        [Label("Entity Types")]
        public ChangingList<Type> EntityTypes { get; set; }

        public SqlServerDbModel() : base()
        {
            EntityTypes = new();
        }

        public SqlServerDbModel(string server, string database) : base(server, database)
        {
            EntityTypes = new();
        }

        public SqlServerDbModel(string server, string database, bool trustedConnection, string userId, string password) : base(server, database, trustedConnection, userId, password)
        {
            EntityTypes = new();

        }

        public Type[] GetEntityClasses() => EntityTypes.ToArray();
        public void AddEntityType<TEntity>() => EntityTypes.Add(typeof(TEntity));
        public void AddEntityType(Type entity) => EntityTypes.Add(entity);



        public string GetCurrentVersion()
        {
            return UserAccount.GetHashSha256(new
            {
                tables = GetTablesMetadata(),
                procedures = GetProceduresMetadata()
            }.ToJsonOnScreen());
        }

        public string GetTargetVersion()
        {


            return UserAccount.GetHashSha256(new
            {
                tables = GetTablesMetadata(),
                procedures = GetProceduresMetadata()
            }.ToJsonOnScreen());
        }
    }
}
