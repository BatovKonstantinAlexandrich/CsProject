using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;

using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization
{
    [Label("Модуль бинзнес-логики")]
    public class UserBusinessServiceTests : TestingElement
    {
        public UserBusinessServiceTests() : this(AppProviderService.GetInstance())
        {
        }

        public UserBusinessServiceTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            AssertService<UserBusinessService<DeliveryDbContext>>
            (
                OnUserBusinessServiceTest,
                $"{nameof(UserBusinessService<DeliveryDbContext>)} протестирован и готов к использованию",
                $"{nameof(UserBusinessService<DeliveryDbContext>)} не пригоден для использования"
            );
        }

        private bool OnUserBusinessServiceTest(UserBusinessService<DeliveryDbContext> service)
        {
            using (var bdc = new DeliveryDbContext())
            {
                service.SetDbContext(bdc);
                bdc.Database.EnsureDeleted();
                bdc.Database.EnsureCreated();
                bdc.GetInitializer<DbContextUser>().Init(bdc);
                const int roleId = 2;
                var user = bdc.UserContexts
                    .Include(p => p.UsersResources)
                    .ThenInclude(p => p.Role)
                    .Include(p => p.Account)
                    .Where(p => p.UsersResources.Select(r => r.Role).Distinct().Any(ur => ur.Id == roleId))
                    .FirstOrDefault();
                UserAccount account = user is not null ? user.Account : null;
                if (account is null)
                {
                    var signup = provider.Get<SignupUser>();
                    signup.SetDbContext(bdc);
                    var signupResult = signup.SignupRoles(new UserAccount()
                    {
                        Email = $"Holder@gmail.com",
                        Password = "Holder@gmail.com",
                        Hash = UserAccount.GetHashSha256("Holder@gmail.com")
                    }, new UserPerson(), bdc.BusinessResources.Select(r => r.Name).ToArray());
                    if (signupResult.Result is not null)
                    {
                        account = signupResult.Result.Account;
                    }
                }
                this.Info(UserAccount.GetHashSha256("p@ssword"));
                account.Hash = UserAccount.GetHashSha256(account.Password);
                bdc.SaveChanges();
                this.Info(account);
                var signin = provider.Get<SigninUser<DeliveryDbContext>>();
                signin.SetDbContext(bdc);
                this.Info(bdc.GetConnectionString());
                var sr = signin.Signin(account);
                var busainessResources = service.GetBusinessResources();
                this.Info(busainessResources);
                Messages.Add("Пользователь может получить набор ролевых моделей");

                var businessFunctions = service.GetBusinessFunctions();
                this.Info(businessFunctions);

                var environmentBusinessFunctions = service.GetBusinessFunctionsForEntity<MessagesDbContextInitializer.Environment>();
                this.Info(environmentBusinessFunctions);
                Messages.Add("Пользователь может получить набор бизнес функций");

                var messageProtocols = service.GetMessageProtocols();

                Messages.Add("Пользователь может получить протоколы сообщений MessageProtocols");
                if (busainessResources.Count() > 0 && messageProtocols.Count() > 0)
                {
                    Messages.Add("Недостаточно данных по бизнес-модели");
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
