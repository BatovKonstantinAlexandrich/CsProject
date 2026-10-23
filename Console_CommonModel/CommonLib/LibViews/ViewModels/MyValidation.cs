
[Label("Управление валидацией")]
public interface MyValidation
{
    public string Validate( object value);
    public string Validate(object model, string property, object value);
    public string GetMessage(object model, string property, object value);
}