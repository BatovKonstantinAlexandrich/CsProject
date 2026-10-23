namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

[Label($"Тестирование модели {nameof(ClassDbContext)}")]
public class ClassServiceModelModuleTest : TestingUnit
{
    public ClassServiceModelModuleTest(IServiceProvider provider) : base(provider)
    {
        Append(new ClassServiceModelBuilderCollectsEntitiesTest(provider));
    }
}
