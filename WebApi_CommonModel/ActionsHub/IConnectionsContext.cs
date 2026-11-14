
public interface IConnectionsContext
{    
    void Ping();
    void RemoveClientConnection(string connectionId);
}