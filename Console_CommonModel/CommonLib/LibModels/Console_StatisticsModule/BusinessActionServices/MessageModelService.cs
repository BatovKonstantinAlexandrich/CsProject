using Namotion.Reflection;

using System.Reflection;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using static System.Runtime.InteropServices.JavaScript.JSType;

/// <summary>
/// Предоставляет функции для создания концептуальной схемы данных,
/// на основе модели потоков сообщений.
/// </summary>
/// <param name="db"></param>
public class MessageModelService(BusinessDbFactory db) : IMessageModelService
{


    /// <summary>
    /// Проверка целостности модели
    /// </summary>    
    public Dictionary<string, Dictionary<int, Dictionary<string, List<string>>>> ValidateModel()
    {
        Dictionary<string, Dictionary<int, Dictionary<string, List<string>>>> results = new();
        foreach(var function in db.BusinessFunctions)
        {
            var functionValidationResults = function.Validate();
            if(functionValidationResults.Any(kv => kv.Value.Count() > 0))
            {
                if(!results.ContainsKey(nameof(db.BusinessFunctions)))
                {
                    results[nameof(db.BusinessFunctions)] = new();
                }
                results[nameof(db.BusinessFunctions)][function.Id] = functionValidationResults;
            }
        }
        foreach (var function in db.MessageProtocols)
        {
            var functionValidationResults = function.Validate();
            if (functionValidationResults.Any(kv => kv.Value.Count() > 0))
            {
                if (!results.ContainsKey(nameof(db.MessageProtocols)))
                {
                    results[nameof(db.MessageProtocols)] = new();
                }
                results[nameof(db.MessageProtocols)][function.Id] = functionValidationResults;
            }
        }
        foreach (var function in db.BusinessResources)
        {
            var functionValidationResults = function.Validate();
            if (functionValidationResults.Any(kv => kv.Value.Count() > 0))
            {
                if (!results.ContainsKey(nameof(db.BusinessResources)))
                {
                    results[nameof(db.BusinessResources)] = new();
                }
                results[nameof(db.BusinessResources)][function.Id] = functionValidationResults;
            }
        }
        return results;
    }



 
    public void CreateBusinessIndicator(BusinessFunction function)
    {
        function.EnsureIsValide();
        
        var ddl = new SqlServerDDLFactory();        
        var resource = db.BusinessResources.Find(function.BusinessResourceId);
        var name = this.GetTableNameFor(resource);
        var columns = db.GetColumnsMetadata($"{nameof(BusinessResource)}", $"{resource.Name}");
        var columnName = GetIndicatorNameFor(function);
        if(columns.Keys.Select(key => key.ToLower()).Any(p => p == columnName.ToLower()))
        {
            var drop_sql = ddl.DropColumnFromTable(name, columnName);
            db.PrepareQuery(drop_sql);
        }
        var sql = ddl.AddColumnToTable(name, columnName, "int", true);
        db.PrepareQuery(sql);

    }

    public void DropBusinessIndicator(BusinessFunction function)
    {
        var ddl = new SqlServerDDLFactory();
        var resource = db.BusinessResources.Find(function.BusinessResourceId);
        var name = this.GetTableNameFor(resource);
        var columnName = GetIndicatorNameFor(function);
        var sql = ddl.DropColumnFromTable(name, columnName);
        db.PrepareQuery(sql);
    }


    /// <summary>
    /// Получение имени таблицы для протокола
    /// </summary>
    public string GetTableNameFor(MessageProtocol protocol)
    {
        return $"[{nameof(MessageProtocol)}].[{protocol.Name}]";
    }


    /// <summary>
    /// Получение имени процедуры для передачи по протоколу
    /// </summary>
    public string GetProcedureNameFor(BusinessFunction function)
    {       
        var resource = db.BusinessResources.Find(function.BusinessResourceId);
        return $"[{resource.Name}].[{function.Name}]";
    }

    /// <summary>
    /// Получение имени процедуры для передачи по протоколу
    /// </summary>
    public string GetIndicatorNameFor(BusinessFunction function)
    {
        var resource = db.BusinessResources.Find(function.BusinessResourceId);
        return $"{function.Name}";
    }


    /// <summary>
    /// Получение имени процедуры для передачи по протоколу
    /// </summary>
    public string GetProcedureNameFor(MessageProtocol protocol)
    {
        var function = db.BusinessFunctions.Find(protocol.FromBusinessFunctionId);
        if (function is null)
            return null;
        var resource = db.BusinessResources.Find(function.BusinessResourceId);
        return $"[{resource.Name}].[{protocol.Name}]";
    }


    /// <summary>
    /// Регистрация протокола обмена сообщениями
    /// </summary>
    public int AddMessageProtocol(MessageProtocol protocol)
    {
        int ctn = 0;
        var props = protocol.Properties;
        protocol.Properties = new List<MessageProperty>();
        db.Add(protocol);
        ctn += db.SaveChanges();
        foreach (var property in props)
        {
            property.MessageProtocol = null;
            property.MessageProtocolId = protocol.Id;
            db.Add(protocol);
            ctn += db.SaveChanges();
        }
        return ctn;
    }

    public string GetBusinessResourceTableNameFor(BusinessFunction function)
    {
        var resource = db.BusinessResources.First(r => r.Id == function.BusinessResourceId);
        if (resource is null)
            throw new Exception("BusinessResourceId property is null");
        return GetTableNameFor(resource);
    }


    public void CreateStoredProcedure(BusinessFunction function)
    {
        


        var api = db;
        var role = db.BusinessResources.Find(function.BusinessResourceId);
        var schema = role.Name;
        var name = function.Name;
        var bp = new BusinessDatabaseGenerator();
        MessageProtocol pin = null, pout = null;
        var generator = new BusinessDatabaseGenerator();
        
        if (function.OutputId is not null)
        {
            pout = db.MessageProtocols.Find(function.OutputId);
            pout.Properties = db.MessageProperties.Where(p => p.MessageProtocolId == function.OutputId).ToList();
            var dml = generator.CreateTableModel(pout);
            if (!api.GetSchemasNames().Any(name => name.ToLower() == dml.schema.ToLower()))
            {
                var bg = new SqlServerDDLFactory();
                api.PrepareQuery(bg.CreateSchema(dml.schema));
            }
            var ddl = new SqlServerDDLFactory();
            api.PrepareQuery(ddl.DropTable(dml));
            var sql = api.CreateTable(dml);
            api.PrepareQuery(sql);
        }
        var signin = AppProviderService.GetInstance().Get<SigninUser<DeliveryDbContext>>();
        var resource = db.BusinessResources.First(r => r.Id == function.BusinessResourceId);
        if (resource is null)
            throw new Exception("BusinessResourceId property is null");        
        var builder = new ProcedureBuilder();
        builder.AddInputParameter("@" + resource.Name + "Id", "int");
        if (pin is not null)
            builder.AddInputParameter($"@{pin.Name}Id", "int");
        if (pout is not null)
        {
            
            if (pout is not null)
            {
                var tableMeta = bp.CreateDataModel(pout);
                if(tableMeta.ColumnsMetadata.Count() > 1)
                {

                    var tableModel = bp.CreateTableModel(pout);
                     
                    foreach (var prop in tableModel.columns.Values)
                    {
                        if (prop.name == "[ID]")
                            continue;
                        var pname = "@" + prop.name;
                        var ptype = prop.type;
                        builder.AddInputParameter(pname, ptype);
                    }
                    builder.AddInsertToTable(tableMeta);
                }


            }
            var businessResourceTableName = this.GetBusinessResourceTableNameFor(function);
            var indicatorName = this.GetIndicatorNameFor(function);
            builder.AddIncrementCounter("@" + resource.Name + "Id", businessResourceTableName, indicatorName);
        }

        if (!api.GetSchemasNames().Any(name => name.ToLower() == schema.ToLower()))
        {
            var bg = new SqlServerDDLFactory();
            api.PrepareQuery(bg.CreateSchema(schema));
        }

        if (db.HasProcedure(schema, name))
        {
            api.PrepareQuery($"DROP PROCEDURE {$"[{schema}].[{name}]".ToLower().Replace("[[", "[").Replace("]]", "]")}");
        }
        var createProcedure = builder.Build(schema, name);
        this.Info(createProcedure);
        api.PrepareQuery(createProcedure);
    }

    public string GetTableNameFor(BusinessResource resource)
    {
        return $"[{nameof(BusinessResource)}].[{resource.Name}]";
    }

    public void DropStoredProcedure([NotNullNotEmpty]BusinessFunction function)
    {
        MethodBase.GetCurrentMethod().EnsureIsValid(new object[] { function });
        var api = db;
        var role = db.BusinessResources.Find(function.BusinessResourceId);
        var schema = role.Name;
        var name = function.Name;
        var generator = new BusinessDatabaseGenerator();

        api.PrepareQuery(generator.DropProcedure($"[{schema}].[{name}]"));

    }

    public MessageProtocol CreateMessageProtocol<TModel>() where TModel : class
    {
        return ((IMessageModelService)db).CreateMessageProtocol<TModel>();
    }

    public BusinessFunction CreateBusinessFunction<TBusinessResource, TInputMessageProtocol>([NotNullNotEmpty] BusinessLogic logic)
    {
        return ((IMessageModelService)db).CreateBusinessFunction<TBusinessResource, TInputMessageProtocol>(logic);
    }
}
