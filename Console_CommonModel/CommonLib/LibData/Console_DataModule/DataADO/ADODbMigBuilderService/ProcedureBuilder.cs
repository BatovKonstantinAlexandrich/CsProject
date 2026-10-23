using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Google.Protobuf.WellKnownTypes;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{
    internal class ProcedureBuilder
    {
        private readonly Dictionary<string, ColumnMetadata> input = new();
        private readonly List<Func<ProcedureBuilder, string>> todos = new();

        public ProcedureBuilder()
        {
            input = new();
        }

        private bool IsUserInput(ColumnMetadata columnMetadata)
        {
            return columnMetadata.ColumnName != "Id";
        }

        /// <summary>
        /// Используется для передачи данных в SQL
        /// </summary>
        private string GetValueContant(object value, ColumnMetadata metadata)
        {
            if (value == null)
                return "NULL";
            switch (metadata.DataType.ToLower())
            {
                case "date":
                case "datetime":

                    return ToDateString(value);


                //case "datetime2":
                //      break;
                case "ntext":
                case "text":

                case "nvarchar":
                case "nvarchar(max)":
                case "varchar":
                    return value != null ? "'" + value + "'" : "";
                case "bit":
                    return value is true ? "1" : "0";
                case "smallint":

                case "int":
                case "float":
                    return "" + value.ToString().Replace(",", ".") + "";
                default: throw new NotImplementedException("Тип данных " + metadata.DataType + " не поддерживается ");
            }
        }

        private string ToDateString(object value)
        {
            string text = "'" + ((DateTime)value).ToString("yyyy-MM-dd") + "'";
            return text;
        }


        /// <summary>
        /// Добавление скрипта для увеличения счяётчика
        /// </summary>
        public void AddIncrementCounter(string id, string fullTableName, string columnName)
        {
            var sql = $"UPDATE C SET {columnName}={columnName}+1\r\n FROM {fullTableName} c WHERE Id={id}";

            this.Info(sql);
            todos.Add((p) => sql);
        }

        public void AddInsertToTable(TableMetadata input)
        {
            string Values = $"(";
            string Columns = $"(";
            foreach (string key in input.ColumnsMetadata.Keys)
            {
                if (key == "ID" || key == "[ID]")
                    continue;
                var f = IsUserInput(input.ColumnsMetadata[key]);
                if (f)
                {
                    var ckey = key.ToCapitalStyle();
                    var val = $"@{ckey}";
                    string Constant = GetValueContant(val, input.ColumnsMetadata[key]);
                    Columns += $"[{key}],".Replace("[[", "[").Replace("]]", "]");
                    Values += $"{Constant},".Replace("[[", "").Replace("]]", "");
                    this.Info(Columns);
                    this.Info(Values);
                }
            }
            if (Columns.EndsWith(","))
            {
                Columns = Columns.Substring(0, Columns.Length - 1);
                Values = Values.Substring(0, Values.Length - 1);
            }
            Values += ")";
            Columns += ")";
            var sql = $"INSERT INTO [{input.TableSchema}].[{input.TableName}] {Columns} VALUES {Values}";

            this.Info(sql);
            todos.Add((p) => sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public string Build(string schema, string name)
        {
            string sql = $"CREATE PROCEDURE [{schema}].[{name}]";
            sql += "(";
            if (input is not null && input is not null)
                foreach (var column in input.Values)
                {
                    sql += $"{column.ColumnName} {column.DataType} {(column.IsNullable ? "NULL" : "")} {(column.IsAutoIncrement ? "IDENTITY(1,1)" : "")},";
                }

            if (sql.EndsWith(","))
                sql = sql.Substring(0, sql.Length - 1);
            sql += ") ";
            sql += "AS BEGIN\n";
            sql += $"  print '[{schema}].[{name}]' \n";
            foreach (var todo in todos)
            {
                sql += $"{todo(this)}\n";
            }
            sql += "END\n";
            return sql;
        }

        internal void AddInputParameter(string name, string type)
        {
            input[name] = new ColumnMetadata()
            {
                ColumnName = name.Replace("[", "").Replace("]", ""),
                DataType = type
            };
        }
    }
}