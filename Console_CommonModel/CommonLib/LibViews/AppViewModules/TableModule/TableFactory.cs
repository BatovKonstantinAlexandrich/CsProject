[Label("Фабрика таблиц")]
[Description("Фабрика для создания табличных представлений из коллекций свойств объектов и словарей, автоматически определяющая колонки и строки данных")]
public class TableFactory
{
    internal TableView ForCollectionProperty(object item, string name)
    {
        var table = new TableView();

        Type ptype = null;
        foreach( object p in (dynamic)item)
        {
            ptype = p.GetType();
            table.DataRows.Add(p);
        }
        table.Columns = ptype.GetInputPropertiesSorted();
        return table;
    }

    internal TableView ForDictionary(object p)
    {
        var table = new TableView();
        table.Columns = new() { "Key", "Value" };
        var dictionary = (Dictionary<string, object>)p;
        foreach(var kv in dictionary)
        {
            table.DataRows.Add(new
            {
                key = kv.Key,
                values = kv.Value
            });
        }
        return table;
    }
}