[Label("Тестирование атрибутов сущности")]
public class AttributesEntityUnit : TestingUnit
{
    public AttributesEntityUnit() : this(AppProviderService.GetInstance()) { }
    public AttributesEntityUnit(IServiceProvider provider)
    {
        this.Append(new ManyToManyAttributeTest(provider));
        this.Append(new UniqueConstraintAttributeTest(provider));
        this.Append(new SearchTermsAttributeTest(provider));
    }

    
}
