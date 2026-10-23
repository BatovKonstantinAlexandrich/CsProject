[Label("Тестирование атрибутов ввода")]
public class AttributesInputUnit: TestingUnit
{
    public AttributesInputUnit() : this(AppProviderService.GetInstance()) { }
    public AttributesInputUnit(IServiceProvider provider)
    {
        Append(new InputComponentsUnit(provider));
        Append(new InputCollectionUnit(provider));
        Append(new InputTextUnit(provider));
        Append(new InputNumberUnit(provider));
        Append(new InputDateUnit(provider));
        Append(new InputCustomUnit(provider));
    }
}
