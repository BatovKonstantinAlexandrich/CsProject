namespace Console_CommonModel.CommonLib.LibViews.Testing;

[Label("Конструкторы представлений LibViews")]
public class LibViewsAreaConstructorTestsUnit : TestingUnit
{
    public LibViewsAreaConstructorTestsUnit(IServiceProvider provider, string areaName, IReadOnlyList<Type> viewTypes)
        : base(provider)
    {
        Name = areaName;
        foreach (var viewType in viewTypes)
            Append(new ViewConstructorCreatesWithoutErrorTest(viewType, provider));
    }

    public override string GetTitle() => $"LibViews.{Name}: конструкторы ({Children.Count})";
}
