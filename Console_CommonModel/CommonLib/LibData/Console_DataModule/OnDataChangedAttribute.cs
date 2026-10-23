using Microsoft.EntityFrameworkCore;



/// <summary>
/// События изменения данных TModel
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public abstract class OnDataChangedAttribute : Attribute
{
    public string TypeName { get; set; }
}



/// <summary>
/// Атрибут для регистрации метода на события изменения данных сущности
/// </summary>    
public class OnDataChangedAttribute<TModel> : OnDataChangedAttribute where TModel : class
{
    public OnDataChangedAttribute(string exp = null)
    {
        TypeName = typeof(TModel).GetTypeName();
    }
}
