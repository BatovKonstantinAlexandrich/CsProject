using System.Collections.Concurrent;

using static ClassModelLib.ActionsClient.ActionDbContext;


namespace WebApi_CommonModel.MiddlewareComponents
{
    public interface IActionBuilder
    {
        public Dictionary<string, HubActionModel> Build(ActionServiceModel serviceModel);
    }
}
