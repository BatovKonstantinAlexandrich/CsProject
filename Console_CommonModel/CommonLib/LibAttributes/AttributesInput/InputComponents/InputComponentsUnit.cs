
[Label("Тестирование ввода ссылочных типов")]
public class InputComponentsUnit : TestingUnit
{


    public InputComponentsUnit(IServiceProvider provider)
    {
        Append(new InputConstructorAttributeTest(provider));        
        Append(new InputControlTypeAttributeTest(provider));
        Append(new InputEntityTypeAttributeTest(provider));
        Append(new InputInputTypeAttributeTest(provider));               
    }
}
