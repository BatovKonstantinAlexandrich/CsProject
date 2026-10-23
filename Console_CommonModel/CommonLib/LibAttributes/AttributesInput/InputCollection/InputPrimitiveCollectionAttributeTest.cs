[Label("Проверка примитивных коллекций")]
public class InputPrimitiveCollectionAttributeTest: TestingElement 
{
    public InputPrimitiveCollectionAttributeTest(IServiceProvider sp) : base(sp)
    {
    }

    public InputPrimitiveCollectionAttributeTest( ) : this(AppProviderService.GetInstance())
    {
    }

    public override void OnTest()
    {
        var attr = new InputPrimitiveCollectionAttribute();
        this.Assert(test => true == attr.IsValidValue(new List<string>()),
            "Удалось определить примитивную коллекцию: new List<string>()",
            "Не удалось определить примитивную коллекцию: new List<string>()");
        this.Assert(test => true == attr.IsValidValue(new List<int>()),
            "Удалось определить примитивную коллекцию: new List<int>()",
            "Не удалось определить примитивную коллекцию: new List<int>()");
        this.Assert(test => false == attr.IsValidValue(new List<object>()),
            "Удалось определить коллекцию ссылочных типов: new List<object>()",
            "Не удалось определить коллекцию ссылочных типов: new List<object>()");
   
    }

    public static void Run(string[] args)
    {
        new InputPrimitiveCollectionAttributeTest().DoTest(false).ToDocument().WriteToConsole();
    }
     
}