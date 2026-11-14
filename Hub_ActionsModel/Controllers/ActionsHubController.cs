using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Microsoft.AspNetCore.Mvc;

namespace Hub_ActionsModel.Controllers
{
    [ApiController]
    public class ActionsHubController(ActionsContext actions, [FromServices] ConnectionsContext connections) : Controller
    {
        [HttpGet]
        [Route("/api/actions/connections")]
        public async Task<IActionResult> GetConnectedClients()
        {
            await Task.CompletedTask;
            return Ok(connections.Connections.Select(c => c.Value).ToList());
        } 

        [HttpGet]
        [Route("/api/actions/connections/ping")]
        public void Ping([FromServices] ConnectionsContext connections)
        {
            connections.Ping();
        }

        [HttpDelete]
        [Route("/api/actions/connections")]
        public void RemoveClientConnection([FromServices] ConnectionsContext connections)
        {
            connections.Ping();
        }

        //[HttpPost]
        //[Route("/api/actions/connections/actions")]
        //public Action AddAction(string Name, MethodModel method, Action<Dictionary<string, string>, Action<object>, Action<string>> Todo)
        //    => actions.AddAction(Name, method, Todo);


      

        [HttpPost]
        [Route("/api/actions/connections/actions/do")]
        public IActionResult DoActionAsync(string Action, Dictionary<string, string> args)
        {
            bool completed = false;
            object result = null;
            string error = null;
            actions.DoAction(Action, 
                args,
                result => {
                    completed = true;
                }, 
                error => {
                    completed = true;
                });
            while (!completed)
                Thread.Sleep(100);
            return Ok(new
            {
                result,
                error
            });
        }

        [HttpGet]
        [Route("/api/actions/connections/actions/{name}")]
        public HubActionModel GetActionModel(string name)
            => actions.GetActionModel(name);

        [HttpGet]
        [Route("/api/actions/connections/actions")]
        public List<MethodModel> GetActions()
            => actions.GetActions();
    }
}
