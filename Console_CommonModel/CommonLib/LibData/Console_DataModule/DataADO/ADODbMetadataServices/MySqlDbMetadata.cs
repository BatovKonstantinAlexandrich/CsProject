using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices
{
    [Label("Сервис управления метаданными MySQL")]
    public class MySqlDbMetadata : MySqlExecutor, IDbMetadata
    {
        public MySqlDbMetadata()
        {
        }
        public MySqlDbMetadata(string server, string database) : base(server, database)
        {

        }
        public IEnumerable<string> GetTableNames()
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, TableMetadata> TablesMetadata;

        public IDictionary<string, TableMetadata> GetTablesMetadata()
        {
            this.Info($"GetTablesMetadata()");
            lock (this)
            {
                if (TablesMetadata == null)
                {
                    TablesMetadata = InitTablesMetadata();
                }
            }
            return TablesMetadata;
        }

        public IDictionary<string, TableMetadata> UpdateTablesMetadata()
        {
            this.Info($"UpdateTablesMetadata()");
            lock (this)
            {
                TablesMetadata = InitTablesMetadata();
            }
            return TablesMetadata;
        }

        private Dictionary<string, TableMetadata> InitTablesMetadata()
        {
            throw new NotImplementedException("InitTablesMetadata для MySQL требует реализации");
        }

        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyMetadata> GetKeysMetadata()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema)
        {
            throw new NotImplementedException();
        }

        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSchemasNames()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata()
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetDatabaseNames()
        {
            throw new NotImplementedException();
        }

        public List<string> GetProcedureSql(string schema, string name)
        {
            throw new NotImplementedException();
        }
    }
}
