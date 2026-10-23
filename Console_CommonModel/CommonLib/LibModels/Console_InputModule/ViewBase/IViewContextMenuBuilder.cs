
using Console_CommonModel.CommonLib.LibViews.AppViews.ViewServices;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Reflection;

public interface IViewContextMenuBuilder
{
    ContextView ContextMenu { get; }
    ContextViewBuilder ContextMenuBuilder { get; set; }

    Action AddContextRadioGroup([NotNullNotEmpty] string name, [NotNullNotEmpty] ChangeSupport service, Action<string> onchanged = null);
    Action AddContextSelectOption([NotNullNotEmpty] string name, string value, ChangingList<string> options, Action<string> onchanged = null);
    Action AddContextMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] Dictionary<string, Action> todos);
    Action AddContexxtMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] IDictionary<string, Action> todos);
    Action AddContextServiceMethods([NotNullNotEmpty] string name, [NotNullNotEmpty] object service, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider);
    Action AddContextServiceMethods<TService>([NotNullNotEmpty] string name, [NotNullNotEmpty] TService service);
    Action AddContextServiceMethods<TService>([NotNullNotEmpty] string name, [NotNullNotEmpty] TService service, [NotNullNotEmpty] Func<MethodInfo, object[]> parametersProvider);
    Action AddContextServiceOptions<TServiceOptions>( [NotNullNotEmpty] object service);
    ContextView AddContextView(string menu, string text, Action onclick);
    Action AddServiceMethods(object service);
    ContextView CreateContextAction(string name, Action todo);
    
    ContextView CreateContextMenuView(IEnumerable<object> services, Func<object, string> getTextForService, Func<object, List<string>> getActionsForService, Func<object, string, string> getTextForAction, Func<object, string, object> invokeServiceMethod);
    ContextView CreateContextOptionsView(object target, Type delaringType);
    ContextView CreateContextView(string text, Dictionary<string, Action> operations);
    ContextView CreateContextView(string text, object service);
    
    IDictionary<string, Action> GetActionMap(List<object> services);
    IDictionary<string, Action> GetActionMap(object service, Type ptype);
    object[] GetUserInputParameters(MethodInfo m);
}
