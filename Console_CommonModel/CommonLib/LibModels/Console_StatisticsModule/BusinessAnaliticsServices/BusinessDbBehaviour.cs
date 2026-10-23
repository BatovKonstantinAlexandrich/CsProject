using Microsoft.EntityFrameworkCore;

using System.Reflection;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.Others;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

/// <summary>
/// Поведение бизнес модели.
/// </summary>
[Label("Поведение бизнес-модели")]
public class BusinessDbBehaviour: BusinessDbFactory
{
    public BusinessDbBehaviour(): base()
    {
    }

    public BusinessDbBehaviour(DbContextOptions<DbContextBase> options) : base(options)
    {


    }

    /// <summary>
    /// Обработка событий изменения данных MessageProperty
    /// Смотрит на протокол к которому привязано свойство, получает наименование таблицы и добавляет или удаляет колонку.    
    /// Обновляет хранимую процедуру для записи.
    /// </summary>
    [OnDataChanged<MessageProperty>()]
    public void OnMessagePropertyChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        try
        {
            var before = new MessageProperty();
            foreach (var item in primary)
                before.SetValue(item.Key, item.Value);
            var after = new MessageProperty();
            foreach (var item in data)
                after.SetValue(item.Key, item.Value);
            if (after.Name.ToLower() == "id")
                return;
            //property.EnsureIsValide();

            var api = new SqlServerWebApi();
            var s = this.Database.GetConnectionString();
            api.Configure(s);
            switch (state)
            {
                case EntityState.Modified:
                    {
                        var protocol = this.MessageProtocols.Find(before.MessageProtocolId);
                        var tableName = this.GetTableNameFor(protocol);
                        var propertyName = before.Name;

                        var ddl = new SqlServerDDLFactory();
                        var attribute = this.MessageAttributes.Find(before.AttributeId);
                        var sql = ddl.DropColumnFromTable(tableName, propertyName);
                        api.PrepareQuery(sql);

                        protocol = this.MessageProtocols.Find(after.MessageProtocolId);
                        tableName = this.GetTableNameFor(protocol);
                        propertyName = after.Name;

                        attribute = this.MessageAttributes.Find(after.AttributeId);
                        sql = ddl.AddColumnToTable(tableName, propertyName, attribute.SqlServerDataType, !after.Required);
                        api.PrepareQuery(sql);

                        if (before.MessageProtocolId is not null)
                        {
                            var p = this.MessageProtocols.Find(before.MessageProtocolId);
                            if (p is not null)
                            {
                                var f = this.BusinessFunctions.Find(p.FromBusinessFunctionId);
                                if (f is not null)
                                {
                                    this.DropStoredProcedure(f);
                                }
                            }
                        }
                        this.CreateStoredProcedure(this.BusinessFunctions.Find(this.MessageProtocols.Find(after.MessageProtocolId).FromBusinessFunctionId));
                        break;
                    }
                case EntityState.Deleted:
                    {
                        var protocol = this.MessageProtocols.Find(after.MessageProtocolId);
                        var tableName = this.GetTableNameFor(protocol);
                        var propertyName = after.Name;

                        var ddl = new SqlServerDDLFactory();
                        var attribute = this.MessageAttributes.Find(after.AttributeId);
                        var sql = ddl.DropColumnFromTable(tableName, propertyName);
                        api.PrepareQuery(sql);

                        var function = this.BusinessFunctions.Find(protocol.FromBusinessFunctionId);
                        if (function is not null)
                        {
                            this.DropStoredProcedure(function);
                            this.CreateStoredProcedure(function);
                        }
                        break;
                    }
                case EntityState.Added:
                    {
                        if (after.MessageProtocolId is null)
                        {
                            api.Warn("Не задан протокол сообщений");
                            break;
                        }
                        var protocol = this.MessageProtocols.FirstOrDefault(p => p.Id == after.MessageProtocolId);
                        if (protocol is null)
                            break;
                        var tableName = this.GetTableNameFor(protocol);
                        var propertyName = after.Name;

                        var ddl = new SqlServerDDLFactory();
                        if (after.AttributeId is null)
                            break;
                        var attribute = this.MessageAttributes.Find(after.AttributeId);
                        var sql = ddl.AddColumnToTable(tableName, propertyName, attribute.SqlServerDataType, !after.Required);
                        api.PrepareQuery(sql);
                        if (protocol.FromBusinessFunctionId is null)
                            break;
                        var function = this.BusinessFunctions.Find(protocol.FromBusinessFunctionId);
                        if (function is not null)
                        {
                            this.DropStoredProcedure(function);
                            this.CreateStoredProcedure(function);
                        }
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            InputConsole.Clear();
            this.Error($"Исключение при изменении данных MessageProtocol: {ex.Message}");
        }
    }


    [OnDataChanged<BusinessFunction>()]
    public void OnBusinessFunctionChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        try
        {
            InputConsole.Clear();
            
            this.Debug($"OnBusinessFunctionChanged({state}, {primary.ToJson()}, {data.ToJson()})");
            var functionBefore = new BusinessFunction();
            foreach (var item in primary)
            {
                functionBefore.SetValue(item.Key, item.Value);
            }

            var function = new BusinessFunction();
            foreach (var item in data)
            {
                function.SetValue(item.Key, item.Value);
            }
            if(!function.IsValide())
            {
                this.Info(function.ToJsonOnScreen());
                this.Info(function.Validate().ToJsonOnScreen());
            }
            function.EnsureIsValide();


            var api = new SqlServerWebApi();
            var s = this.Database.GetConnectionString();
            api.Configure(s);
            switch (state)
            {
                case EntityState.Modified:
                    {
                        this.DropBusinessIndicator(functionBefore);
                        this.CreateBusinessIndicator(function);
                        this.DropStoredProcedure(functionBefore);
                        this.CreateStoredProcedure(function);
                        break;
                    }
                case EntityState.Deleted:
                    {
                        this.DropBusinessIndicator(function);
                        this.DropStoredProcedure(function);
                        break;
                    }
                case EntityState.Added:
                    {
                        this.CreateBusinessIndicator(function);
                        this.CreateStoredProcedure(function);
                        break;
                    }

            }
        }
        catch (Exception ex)
        {
            this.Error($"Исключение при изменении данных BusinessFunction: {ex.Message}");
        }
    }



    public class CountResultSet
    {
        public int count { get; set; }
    }



    /// <summary>
    /// Событие изменения протокола обмена сообщениями
    /// </summary>
    /// <param name="state"></param>
    /// <param name="primary"></param>
    /// <param name="data"></param>
    [OnDataChanged<MessageProtocol>()]
    public void OnMessageProtocolChanged(EntityState state, Dictionary<string, object> primary, Dictionary<string, object> data)
    {
        try
        {
            var protocol = new MessageProtocol();
            foreach (var item in data)
                protocol.SetValue(item.Key, item.Value);

            var api = new SqlServerWebApi();
            var s = this.Database.GetConnectionString();
            api.Configure(s);
            switch (state)
            {
                case EntityState.Modified:
                    break;
                case EntityState.Deleted:
                    {
                        
                        var generator = new BusinessDatabaseGenerator();
                        var dml = generator.CreateTableModel(protocol);
                        if (protocol.FromBusinessFunctionId is null)
                        {
                            return;
                        }
                        var function = this.BusinessFunctions.Find(protocol.FromBusinessFunctionId);
                        if (function.BusinessResource is null)
                        {
                            return;
                        }
                        var resource = this.BusinessResources.Find(function.BusinessResourceId);

                        dml.schema = resource.Name;

                        var bg = new SqlServerDDLFactory();

                        api.PrepareQuery(bg.DropTable(dml));
                        break;
                    }
                case EntityState.Added:
                    {
                        var generator = new BusinessDatabaseGenerator();

                        var dml = generator.CreateTableModel(protocol);

                        BusinessFunction function = null;
                        BusinessResource resource = null;

                        dml.schema = nameof(MessageProtocol);
                        if (protocol.FromBusinessFunctionId is not null)
                        {
                            if (function.BusinessResource is not null)
                            {
                                resource = this.BusinessResources.Find(function.BusinessResourceId);
                            }
                        }
                        var bg = new SqlServerDDLFactory();
                        if (!api.GetSchemasNames().Any(name => name.ToLower() == dml.schema.ToLower()))
                        {
                            api.PrepareQuery(bg.CreateSchema(dml.schema));
                        }
                        var dropStatement = bg.DropTable(dml);
                        api.PrepareQuery(dropStatement);
                        var createTableStatement = bg.CreateTable(dml);
                        api.PrepareQuery(createTableStatement);
                    }
                    break;
            }

        }
        catch (Exception ex)
        {
            this.Error($"OnMessageProtocolChanged: {ex}");
            throw;
        }

    }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        try
        {
            //this.Info($"{GetType().GetTypeName()}.{MethodInfo.GetCurrentMethod().Name}() ... ");

            foreach (Type entityType in this.GetEntitiesTypes())
            {
                //this.Info($"{GetType().GetTypeName()}.{MethodInfo.GetCurrentMethod().Name}() ... {entityType.Name} ");
                var attributes = entityType.GetAttributes();
                foreach (var kv in attributes)
                {
                    if (kv.Key.ToType().IsExtends(typeof(EntityAttributeBase)))
                    {
                        var creating = kv.Key.ToType().New<EntityAttributeBase>(kv.Value);
                        creating.OnModelCreating(modelBuilder, entityType, this);
                    }
                }
                var propertyAttrs = entityType.GetPropertiesAttributesMap();
                foreach (var kv in propertyAttrs)
                {

                    if (entityType.GetProperties().FirstOrDefault(p => p.Name == kv.Key).IsExtends(typeof(EntityAttributeBase)))
                    {
                        var creating = kv.Key.ToType().New<EntityAttributeBase>(kv.Value);
                        creating.OnModelCreating(modelBuilder, entityType, this);
                    }
                }
            }
        }
        catch(Exception ex)
        {
            throw new Exception($"Исключение при выполнении {GetType().GetTypeName()} OnModelCreating() => {ex.Message}");
        }
    }
}