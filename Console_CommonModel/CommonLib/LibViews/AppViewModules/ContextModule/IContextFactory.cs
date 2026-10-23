namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
    public interface IContextFactory
    {
        ContextView AddContextView(string menu, string text, Action onclick);
        ContextView CreateContextOptionsView(object target, Type delaringType);
        ContextView CreateContextView(string text, Dictionary<string, Action> operations);
        ContextView CreateContextView(string text, object service);
        Dictionary<string, Action> GetActionMap(List<object> services, Action<string, object> onresult = null);
        Dictionary<string, Action> GetActionMap(object service, Type ptype, Action<string, object> onresult = null);
        Dictionary<string, Action> GetActionNamesMap(List<object> services, Action<string, object> onresult = null);
    }
}