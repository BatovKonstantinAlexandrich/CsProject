
using Console_CommonModel.CommonLib.LibAttributes;

public class InputCollectionDictionaryAttributeTest : InputAttributeTesting<InputCollectionDictionaryAttribute>
{
    public InputCollectionDictionaryAttributeTest( ) : this(AppProviderService.GetInstance())
    {
    }
    public InputCollectionDictionaryAttributeTest(IServiceProvider provider) : base(provider)
    {
    }
}