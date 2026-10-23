[Label("Тестирование ввода коллекций объектов")]
public class InputCollectionUnit : TestingUnit
{
    public InputCollectionUnit(IServiceProvider provider)
    {
        Append(new InputCollectionDictionaryAttributeTest(provider));
        Append(new InputInterfaceCollectionAttributeTest(provider));
        Append(new InputPrimitiveCollectionAttributeTest(provider));
        Append(new InputStructureCollectionAttributeTest(provider));
    }
}
