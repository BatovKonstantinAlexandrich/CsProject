using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

[Label("Проверка функции авторизации")]
public class AuthorizationServiceTest : TestingElement
{
    public AuthorizationServiceTest() { }
    public AuthorizationServiceTest(IServiceProvider provider) : base(provider)
    {
    }

    public override void OnTest()
    {
        try
        {
            var signup = provider.Get<SignupUser>();
            var account = new UserAccount("eckumoc@gmail.com", "Gye*34FRtw");
            if (!signup.HasWith(account))
            {
                signup.Signup(account, new()
                {
                    
                });
            }
            var auth = provider.Get<SigninUser<DeliveryDbContext>>();
            var result = auth.Signin(account);
            this.Info(result.ToJsonOnScreen());
              
        }
        catch(Exception ex)
        {
            Failed = true;
            Messages.Add(ex.Message);
        }

    }
}
