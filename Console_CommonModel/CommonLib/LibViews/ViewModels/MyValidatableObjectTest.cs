
[Label("Проверка выполнения функций валидации")]
[Description("Объект должен проверяться с помощью атрибутов " +
    "реализующих интерфейс MyValidation. Если объект реализует ValidateOptional" +
    "то проверка должна выполняться с общей функцией валидации. " 
    +"Должно передаваться событие изменения состояния валидности.")]
public class MyValidatableObjectTest : TestingElement
{
    public MyValidatableObjectTest() : this(AppProviderService.GetInstance())
    {
    }
    public MyValidatableObjectTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        this.Assert(p =>
        {
            var account = new UserAccount();
            bool? accountIsValid = null;
            account.OnValidationStateChanged += (isValide) => {
                accountIsValid = isValide;
            };
            account.Validate();

            return accountIsValid != null;
        },
        "Реализована поддержка события изменения состояния is_valide объекта",
        "Не реализована поддержка события изменения состояния is_valide объекта");

        this.Assert(p =>
        {
            var account = new UserAccount();
            account.ValidationSuccess = true;
            bool? accountIsValid = null;
            account.ValidateOnInput = true;
            account.Email = "eck";
            account.Validate();

            return accountIsValid != null;
        },
        "При validateOnInput=true выполняется валидация объекта при изменении свойств",
        "При validateOnInput=true не выполняется валидация объекта при изменении свойств");
    }
}