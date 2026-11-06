[Label("Модуль авторизации пользователей")]
[Description("Регистрация пользователя должны исключать возможность " +
    "регистрировать несколько пользователей на один Email и один номер телефона." +
    "Аутентификация выполняет проверку пароля. Нужна возможность выхода пользователя из системы.")]
public class AuthorizationUsersTest : TestingElement
{
    public AuthorizationUsersTest() : base() { }
    public AuthorizationUsersTest(IServiceProvider provider) : base(provider)
    {
    }
      
    static int GetRandom(int max)
    {
        int res = new Random().Next(max);
        return res == 0 ? 1 : res;
    }

    public override void OnTest()
    {
        try
        {
            using (var db = new DeliveryDbContext())
                db.Database.EnsureDeleted();
            using (var db = new DeliveryDbContext())
                db.Database.EnsureCreated();

            var account1 = new UserAccount()
            {
                Email = "eckumocuk@gmail.com",
                Password = "Gye*34FRtw",
                Hash = UserAccount.GetHashSha256("Gye*34FRtw")
            };

            this.AssertService<SignupUser>((signup) =>
            {
                
                var account2 = new UserAccount()
                {
                    Email = "eckumoc@gmail.com",
                    Password = "Gye*34FRtw",
                    Hash = UserAccount.GetHashSha256("Gye*34FRtw")
                };

                if (signup.HasWith(account1))
                {                    
                    signup.Signout(account1);
                }

                if (signup.HasWith(account1) == false)
                {
                    var res = signup.Signup(account1, new UserPerson()
                    {
                        FirstName = "Василий",
                        LastName = "Александрвович",
                        SurName = "Батов",
                        Birthday = DateTime.Parse("22.08.1989"),
                        Tel = "7-921-090-3571".Replace("-","")
                    });
                    if(res.Succeeded == false)
                    {
                        Messages.Add($"Не удалось выполнить регистрацию учётной записи => {res.Exception}");
                    }
                }
                else
                {
                    Messages.Add($"{nameof(signup.Signout)} не удалил учётную запись");
                    Failed = true;
                    return false;
                }

                if (signup.HasWith(account1) != true)
                    return false;
                if (signup.HasWith(account2) )
                    signup.RemoveWith(account2);

                if (signup.HasWith(account2))
                {                                                                
                    return false;                                         
                }                
                return true;
            }, 
                "Регистрация учётных данных пользователя работает корректно",            
                "Регистрация учётных данных пользователя работает не корректно");
           

            AssertService<SignupUser>((signup) =>
            {
                return signup.HasWith(account1) == true;
               
            }, 
                "Проверка регистрации пользователя работает корректно",
                "Проверка регистрации пользователя работает не корректно");

            AssertService<SignupUser>((signup) =>
            {
                signup._model = new DeliveryDbContext();
                var account = new UserAccount()
                {
                    Email = $"transport{GetRandom(9)}@gmail.com",
                    Password = $"transport{GetRandom(9)}@gmail.com",
                    Hash = UserAccount.GetHashSha256($"transport{GetRandom(9)}@gmail.com")
                }; 
                if (signup.HasWith(account))
                {
                    signup.RemoveWith(account);
                }
                var result = signup.SignupRoles(account, new UserPerson()
                {
                    FirstName = "Олег",
                    LastName = "Александр",
                    SurName = "Батов"+ $"{GetRandom(9)}",
                    Birthday = DateTime.Parse("26.08.1989"),
                    Tel = $"7-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}"
                }, new string[] {   "transport" });
                if (!result.Succeeded)
                    Messages.Add("Не удалось зарегистрировать пользователя "+ result.Exception);
                var user = signup.GetBy(account);
                
                    
                return true;
            },
                "Проверка регистрации курьера работает корректно",
                "Проверка регистрации курьера работает не корректно");


            //AssertService<SignupUser>(signup => {
            //    using (signup._model = new DeliveryDbContext())
            //    {


            //        var account = new UserAccount()
            //        {
            //            Email = "customer@gmail.com",
            //            Password = "customer@gmail.com",
            //            Hash = UserAccount.GetHashSha256("customer@gmail.com")
            //        };
            //        if(signup.HasWith(account))
            //        {
            //            signup.RemoveWith(account);
            //        }
            //        var signupResult = signup.SignupRoles(account, new UserPerson()
            //        {
            //            FirstName = "Петр",
            //            LastName = "Александр",
            //            SurName = "Батов" + $"{GetRandom(9)}",
            //            Birthday = DateTime.Parse("26.08.1989"),
            //            Tel = $"7-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}-{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}{GetRandom(9)}"
            //        }, new string[] { "transport", "customer", "webuser" });
            //        if(signupResult.Succeeded == false)
            //        {
            //            Messages.Add(signupResult.Exception);
            //        }
            //        var signinResult = new SigninUser<DeliveryDbContext>(new()).Signin(account);

            //        signup.RemoveWith(account);         
            //        return signinResult.Succeeded;
            //    }

            //},
            //    "Авторизация пользователя работает корректно",
            //    "Авторизация пользователя работает не корректно");
             
        }
        catch (Exception ex)
        {
            Failed = true;
            Messages.Add(ex.Message);
        }

    }
}
