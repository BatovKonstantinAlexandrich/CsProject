using Console_CommonModel.CommonLib.LibModels;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

[Label("ClassServiceModel: ClassDbBuilder собирает сущности DbContext")]
[Description("AddDbContextEntities заполняет EntityClassModels типами из DbSet выбранного контекста.")]
public class ClassServiceModelBuilderCollectsEntitiesTest : TestingElement
{
    public ClassServiceModelBuilderCollectsEntitiesTest(IServiceProvider provider) : base(provider) { }

    public override void OnTest()
    {
        LogMessage("ClassDbBuilder.AddDbContextEntities<ServiceBuilderDbContext>()");
        Assert(_ =>
        {           
            var builder = new ClassDbBuilder();
            builder.AddDbContextEntities<ModelsDbContext>();
            LogMessage($"Собрано ClassModel для сущностей: {builder.EntityClassModels.Count}");
            return builder.EntityClassModels.Count > 0;
        },
        "ClassDbBuilder добавляет ClassModel для сущностей ServiceBuilderDbContext",
        "ClassDbBuilder не создал модели сущностей");
    }
}
