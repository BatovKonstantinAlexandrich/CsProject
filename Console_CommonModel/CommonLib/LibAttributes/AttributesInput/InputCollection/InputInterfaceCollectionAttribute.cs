
[Label("Ввод объектов расширяющих заданный тип")]
public class InputInterfaceCollectionAttribute : InputInterfaceCollectionAttribute<Dictionary<string, string>>
{
    public InputInterfaceCollectionAttribute() : base(InputTypes.Custom)
    {
    }

    public InputInterfaceCollectionAttribute(string expression) : base(expression)
    {
    }
}



[Label("Ввод объектов расширяющих заданный тип")]
[Description($"Свойство реализует тип коллекции с аналогичным типом {nameof(TInterface)}")]
public class InputInterfaceCollectionAttribute<TInterface> : BaseInputAttribute
    where TInterface : class
{

    [Label("Выражение для формирования текстовой надписи")]
    public string Expression { get; set; }


    public InputInterfaceCollectionAttribute() : this("ToString()") { }

    public InputInterfaceCollectionAttribute(string expression) : base(InputTypes.Custom)
    {
        Expression = expression.Replace("\"", "");
    }


    /// <summary>
    /// Возвращает имена типов реализующих базовый тип
    /// </summary>        
    public List<string> GetOptions()
    {
        var genericType = typeof(TInterface);
        if (genericType.IsInterface)
        {
            return ServiceFactory.Get().GetTypesImplements<TInterface>().Where(t => !t.IsAbstract).Select(t => t.GetTypeName()).ToList();
        }
        else
        {
            return ServiceFactory.Get().GetTypesExtended<TInterface>().Where(t => !t.IsAbstract).Select(t => t.GetTypeName()).ToList();
        }
    }
}
