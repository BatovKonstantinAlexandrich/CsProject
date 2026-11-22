using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

public interface IActionsContext
{
    Action AddAction(string Name, MethodModel method, Action<Dictionary<string, object>, Action<object>, Action<string>> Todo);
    object DoAction(string Action, Dictionary<string, object> args);
    void DoAction(string Action, Dictionary<string, object> args, Action<object> onsuccess, Action<string> onerror);
    ActionsModel GetActionModel(string name);
    List<MethodModel> GetActions();
}