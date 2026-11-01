using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;

[Label("Класс для формирования TableMetaData")]
public class TableMetadataBuilder
{
    [Label("Редактируемый объект")]
    public TableInfo model { get; set; } = new();

    [NotInput]
    [Label("Завершить редактирование")]
    public TableInfo Build() => model;

    [NotInput]
    [Label("Завершить редактирование")]
    public TableInfo Build(string schema) {
        model.schema = schema;
        return model;
    }


    [Label("Добавить первичный ключ")]
    public ColumnMetaData AddPrimaryKey()
    {
        model.columns["Id"] = new();
        model.columns["Id"].name = "Id";
        model.columns["Id"].type = "int";        
        model.columns["Id"].incremental = true;
        return model.columns["Id"];
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="indicators">список наименований</param>
    [Label("Добавление статистических показателей")]
    
    public void AddIndicators(params string[] indicators )
    {
        foreach (var indicator in indicators)
        {
            model.columns[indicator] = new();
            model.columns[indicator].name = indicator;
            model.columns[indicator].type = "int";            
        }        
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="table"></param>
    [Label("Добавляет в описание таблицы сведения о ссылки из другой таблице")]
    public void AddReferenceOneToOne([Label("Схема")][NotNullNotEmpty()]string schema, [Label("Схема")] string table, [Label("Схема")] string column)
    {
        model.columns[column] = new();
        model.columns[column].name = column;
        model.columns[column].type = "int";
        model.columns[column].nullable = false;
        model.columns[column].unique = true;
        model.fk[column] = $"[{schema}].[{table}]";
    }    
}