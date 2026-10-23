
/// <summary>
/// Функции связывания элементов
/// </summary>
public interface IComponentModel
{
    public void PreLinking();
    public void PostLinking();
    public void OnError();
    public void OnMessage();
    public void OnInit();
    public void OnChanges();
    public void OnUpdate();
    public void OnDestroy();
}
