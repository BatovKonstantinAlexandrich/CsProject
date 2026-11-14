using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

public interface IActionsContext
{
    Action AddAction(string Name, MethodModel method, Action<Dictionary<string, string>, Action<object>, Action<string>> Todo);
    object DoAction(string Action, Dictionary<string, string> args);
    void DoAction(string Action, Dictionary<string, string> args, Action<object> onsuccess, Action<string> onerror);
    HubActionModel GetActionModel(string name);
    List<MethodModel> GetActions();
}