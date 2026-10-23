namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

[Label("ClassDataModel: свойства ClassModel для генерации кода")]
[Description("Создание ClassModel и проверка инициализации коллекций Properties и Methods.")]
public class ClassDataModelClassModelPropertiesTest : TestingElement
{
    public ClassDataModelClassModelPropertiesTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("Инициализация ClassModel с именем, Label и Description");
        Assert(_ =>
        {
            var model = new ClassModel
            {
                Name = "SampleEntity",
                Label = "Пример",
                Description = "Тестовая модель"
            };
            LogMessage($"ClassModel.Name = {model.Name}, Properties и Methods инициализированы");
            return model.Name == "SampleEntity"
                   && model.Properties != null
                   && model.Methods != null;
        },
        "ClassModel создаётся и содержит коллекции свойств и методов",
        "ClassModel не инициализируется корректно");
    }
}
