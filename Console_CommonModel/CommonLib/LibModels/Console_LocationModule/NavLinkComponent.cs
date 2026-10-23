using static Console_CommonModel.CommonLib.LibModels.Console_LocationModule.LocationDbContext;

namespace Console_CommonModel.CommonLib.LibModels.Console_LocationModule
{
    public class NavLinkComponent : PageComponent
    {
        public NavLinkComponent() { }
        public NavLinkComponent(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
    }
}