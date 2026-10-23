
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EntityFasadeBase: IEntityFasade, IEntitySearch
{ 
    private IWebApi _api;
    private Type _entity;
    private TableMetadata _metadata;
    private ISqlExecutor _exec;
    public EntityFasadeBase(IWebApi api, ISqlExecutor exec, TableMetadata metadata, Type entity )
    {
        _api = api;
        _exec = exec;
        _entity = entity;
        _metadata = metadata;
    }

    public override string ToString()
    {
        return _metadata.TableName.ToLower();
    }

    public int Count( ) 
    {
        return _exec.GetSingleJObject($"SELECT COUNT(*) AS CNT FROM [{_metadata.TableSchema}].[{_metadata.TableName}]")["CNT"].ToString().ToInt();
    }

    public async Task<TResultSet> Find<TResultSet>(int id) where TResultSet : class
    {
        
        return _exec.ExecuteQuery<TResultSet>($"SELECT * FROM [{_metadata.TableSchema}].[{_metadata.TableName}] WHERE {_metadata.PrimaryKey}={id}").FirstOrDefault();
         
    }

    public async Task<int> Delete(int id)
    {
        
        return _exec.PrepareQuery($"DELETE FROM [{_metadata.TableSchema}].[{_metadata.TableName}] WHERE {_metadata.PrimaryKey}={id}");
        
    }

    public async Task<int> Update(object model)
    {
        await Task.CompletedTask ;
        string Settings = $"";
            
        foreach (string key in _metadata.ColumnsMetadata.Keys)
        {
            if (key.ToLower()!="id")
            {
                string Constant = GetValueContant(GetValue(model, key), _metadata.ColumnsMetadata[key]);
                Settings += $"\t[{key}]={Constant},\n";
            }
        }
        if( Settings.EndsWith(",\n"))
        {
            Settings = Settings.Substring(0, Settings.Length - ",\n".Length) + "\n";
        }
        return _exec.PrepareQuery($"UPDATE [{_metadata.TableSchema}].[{_metadata.TableName}]\n SET \n{Settings} WHERE {_metadata.PrimaryKey}={GetValue(model,_metadata.PrimaryKey)}");
      
    }

    private object GetValue(object model, string key)
    {
        
        return ReflectionService.GetValueFor(model, key);
    }

    public async Task<int> Create(object model)
    {
        string Values = $"(";
        string Columns = $"(";
        foreach (string key in _metadata.ColumnsMetadata.Keys)
        {
            var f = IsUserInput(_metadata.ColumnsMetadata[key]);
            if (f)
            {
                var ckey = key.ToCapitalStyle();
                var val = model.GetValue(ckey);
                string Constant = GetValueContant(val, _metadata.ColumnsMetadata[key]);
                Columns += $"[{key}],";
                Values += $"{Constant},";
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
        var result = _exec.ExecuteQuery<BaseEntity>($"INSERT INTO [{_metadata.TableSchema}].[{_metadata.TableName}] {Columns} VALUES {Values}; select SCOPE_IDENTITY() as [Id]");
        if(model is BaseEntity bs)
        {
            bs.Id = result.FirstOrDefault().Id;
        }

        return result.Count();
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
        switch(metadata.DataType.ToLower())
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
                return value!=null?"'" + value   + "'": "";
            case "bit":
                return value is true ? "1" : "0";
            case "smallint":
            
            case "int":
            case "float":
                return "" + value.ToString().Replace(",", ".") + "";
            default: throw new NotImplementedException("Тип данных "+ metadata.DataType + " не поддерживается ");  
        }
    }

    private string ToDateString(object value)
    {
        string text = "'"+((DateTime)value).ToString("yyyy-MM-dd")+"'";
        return text;
    }

    public async Task<object[]> GetAll()
    {
         
        return _exec.ExecuteQuery($"SELECT * FROM [{_metadata.TableSchema}].[{_metadata.TableName}]",_entity).ToArray();
        
    }

    public Task<object[]> Page(int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<object[]> Page(int page, int size, params string[] sorting)
    {
        throw new NotImplementedException();
    }

    public string GetEntityName()
    {
        return _metadata.TableName;
    }

    public async Task<object> CreateNew()
    {
       
        var record = _entity.New();
        await Create(record);
        return record;
        
    }

    public IEntityFasade<T> ToFasade<T>() where T : BaseEntity
    {
        return new EntityFasade<T>(this);
    }

    internal IEnumerable<Dictionary<string,object>> GetPage(int v, int size)
    {


        var dt = _exec.ExecuteQuery($"SELECT * FROM [{_metadata.TableSchema}].[{_metadata.TableName}]");
        
        return _exec.GetResultSetDictionary(dt);

    }

    internal List<string> GetOptions(string v)
    {
        return new List<string>();
    }

    public Task<Dictionary<string, string>> Lastest(int size)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> GetMostPopulare(int size)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> GetKeywords(string query)
    {
        throw new NotImplementedException();
    }

    public List<string> GetIndexes()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> Search(string query)
    {
        var results = new List<Dictionary<string, object>>();
        
        var q = _exec.ExecuteQuery($"SELECT * FROM [{_metadata.TableSchema}].[{_metadata.TableName}]");
        var dataset = _exec.GetResultSetDictionary(q);
        if (String.IsNullOrWhiteSpace(query))
            return dataset;
        foreach (var row in dataset)
        {
            var text = row.Values.ToJson();
            if(text.ToLower().IndexOf(query.ToLower()) != -1)
            {
                results.Add(row);
            }
        }
        return results;
    }

    public Task<Dictionary<string, object>[]> Search(string query, int page, int size)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, object>[]> Search(string query, int page, int size, params string[] sorting)
    {
        throw new NotImplementedException();
    }
}