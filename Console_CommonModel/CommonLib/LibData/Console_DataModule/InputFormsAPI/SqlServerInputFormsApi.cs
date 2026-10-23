using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

[Label("Интерфейс для управления формами")]
public interface IInputFormsApi
{

    /// <summary>
    /// Получение протоколов на основе анализа структур данных таблиц
    /// </summary>    
    public List<MessageProtocol> GetTablesProtocols();


    /// <summary>
    /// Формирование протокола по метаданным таблицы
    /// </summary>
    public MessageProtocol GetMessageProtocol(TableMetadata tableMetadata);

    /// <summary>
    /// Формирование свойства протокола по метаданным колонки
    /// </summary>
    public MessageProperty GetMessageProperty(ColumnMetadata columnMetadata);


    /// <summary>
    /// Получение протоколов на основе анализа параметров процедур
    /// </summary>
    public List<MessageProtocol> GetProceduresProtocols();
}


[Label("Интерфейс для управления формами")]
public class SqlServerInputFormsApi : SqlServerWebApi, IInputFormsApi
{
    public SqlServerInputFormsApi()
    {
        try
        {
            this.AddEntityType(typeof(MessageAttribute));
            this.AddEntityType(typeof(MessageProperty));
            this.AddEntityType(typeof(MessageProtocol));
            this.AddEntityType(typeof(ValidationModel));
            this.Init();
        }
        catch(Exception ex)

        {
            this.Error($"Исключение при создании объекта {GetType().GetTypeName()} => {ex.Message}");
        }
    }

    public List<MessageProtocol> GetTablesProtocols()
    {
        List<MessageProtocol> protocols = new List<MessageProtocol>();

        foreach(var kv in this.GetTablesMetadata())
        {
            protocols.Add(GetMessageProtocol(kv.Value));
        }
        return protocols;
    }

    public MessageProtocol GetMessageProtocol(TableMetadata tableMetadata)
    {
        tableMetadata.EnsureIsValide();
        var protocol = new MessageProtocol();
        protocol.Label = tableMetadata.TableLabel;
        protocol.Name = tableMetadata.TableName;
        foreach(var kv in tableMetadata.ColumnsMetadata)
        {
            var property = GetMessageProperty(kv.Value);            
            property.MessageProtocol = protocol;
            property.EnsureIsValide();
        }
        protocol.EnsureIsValide();
        return protocol;
    }

 

    public MessageProperty GetMessageProperty(ColumnMetadata columnMetadata)
    {
        var property = new MessageProperty();
        property.Label = property.Name = columnMetadata.ColumnName;
        
        switch (columnMetadata.DataType)
        {
            
        }
        return property;
    }

    public List<MessageProtocol> GetProceduresProtocols()
    {
        throw new NotImplementedException();
    }
}

