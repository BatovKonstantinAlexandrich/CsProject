
/// <summary>
/// Фабрика компонентов, используется в прцоессе десериализации XML
/// </summary>
public interface IBlazorComponentFactory
{
    public object Create(ComponentParams parameters, ComponentModel Parent, global::DataReport Root);
}

