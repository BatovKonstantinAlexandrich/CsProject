using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

[Label($"Тестирование модели {nameof(ClassDbContext)}")]
public class ClassDataModelModuleTest : TestingUnit
{
    public ClassDataModelModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ClassDataModelClassModelPropertiesTest(provider));
    }
}
