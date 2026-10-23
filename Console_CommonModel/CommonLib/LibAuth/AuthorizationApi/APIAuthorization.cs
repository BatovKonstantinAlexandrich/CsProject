 

using System.Collections.Concurrent;
using System.Threading.Tasks;

public interface APIAuthorization:  APIRegistration
{
    bool IsSignin();
    bool InBusinessResource(string roleName);
    bool IsActivated();
    UserContext  Signin(string RFIdLabel);
    UserContext  Signin(string Email, string Password, bool? IsFront=false);
    void Signout(bool? IsFront = false);
    UserContext  Verify(bool? IsFront = false);               
    ConcurrentDictionary<string, object> Session();
    Task<UserAccount> GetAccountById(int iD);
    string GetUserHomeUrl();

    string Login(string Email, string Password);
    bool InRole(string roleName);
    IReadOnlyList<ServiceContext> GetConnectedServices();
    ServiceContext RegisterService(string name, string email, string password, string description = null);
    ServiceContext SigninService(string email, string password, bool? IsFront = false);
    ServiceContext SinginService(string email, string password, bool? IsFront = false);
}


public interface APIAuthorizationOptions
{
    int CheckTimeout { get; }
    bool LogginAuth { get; }
    int SessionTimeout { get; }
}

