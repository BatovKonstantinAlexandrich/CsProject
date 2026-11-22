public class ViewServicesUnit : TestingUnit
{
    public ViewServicesUnit(IServiceProvider parent) : base(parent)
    {
        this.Append(new NavMenuServiceTest(parent));
    }
}


