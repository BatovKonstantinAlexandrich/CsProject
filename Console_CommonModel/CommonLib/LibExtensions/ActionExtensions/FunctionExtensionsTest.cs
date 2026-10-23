[Label("Компиляция выражений в методы действия")]
public class FunctionExtensionsTest: TestingElement
{
    public FunctionExtensionsTest(IServiceProvider provider) : base(provider)
    {
    }

    public class FunctionExtensionsTestModel
    {
        public int Increment(int x)
        {
            return x + 1;
        }
    }

    public override void OnTest()
    {
        Assert(p => {
            var service = new FunctionExtensionsTestModel();
            var incrementFunction = service.CreateFunction<int>("Increment({{x}})");
            var incrementResult = incrementFunction(new() {
                { "x", "1"}
            });
            return incrementResult==2;
        },
        "Реализованы функции создания ссылок на методы действия на основе компиляции специальных выражений",
        "Не работают функции создания ссылок на методы действия на основе компиляции специальных выражений");
    }
}