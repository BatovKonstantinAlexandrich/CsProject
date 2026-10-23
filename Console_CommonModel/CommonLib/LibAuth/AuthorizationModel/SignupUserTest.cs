using System;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth
{
    internal class SignupUserTest: TestingElement
    {
        internal object SignupTest(UserAccount userAccount, UserPerson userPerson)
        {
            var signin = AppProviderService.GetSingletonInstance().Get<SigninUser<DeliveryDbContext>>();
            var signup = AppProviderService.GetSingletonInstance().Get<SignupUser>();
            var account = new UserAccount("customer@gmail.com", "sgdf1423");
            MethodResult<UserContext> signupResult = null;
            if (!signup.HasWith(account))
            {
                var person = new UserPerson()
                {
                    SurName = "Иванов",
                };
                this.Info(person.Validate().ToJson());
                signupResult = signup.Signup(account, person);
                this.Info(signupResult.ToJson());

                if (signupResult.Succeeded == false)
                    throw new Exception("Не удалось зарегистрировать учетную запись");
            }
            var result = signin.Signin("customer@gmail.com", "sgdf1423");
            Console.WriteLine($"{result.Succeeded} {result.Result.ToJson()}");
            var user = result.Result;
            user.EnsureIsValide();
            return user;
        }

        public void SignupRolesTest()
        {
            var account = new UserAccount("test@gmail.com", "test@gmail.com");
            var person = new UserPerson("Егор", "Батов", "Александрович", DateTime.Parse("26.08.1989"), "79210901111");

            var signup = provider.Get<SignupUser>();

            if (!signup.HasWith(account))
            {
                signup.Signup(account, person);
            }
            var tests = provider.Get<SignupUser>();
            tests.SignupRoles(
                account, person,
                new string[] { "customer" });
            Messages.Add("Регистрация пользователя с явным указанием ролей");
            var result = provider.Get<SigninUser<DeliveryDbContext>>().Signin("test@gmail.com", "test@gmail.com");


            Messages.Add("Аутентификация по логину и паролю");
        }

        internal void SignupRoles(UserAccount userAccount, UserPerson userPerson, string[] strings)
        {

            var signup = AppProviderService.GetSingletonInstance().Get<SignupUser>();
            var account = new UserAccount() { Email = "customer@gmail.com", Password = "sgdf1423" };
            if (signup.HasWith(account))
            {
                return;
                //signup.Signout(account);
            }

            var result =
                signup.SignupRoles(
                account,
                new UserPerson()
                {

                }
                , new string[] { "admin" });
            if (!result.Succeeded)
            {
                throw new Exception($"Регистрация пользователя не работает: {result.Exception}");
            }
            var user = result.Result;
            Console.WriteLine($"{result.Succeeded} {user?.ToJson()}");

            user.EnsureIsValide();
        }

        public override void OnTest()
        {
            throw new NotImplementedException();
        }
    }
}