
using System.Collections.Generic;

using Blazor_CommonModel.Application;

using Microsoft.Extensions.Logging;
/// <summary>
/// Служба уровня приложения, содержит пользователей сеансов
/// </summary>
public class AuthorizationServices : AuthorizationCollection<ServiceContext>, APIServices
{
    public AuthorizationServices(AuthorizationOptions options, ApplicationEventsService applicationEventsService):base(options, applicationEventsService) { }

    public IDictionary<string, string> GetApis()
    {
        throw new System.NotImplementedException();
    }

    public IDictionary<string, string> Signin(UserContext user)
    {
        foreach(var service in this.GetAll())
        {
            
        }
        return null;
    }
}

