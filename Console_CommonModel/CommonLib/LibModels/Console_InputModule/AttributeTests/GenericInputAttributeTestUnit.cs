namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.AttributeTests
{

    [Label("Модуль тестирования атрибутов ввода")]
    public class GenericInputAttributeTestUnit : TestingUnit
    {
        public GenericInputAttributeTestUnit()
        {
            AppendTestGeneration(typeof(GenericInputAttributeTest<>),
                ServiceFactory.Get().GetTypesExtended<BaseInputAttribute>()
                    .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
            );
        }

        public GenericInputAttributeTestUnit(IServiceProvider parent) : base(parent)
        {
            AppendTestGeneration(typeof(GenericInputAttributeTest<>),
                ServiceFactory.Get().GetTypesExtended<BaseInputAttribute>()
                    .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition)
            );
        }
    }
}
