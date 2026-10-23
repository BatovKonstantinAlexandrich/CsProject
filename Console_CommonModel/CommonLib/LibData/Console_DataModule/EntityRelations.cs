using System.ComponentModel.DataAnnotations.Schema;

[Label("Модель сведений об отношениях сущностей")]
[Description("Показывает 1 связь сущности с другой")]
public class EntityRelations
{
    [Label("Тип сущности")]
    [NotNullNotEmpty]
    public string EntityTypeName { get; set; }

    [Label("Имя навигации")]
    [NotNullNotEmpty]
    public string PropertyName { get; set; }

    [Label("Тип свойства")]
    [NotNullNotEmpty]
    public string PropertyTypeName { get; set; }


    /// <summary>
    /// Определённый во внешней таблицы
    /// </summary>
    [Label("Внешний ключ")]
    public string? ForeignKey { get; set; }

    [Label("Тип отношений")]
    [NotNullNotEmpty]
    public string RelationshipType { get; set; }

    [Label("Тип сущности свчязи много-ко-многим")]
    public string RelativeEntityType { get; set; }


    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotMapped]
    public List<EntityRelations> RelativeEntityRelations { get; set; } = new();




    public void Trace(int level = 1)
    {
        for (int i = 0; i < level; i++)
        {
            System.Console.Write("  ");
        }
        System.Console.WriteLine(this.ToString());
        foreach (var rel in RelativeEntityRelations)
        {
            rel.Trace(level+1);
        }
    }

    public List<List<string>> GetRelationsList(List<string> before = null)
    {
        if(before == null)
            before = new List<string>();
        List<List<string>> results = new();
        var added = new List<string>(before);
        added.Add(RelativeEntityType);
        results.Add(added);
        foreach (var r in RelativeEntityRelations)
        {            
            foreach (var list in r.GetRelationsList(added))
            {
                results.Add(list);
            }
        }
        return results;
    }
     

    public override string ToString()
    {
        var s = this.RelationshipType.ToString();
        var arr = s.Split("-");
        return $"{arr[0]} {EntityTypeName} - to - {arr[2]} {RelativeEntityType}";
    }

}