using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class ConnectionsContext : AsyncContext, IConnectionsContext
{
    private readonly bool _Logging = true;


    /// <summary>
    /// Операции по наименованиям
    /// </summary>
    public ConcurrentDictionary<string, ActionsConsumer> Connections = new ConcurrentDictionary<string, ActionsConsumer>();


    public void Ping()
    {
        foreach (var kv in Connections)
        {
            kv.Value.Request("Ping", new Dictionary<string, object>() { },
                result => { }).Wait();
        }

    }

    public ActionsConsumer AddClientConnection(string ConnectionId, Action<string> Request)
    {
        return Connections[ConnectionId] = new ActionsConsumer(Request, this);
    }

    public void RemoveClientConnection(string connectionId)
    {
        Connections.TryRemove(connectionId, out var consumer);
        var rollback = consumer.Rollback.ToList();
        consumer.Rollback.Clear();
        foreach (var todo in rollback)
        {
            todo();
        }
    }

}