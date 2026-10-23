
[Label("Тестирование атрибутов")]
public class LibAttrbiutesUnit : TestingUnit
{


    public LibAttrbiutesUnit(IServiceProvider parent) : base(parent)
    {
        Append(new AttributesEntityUnit(parent));
        Append(new AttributesInfoUnit(parent));
        Append(new AttributesInputUnit(parent));
        Append(new AttributesControlUnit(parent));
    }
}