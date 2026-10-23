 
/// <summary>
/// Подключаемый сервис управления
/// </summary>
/// <typeparam name="T">тип элемента управления</typeparam>
public interface IControlService<T>
{
    public T Control { get; set; }
    public void SetControl(T item);
}


public interface IControlService<T, TInterface>
{
    public T Control { get; set; }
    public void SetControl(T item);

    public TInterface BuildControl();
}