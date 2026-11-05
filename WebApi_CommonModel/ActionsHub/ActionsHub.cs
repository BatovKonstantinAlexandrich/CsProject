using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;


/// <summary>
/// Обмен описаниями методов действий
/// </summary>
public class ActionsHub: HubBase 
{
    private readonly ActionContextService actionsService;

    public ActionsHub(ILogger<ActionsHub> logger, IHttpContextAccessor httpContextAccessor, ConnectionsContext clientsContext, ActionsContext serviceContext) : base(logger, httpContextAccessor, clientsContext, serviceContext)
    {
        this.actionsService = new ActionContextService(this.actionsContext);        
    }


    public Dictionary<string, List<MethodModel>> ListClientActions()
    {
        int counter = 1;
        Dictionary<string, List<MethodModel>> result = new();
        foreach (var kv in connectionsContext.Connections)
        {
            if (kv.Key == this.Context.ConnectionId)
                continue;

            this.Info(kv.Value.Name);
            //if (kv.Value.Name is null)
            //  throw new Exception("Не задано имя клиента");
            var clientName = kv.Value.Name is null ? (counter++).ToString() : kv.Value.Name;
            result[clientName] = kv.Value.Actions.Values.Select(a => (MethodModel)a).ToList();
        }
        return result;
    }


    public HubActionModel GetActionModel(string action) => actionsContext.GetActionModel(action);


    public string Login(string name)
    {
        return connectionsContext.Connections[Context.ConnectionId].Name = name;
    }


    public string GetLoginName()
    {
        return connectionsContext.Connections[Context.ConnectionId].Name;
    }

    public void Publish(string json)
    {                       
        MethodModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodModel>(json);
        _logger.LogInformation($"Publish({model.Name})");

        var action = model.Name;

        connectionsContext.Connections[Context.ConnectionId].Rollback.Add(
            connectionsContext.Connections[Context.ConnectionId].AddAction(action, model));
        var ConnectionID = Context.ConnectionId;

        connectionsContext.Connections[Context.ConnectionId].Rollback.Add(
            actionsContext.AddAction(action, model, (args, onsuccess, onerror) => {

                _logger.LogInformation($"ExecuteAction({action})");
                
                var consumerController = connectionsContext.Connections[ConnectionID];
                object response = null; 
          
                var requestTask = consumerController.Request(action, args, 
                    (resp)=> {                        
                        _logger.LogInformation(ConnectionID);
                        _logger.LogInformation("SUCCESS " + resp);
                        onsuccess(resp);
                        //response = resp;
                        //completed = true;
                    },
                    (error) =>{
                        _logger.LogInformation("ERROR " + error);
                        onerror(error);
                        //response = error;
                        //completed = true;
                    }
                );             
            }));
        var connectionId = Context.ConnectionId;
        foreach(var kv in this.connectionsContext.Connections.Where(kv => kv.Key != connectionId).ToList())
        {
            var otherConnectionId = kv.Key;
            var pars = new Dictionary<string, string>() {
                { "method", model.ToJson() }
            };
            kv.Value.Request("OnPublish", pars, 
                p => {
                    this.Info($"[{kv.Key}].OnPublish()");
                }, 
                p => { 
                }).Wait();
        }
    }

    public void UnPublish(string json)
    {
        var model = Newtonsoft.Json.JsonConvert.DeserializeObject<MethodModel>(json);
        _logger.LogInformation($"UnPublish({model.Name})");

        var action = model.Name;
        connectionsContext.Connections[Context.ConnectionId].Actions.Remove(action);

        var connectionId = Context.ConnectionId;
        foreach (var kv in this.connectionsContext.Connections.Where(kv => kv.Key != connectionId).ToList())
        {
            var otherConnectionId = kv.Key;

        }
    }
}
