using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

using Newtonsoft.Json.Linq;

using System;

using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.Connectors
{

    /**
     * 
        //System.Data.Odbc   @"Driver={MySQL ODBC 5.3 ANSI Driver};DATA SOURCE=mysql_app;Uid=root;Pwd=root;"
        //System.Data.Odbc   @"Driver={Microsoft Access Driver (*.mdb)};Dbq=C:\mydatabase.mdb;Uid=Admin;Pwd=;"
        //System.Data.Odbc   @"DRIVER={SQL SERVER};SERVER=(LocalDB)\\v11.0;AttachDbFileName=G:\projects\eckumoc\AppData\persistance.mdf;"   "
        //System.Data.OleDb  @"Provider=Microsoft.Jet.OLEDB.12.0;Data Source=a:\\master.mdb;";
     */
    public class OdbcDataSource : MyValidatableObject, APIdataSource
    {
        public DatabaseMetadata Metadata { get; set; }
        public string ConnectionString { get; set; }
        public string Alias { get; set; }
        /// <summary>
        /// Вывод в консоль информации об исключении
        /// </summary>
        /// <param name="ex"></param>
        public void Log(Exception ex)
        {
            this.Info(ex.Message);

        }
        public OdbcDataSource()
        {
            SetSystemDatasource("DataContext", "", "");

        }

        /// <summary>
        /// Подключение через строку соединения
        /// </summary>
        /// <param name="connectionString"> строка соединения</param>
        public OdbcDataSource(string connectionString)
        {
            ConnectionString = connectionString;
            OnInit();
        }

        public void OnInit()
        {
            GetConnection().InfoMessage += (sender, args) =>
            {
                this.Info("From ODBC Driver: " + sender + " " + args);
            };
            GetConnection().StateChange += (sender, args) =>
            {
                this.Info("ODBC state changed: " + sender + " " + args);
            };
            this.Info("Connection state: " + GetConnection().State);
        }

        public ResultSet CleverExecute(string expression)
        {

            using (OdbcConnection connection = GetConnection())
            {

                connection.Open();
                DataTable dataTable = new DataTable();
                OdbcDataAdapter adapter = new OdbcDataAdapter(expression, connection);
                adapter.Fill(dataTable);

                TableInfo tmd = new TableInfo();
                foreach (DataColumn column in dataTable.Columns)
                {
                    ColumnMetaData cmd = new ColumnMetaData()
                    {
                        nullable = column.AllowDBNull,
                        unique = column.Unique,
                        description = column.Caption,
                        type = column.DataType.Name,
                    };
                    tmd.columns.Add(column.ColumnName, cmd);

                }

                var rs = convert(dataTable);
                return new ResultSet()
                {

                    MetaData = tmd,
                    DataTable = dataTable,
                    DataSet = rs
                };
            }
        }


        /// <summary>
        /// Подключение к источнику зарегистрированному в системе
        /// </summary>
        /// <param name="dns"> имя источника </param>
        /// <param name="login"> логин </param>
        /// <param name="password"> пароль </param>
        public OdbcDataSource(string dns, string login, string password)
        {
            SetSystemDatasource(dns, login, password);
        }

        public void SetSystemDatasource(string dns, string login, string password)
        {
            ConnectionString = "dsn=" + dns + ";UId=" + login + ";PWD=" + password + ";";
        }


        /// <summary>
        /// Установка соединения 
        /// </summary>
        public virtual OdbcConnection GetConnection()
        {

            OdbcConnection connection = null;
            try
            {
                this.Info(ConnectionString);
                connection = new OdbcConnection(ConnectionString);
            }
            catch (Exception ex)
            {
                Error("При попытки установить соединение ODBC: " + ConnectionString + " возникла неожиданныя ситуация", ex);
            }
            return connection;
        }

        public void Error(string message, Exception ex)
        {
            this.Info(message);
        }


        /// <summary>
        /// Считывание бинарных данных, получаемых запросом
        /// </summary>
        public byte[] ReadBlob(string sqlCommand)
        {
            using (OdbcConnection connection = GetConnection())
            {
                connection.ChangeDatabase("FRMO");
                connection.Open();
                OdbcCommand command = new OdbcCommand(sqlCommand, connection);
                OdbcDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // matching record found, read first column as string instance
                    byte[] value = (byte[])reader.GetValue(0);
                    reader.Close();
                    command.ExecuteNonQuery();
                    return value;
                }
                return null;
            }
        }


        /// <summary>
        /// Запись бинарных данных в базу
        /// </summary>
        public int InsertBlob(string sqlCommand, string blobColumn, byte[] data)
        {
            using (OdbcConnection connection = GetConnection())
            {
                connection.Open();
                OdbcCommand command = new OdbcCommand(sqlCommand, connection);
                command.Parameters.Add(blobColumn, OdbcType.Binary);
                command.Parameters[blobColumn].Value = data;
                return command.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Получение расширенной справочной информации
        /// </summary>
        public Dictionary<string, object> GetSchemaDictionary()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            using (OdbcConnection connection = GetConnection())
            {
                connection.Open();
                DataTable catalogs = connection.GetSchema();
                JArray jcatalogs = convert(catalogs);
                foreach (JObject catalogInfo in jcatalogs)
                {
                    string collectionName = catalogInfo["CollectionName"].Value<string>();
                    if (collectionName == "Indexes")
                    {
                        Dictionary<string, object> indexes = new Dictionary<string, object>();
                        foreach (string table in GetTables())
                        {
                            JArray catalog = convert(connection.GetSchema(collectionName, new string[] { null, null, table }));
                            indexes[table] = catalog;
                        }
                        result[collectionName] = indexes;
                    }
                    else
                    {
                        if (collectionName != "DataTypes")
                        {
                            JArray catalog = convert(connection.GetSchema(collectionName));
                            result[collectionName] = catalog;
                        }
                    }
                }
                result["catalogs"] = jcatalogs;
            }
            return result;
        }


        /// <summary>
        /// Вспомогательный метод преобразования данных в JSON
        /// </summary>
        public JArray convert(DataTable dataTable)
        {
            Dictionary<string, object> resultSet = new Dictionary<string, object>();
            List<Dictionary<string, object>> listRow = new List<Dictionary<string, object>>();
            foreach (DataRow row in dataTable.Rows)
            {
                Dictionary<string, object> rowSet = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowSet[column.Caption] = row[column.Caption];
                }
                listRow.Add(rowSet);
            }
            resultSet["rows"] = listRow;

            JObject jrs = JObject.FromObject(resultSet);
            return (JArray)jrs["rows"];
        }


        /// <summary>
        /// Получение списка таблиц
        /// </summary>
        public IEnumerable<string> GetTables()

        {
            List<string> tableNames = new List<string>();
            using (OdbcConnection connection = GetConnection())
            {
                connection.Open();
                DataTable tables = connection.GetSchema("Tables");
                foreach (JObject next in convert(tables))
                {
                    this.Info(next);
                    if (next["TABLE_SCHEM"].Value<string>() != "sys")
                    {
                        string tableName = next["TABLE_NAME"].Value<string>();
                        if (tableName.StartsWith("__") == false)
                        {
                            tableNames.Add(tableName);
                        }
                    }

                }
            }
            return tableNames.ToArray();
        }


        /// <summary>
        /// Запрос параметров хранимых процедур
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, ProcedureInfo> GetStoredProceduresMetadata()
        {
            Dictionary<string, ProcedureInfo> metadata = new Dictionary<string, ProcedureInfo>();
            // TODO:
            return metadata;
        }






        public virtual DatabaseMetadata GetDatabaseMetadata()
        {


            Metadata = new DatabaseMetadata();
            //metadata.Metadata = this.GetSchemaDictionary();
            using (OdbcConnection connection = GetConnection())
            {
                connection.Open();

                Metadata.driver = connection.Driver;
                Metadata.database = connection.Database;
                object site = connection.Site;

                //metadata.serverVersion = connection.ServerVersion;
                Metadata.connectionString = connection.ConnectionString;

                DataTable columns = connection.GetSchema("Columns");
                foreach (DataRow row in columns.Rows)
                {
                    string table = row["TABLE_NAME"].ToString();
                    string column = row["COLUMN_NAME"].ToString();
                    string type = row["TYPE_NAME"] == null ? null : row["TYPE_NAME"].ToString();
                    string catalog = row["TABLE_CAT"] == null ? null : row["TABLE_CAT"].ToString();
                    string schema = row["TABLE_SCHEM"] == null ? null : row["TABLE_SCHEM"].ToString();
                    string description = row["COLUMN_DEF"] == null ? null : row["COLUMN_DEF"].ToString();
                    string nullable = row["NULLABLE"] == null ? null : row["NULLABLE"].ToString();

                    //исколючаем системные таблицы и служебные
                    if (schema == "sys" || schema == "INFORMATION_SCHEMA" || table.ToLower().IndexOf("migration") != -1)
                    {
                        continue;
                    }



                    if (!Metadata.Tables.ContainsKey(table))
                    {
                        Metadata.Tables[table] = new TableInfo();
                        Metadata.Tables[table].name = table;
                        Metadata.Tables[table].description = "";

                        //определение наименования в множественном числе и единственном                        
                        string tableName = table;
                        if (tableName.EndsWith("s"))
                        {
                            if (tableName.EndsWith("ies"))
                            {
                                Metadata.Tables[table].multicount_name = tableName;
                                Metadata.Tables[table].singlecount_name = tableName.Substring(0, tableName.Length - 3) + "y";
                            }
                            else
                            {
                                Metadata.Tables[table].multicount_name = tableName;
                                Metadata.Tables[table].singlecount_name = tableName.Substring(0, tableName.Length - 1);
                            }
                        }
                        else
                        {
                            if (tableName.EndsWith("y"))
                            {
                                Metadata.Tables[table].multicount_name = tableName.Substring(0, tableName.Length - 1) + "ies";
                                Metadata.Tables[table].singlecount_name = tableName;

                            }
                            else
                            {
                                Metadata.Tables[table].multicount_name = tableName + "s";
                                Metadata.Tables[table].singlecount_name = tableName;
                            }
                        }
                    }
                    Metadata.Tables[table].columns[column] = new ColumnMetaData();
                    Metadata.Tables[table].columns[column].name = column;
                    Metadata.Tables[table].columns[column].type = type;
                    Metadata.Tables[table].columns[column].nullable = nullable == "1" ? true : false;
                    Metadata.Tables[table].columns[column].description = description;
                }


                //определение внешних ключей по правилам наименования
                List<TableInfo> tables = (from table in Metadata.Tables.Values select table).ToList();
                foreach (var ptable in Metadata.Tables)
                {

                    HashSet<string> associations = new HashSet<string>() { ptable.Value.multicount_name, ptable.Value.singlecount_name };
                    foreach (var pcolumn in ptable.Value.columns)
                    {
                        //дополнительный анализ наименований колоной
                        string[] ids = pcolumn.Key.ToLower().Split("_");
                        HashSet<string> idsSet = new HashSet<string>(ids);
                        List<string> lids = (from id in idsSet select id.ToLower()).ToList();
                        if (idsSet.Contains("id"))
                        {
                            int count = (from s in idsSet where associations.Contains(s) select s).Count();
                            if (count == 0)
                            {
                                TableInfo foreignKeyTable = (from table in tables where lids.Contains(table.singlecount_name) || lids.Contains(table.multicount_name) select table).SingleOrDefault();
                                if (foreignKeyTable == null)
                                {
                                    //throw new Exception("внешний ключ не найден для поля "+ ptable.Key+"."+pcolumn.Key );
                                }
                                else
                                {

                                    ptable.Value.fk[pcolumn.Key] = foreignKeyTable.singlecount_name;
                                }
                            }
                            else
                            {
                                pcolumn.Value.primary = true;
                                ptable.Value.pk = Metadata.Tables[ptable.Key].pk = pcolumn.Key;

                            }
                        }
                    }
                }
                return Metadata;
            }
        }


        /// <summary>
        /// Выполнение запроса, возвращающего одну запись.
        /// </summary>
        public JObject GetSingleJObject(string sql)
        {

            using (OdbcConnection connection = GetConnection())
            {
                connection.Open();
                DataTable dataTable = new DataTable();
                OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                adapter.Fill(dataTable);
                JArray rs = convert(dataTable);
                foreach (JObject next in rs)
                {
                    return next;
                }
                throw new Exception("Запрос не вернул данные " + sql);
            }
        }



        JArray APIdataSource.GetJsonResult(string sql)
        {
            return Execute(sql);
        }

        public DataTable CreateDataTable(string sql)
        {
            this.Info(sql);
            using (OdbcConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();
                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                    adapter.Fill(dataTable);
                    return dataTable;
                }
                catch (Exception ex)
                {
                    ex.ToString().WriteToConsole();
                    throw;
                }
            }
        }

        /// <summary>
        /// Выполнение запроса 
        /// </summary>
        public DataTable ExecuteDT(string sql)

        {
            this.Info(sql);
            using (OdbcConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();
                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                    adapter.Fill(dataTable);

                    return dataTable;
                }
                catch (Exception ex)
                {
                    Log(ex);
                    throw;
                }

            }
        }
        /// <summary>
        /// Выполнение запроса 
        /// </summary>
        public JArray Execute(string sql)

        {
            this.Info(sql);
            using (OdbcConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = new DataTable();
                    OdbcDataAdapter adapter = new OdbcDataAdapter(sql, connection);
                    adapter.Fill(dataTable);

                    TableInfo tmd = new TableInfo();
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        ColumnMetaData cmd = new ColumnMetaData()
                        {
                            nullable = column.AllowDBNull,
                            unique = column.Unique,
                            description = column.Caption,
                            type = column.DataType.Name,
                        };
                        tmd.columns.Add(column.ColumnName, cmd);
                    }
                    var array = convert(dataTable);
                    this.Info(array);
                    return array;
                }
                catch (Exception ex)
                {
                    this.Info("Ошибка при выполнении запроса: " + sql + " " + ex.Message);
                    throw;
                }

            }
        }


        public string GetConenctionString()
        {
            return ConnectionString + "Driver={SQL Server};";
        }

        public bool canConnect()
        {
            return GetTables() != null;
        }

        public bool canReadAndWrite()
        {
            //TODO:
            return true;
        }

        public bool canCreateAlterTables()
        {
            //TODO:
            return true;
        }

        public object SingleSelect(string sql)
        {
            throw new NotImplementedException();
        }

        public object MultiSelect(string sql)
        {
            throw new NotImplementedException();
        }

        public object Exec(string sql)
        {
            throw new NotImplementedException();
        }

        public object Prepare(string sql)
        {
            throw new NotImplementedException();
        }

        void APIdataSource.InsertBlob(string sql, string v, byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}
