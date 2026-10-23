[Label("Информация об источнике данных ODBC")]
public class OdbcDatasourceInfo
{
    [Label("Наименование")]
    public string Name { get; set; }

    [Label("Драйвер")]
    public string DriverName { get; set; }

    [Label("Тип")]
    public string DsnType { get; set; }

    [Label("Платформа")]
    public string Platform { get; set; }

    [Label("Аттрибуты")]
    public string Attribute { get; set; }
}
