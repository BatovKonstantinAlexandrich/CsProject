using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices
{
    public interface IDbMetadata
    {
        public IEnumerable<string> GetTableNames();
        public IEnumerable<string> GetDatabaseNames();

        public IEnumerable<string> GetSchemasNames();
        public IDictionary<string, TableMetadata> GetTablesMetadata();
        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName);
        public IEnumerable<KeyMetadata> GetKeysMetadata();
        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata();

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema);
        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName);
        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata();
        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName);
    }

    public class DbMetadata : IDbMetadata
    {
        public IDictionary<string, SchemaMetadata> Schemas { get; set; } = new Dictionary<string, SchemaMetadata>();
        public IDictionary<string, TableMetadata> Tables { get; set; } = new Dictionary<string, TableMetadata>();
        public IDictionary<string, ColumnMetadata> Columns { get; set; } = new Dictionary<string, ColumnMetadata>();
        public IEnumerable<KeyMetadata> Keys { get; set; } = new List<KeyMetadata>();
        public IDictionary<string, ProcedureMetadata> Procedures { get; set; } = new Dictionary<string, ProcedureMetadata>();
        public IDictionary<string, ParameterMetadata> Parameters { get; set; } = new Dictionary<string, ParameterMetadata>();

        public IEnumerable<string> GetTableNames() => Tables.Keys;
        public IDictionary<string, TableMetadata> GetTablesMetadata() => Tables;

        public IDictionary<string, ColumnMetadata> GetColumnsMetadata(string TableSchema, string TableName) => Columns;

        public IEnumerable<KeyMetadata> GetKeysMetadata() => Keys;
        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata(string Schema) => Procedures;
        public ProcedureMetadata GetProcedureMetadata(string SchemaName, string ProcedureName) => Procedures[ProcedureName];

        public IDictionary<string, ParameterMetadata> GetParametersMetadata(string SchemaName, string ProcedureName) => Parameters;

        public IEnumerable<string> GetSchemasNames() => Schemas.Keys.ToList();
        public IDictionary<string, SchemaMetadata> GetSchemas()
        {

            return Schemas;
        }

        public IDictionary<string, ProcedureMetadata> GetProceduresMetadata() => Procedures;

        public IDictionary<string, IDictionary<string, ParameterMetadata>> GetProceduresParametersMetadata()
        {
            Dictionary<string, IDictionary<string, ParameterMetadata>> p = new();
            foreach (var kv in Parameters)
            {
                if (!p.ContainsKey(kv.Key))
                {
                    p[kv.Value.SpecificSchema] = new Dictionary<string, ParameterMetadata>();
                }
                p[kv.Value.SpecificSchema][kv.Value.ParameterName] = kv.Value;
            }
            return p;
        }

        public List<string> DatabaseNames = new();
        public IEnumerable<string> GetDatabaseNames() => DatabaseNames;

        public List<string> GetProcedureSql(string schema, string name)
        {
            return this.Commit<List<string>>(() =>
            {
                return Procedures[$"[{schema}].[{name}]"].Sql.Split("\n").ToList();
            }, new()
            {
                { "schema", schema }, { "name", name }
            });
        }
    }
}

