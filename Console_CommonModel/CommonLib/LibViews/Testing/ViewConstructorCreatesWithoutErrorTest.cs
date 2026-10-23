namespace Console_CommonModel.CommonLib.LibViews.Testing;

[Label("Тест конструктора типа {{_viewTypeName}}")]
public class ViewConstructorCreatesWithoutErrorTest : TestingElement
{
    public Type _viewType { get; set; }
    public string _viewTypeName { get; set; }

    public ViewConstructorCreatesWithoutErrorTest(Type viewType, IServiceProvider provider) : base(provider)
    {
        _viewType = viewType;
        _viewTypeName = _viewType.GetTypeName();
    }

    public override string GetTitle() =>
        $"[{_viewType.Name}] конструктор без ошибок";

    public override void OnTest()
    {
        var displayName = _viewType.FullName ?? _viewType.Name;
        Assert(_ =>
        {
            if (!LibViewsViewInstantiator.TryCreate(_viewType, out var instance, out var error))
            {
                _.Messages.Add($"{displayName}: {error}");
                return false;
            }

            _.Messages.Add($"{displayName}: создан экземпляр {instance!.GetType().Name}");
            return true;
        },
        $"{displayName} создаётся через конструктор",
        $"{displayName} не создаётся через конструктор");
    }
}
