[Label("Вызов метода при изменении свойства")]
[HelpMessage("Метод должен принимать аргумент PropertyChangedMessage")]
public class OnChangeAttribute: ViewAttribute
{
    // имя свойства - источника изменений
    private string _propertyName;

    public OnChangeAttribute()
    {
         
    }

    public OnChangeAttribute(string propertyName)
    {
        _propertyName = propertyName;
    }
}