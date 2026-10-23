using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;


/// <summary>
/// Класс для формирования TableMetaData
/// </summary>
public class TableMetadataBuilder
{
    private TableInfo model = new();


    public TableInfo Build() => model;
    public TableInfo Build(string schema) {
        model.schema = schema;
        return model;
    }


    /// <summary>
    /// добавление первичного ключа
    /// </summary>
    public void AddPrimaryKey()
    {
        model.columns["Id"] = new();
        model.columns["Id"].name = "Id";
        model.columns["Id"].type = "int";        
        model.columns["Id"].incremental = true;
    }


    /// <summary>
    /// Добавление статистических показателей
    /// </summary>
    /// <param name="indicators">список наименований</param>
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
    /// Добавляет в описание таблицы сведения о ссылки из другой таблице
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="table"></param>
    public void AddReferenceOneToOne(string schema, string table, string column)
    {
        model.columns[column] = new();
        model.columns[column].name = column;
        model.columns[column].type = "int";
        model.columns[column].nullable = false;
        model.columns[column].unique = true;
        model.fk[column] = $"[{schema}].[{table}]";
    }    
}