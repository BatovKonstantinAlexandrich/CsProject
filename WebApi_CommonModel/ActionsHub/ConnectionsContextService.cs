using System;
using System.Collections.Generic;
using System.Linq;

namespace RootHost.ActionsHub
{


    public interface IConnectionsContextService
    {
        public List<string> GetServiceNames();
        public List<string> GetServiceActionNames(string service);
        public HubActionModel GetServiceActionModel(string service, string action);
    }




    /// <summary>
    /// 
    /// </summary>
    public class ConnectionsContextService: IConnectionsContextService
    {
        private readonly ConnectionsContext connectionsContext;

        public ConnectionsContextService(ConnectionsContext connectionsContext)
        {
            this.connectionsContext = connectionsContext;
        }

        public List<string> GetServiceNames()
        {
            return connectionsContext.Connections.Values.Where(val => !String.IsNullOrWhiteSpace(val.Name)).Select(val => val.Name).ToList();
        }

        public List<string> GetServiceActionNames(string service)
        {
            var consumer = connectionsContext.Connections.Values.FirstOrDefault(val => val.Name == service);
            return consumer.Actions.Values.Select(val => val.Name).ToList();
        }

        public HubActionModel GetServiceActionModel(string service, string action)
        {
            var consumer = connectionsContext.Connections.Values.FirstOrDefault(val => val.Name == service);
            var model = consumer.Actions.Values.FirstOrDefault(val => val.Name== action);
            return model;
        }
    }
}
