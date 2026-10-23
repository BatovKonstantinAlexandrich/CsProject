
/// <summary>
/// Справочник операций
/// </summary>
[Label("Выполнение динамических операция")]
public class ActionExecuter: StateSupport
{
    [Label("Справочник операций")]
    [InputCollectionDictionary()]
    [NotInput()]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public ChangingDictionary<string, Func<object[], MethodResult<object>>> Todos { get; set; } = new();

    public ActionExecuter()
    {
        foreach(var method in this.GetType().GetMethods())
        {
            var methodName = method.Name;
            Todos[method.Name] = (object[] args) => {
                MethodResult<object> methodResult = null;
                try
                {
                    this.Info($"Выполнение {methodName} с аргументами {args.ToJson()}");
                    var result = this.Call(method.Name, args.ToList());
                    methodResult = MethodResult<object>.OnResult(result);
                }
                catch (Exception ex)
                {
                    methodResult = MethodResult<object>.OnError(ex);
                }                
                return methodResult;                
            };
        }
    }
   
    public MethodResult<object> TryActionExecute(string name, params object[] arguments)
    {
        if (!Todos.ContainsKey(name))
        {
            throw new InvalidOperationException(name);
        }
        else
        {
            try
            {
                return Todos[name](arguments);
            }
            catch (Exception ex) 
            {
                return MethodResult<object>.OnError(ex);
            }
        }
    }
}
