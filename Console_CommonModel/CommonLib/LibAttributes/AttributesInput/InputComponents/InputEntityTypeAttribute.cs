using Microsoft.EntityFrameworkCore;

[Label("Тип сущности")]
public class InputEntityTypeAttribute : InputSelectAttribute
{
     
    public static string GetEntityTypes()
    {
        List<string> list = new();
        foreach (var pdbtype in ServiceFactory.Get()
            .GetTypesExtended<DbContext>()
            .Where(p => p.GetConstructors().Any(c => c.GetParameters().Count()==0))
            .Where(p => !p.IsGenericTypeDefinition)
            .Where(p => !p.IsAbstract))
        {
            try
            {
                using (var pdb = (DbContext)pdbtype.New())
                {
                    list.AddRange(pdb.GetEntitiesTypes().Select(t => t.GetTypeName()));
                }
            }
            catch { }
        }
        string text = "";
        foreach (string controlType in list)
        {
            text += $"{controlType},";
        }
        if (text.EndsWith(","))
            text = text.Substring(0, text.Length - 1);
        return text;
    }


    public InputEntityTypeAttribute() : base(GetEntityTypes())
    {
    }

    public InputEntityTypeAttribute(string entity) : base(GetEntityTypes())
    {
    }
}
