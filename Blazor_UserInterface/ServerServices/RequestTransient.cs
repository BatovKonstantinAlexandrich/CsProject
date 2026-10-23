public interface RequestTransient
{
    SessionContext GetSession();
    string Identify();
    ISessionManager GetSessionManager();
    ViewContext GetRoot();
}