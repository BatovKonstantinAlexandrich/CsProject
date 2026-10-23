
using System;
using System.Collections.Generic;
 

[Label("Источник данных")]
public class BusinessDatasource: DictionaryTable 
{
          
    public BusinessDatasource()
    {
        this.Datasets = new List<BusinessDataset>();
    }

    /*[InputOrder(3)]
    [InputSelect("ADO,ODBC,OLEDB")]
    public string ConnectionType { get; set; } = "ADO";*/

    [InputOrder(3)]
    [InputSelect("MSSQL,Postgres,MySQL")]
    public string ProviderDatabase { get; set; } = "MSSQL";


    [InputOrder(3)]
    [Label("Строка подключения")]
    [NotNullNotEmpty("Необходимо задать строку подключения")]
    public string ConnectionString { get; set; }


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [InputOrder(4)]
    [Label("Наборы данных")]
    public virtual List<BusinessDataset> Datasets { get; set; }


    /*public DatabaseManager GetOdbcDatabaseManager()
    {
        return DatabaseManager.GetOdbcDatabaseManager(ConnectionString);        
    }*/


    /*public override Dictionary<string, List<string>> ValidateOptional()
    {
        var res = new Dictionary<string, List<string>>();
        try
        {
            OdbcDataSource ds = new OdbcDataSource(ConnectionString);
            var tables = ds.GetTables();

            int ctn = tables.Count;
        }
        catch (Exception ex)
        {
            res["ConnectionString"] = new List<string>() { "Ошибка при подключении к источнику " + ex.Message };
        }
        return res;
    }*/


 
} 