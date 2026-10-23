using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.AppBusiness;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;
using Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessActionServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.BusinessTests
{
    [Label("Проверка бизнес модели")]
    [Description(
        "Проверяем что при регистрации бизнес функцйий создаётся процедура, таблица в првильной схеме." +
        "Проверяем что пользователь получает список функций для работы с определённым набором данных и" +
        "способен их выполнять согласно бизнес-логики")]
    public class BusinessTests : TestingElement
    {
        public BusinessTests(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            LogMessage("Регистрация бизнес-функций, процедур и выполнение от имени пользователя");
            Assert(p =>
            {
                var delivery = new DeliveryDbContext();
                LogMessage("DeliveryDbContext создан, signup/signin…");

                var signup = provider.Get<SignupUser<DeliveryDbContext>>();
                var signin = new SigninUser<DeliveryDbContext>();
                if (!signup.HasWith(new() { Email = "eckumoc@gmail.com" }))
                {
                    signup.Signup(new("eckumoc@gmail.com", "eckumoc@gmail.com"), new());
                }
                var signinResult = signin.Signin(new("eckumoc@gmail.com", "eckumoc@gmail.com"));
                if (!signinResult.Succeeded)
                {
                    Messages.Add("Не удалось авторизоваться");
                    return false;
                }
                var users = provider.Get<AuthorizationUsers>();
                var currentUser = signinResult.Result;
                if (!users.ContainsKey(currentUser.SecretKey))
                {
                    Messages.Add("Ключ доступа пользователя не подходит, либо коллекция не наоплнена.");
                    return false;
                }


                var f = provider.Get<UserBusinessFunctionsService<DeliveryDbContext>>();
                var service = provider.Get<BusinessUserService<DeliveryDbContext>>();
                var business = provider.Get<UserBusinessService<DeliveryDbContext>>();
                var userGroups = provider.Get<UserGroupsService<DeliveryDbContext>>();
                var deliveryGroup = new string[] { "Customer", "Holder", "Transport" };
                userGroups.CheckGroupsHasRegistered(deliveryGroup);
                userGroups.JoinToGroups(deliveryGroup);
                if (userGroups.GetUserGroups().All(g => deliveryGroup.ToList().Contains(g.Name)))
                {
                    Messages.Add("Не удалось пользователю назначить группу");
                }
                var addToBasket = new BusinessFunction()
                {
                    Name = "add_to_basket",
                    Output = f.CreateMessageProtocol<OrderItem>(),
                    Error = f.CreateMessageProtocol<ProcessError>()
                };
                var businessBuilder = AppProviderService.GetInstance().Get<IBusinessProcessBuilder>();
                businessBuilder =
                    businessBuilder.CreateUserBusinessFunction<Customer, Product>(new()
                    {
                        { "добавить в корзину", addToBasket}
                    });
                businessBuilder = businessBuilder
                    .CreateUserBusinessFunction<Holder, Order>(new()
                    {
                        { "оставить комментарий", new BusinessFunction(){
                            Output = f.CreateMessageProtocol<ProductComment>(),
                            Error = f.CreateMessageProtocol<ProcessError>()
                        }}
                    })
                    .CreateUserBusinessFunction<Holder, Order>(new()
                    {
                        { "оставить комментарий", new BusinessFunction(){
                            Output = f.CreateMessageProtocol<ProductComment>(),
                            Error = f.CreateMessageProtocol<ProcessError>()
                        }}
                    });
                var validator = AppProviderService.GetInstance().Get<BusinessProcessValidator<DeliveryDbContext>>();
                var process = businessBuilder.BuildBusiness();
                var messages = validator.Validate(process);

                this.Info(messages);
                this.Info(delivery.ValidateModel());

                messages.Values.ToList().ForEach(Messages.AddRange);
                messages.Values.ToList().ForEach(Messages.AddRange);
                ;



                var businessFunctions = business.GetBusinessFunctionsForEntity<Order>();

                //service.DoAction("test", new());
                var allFunctions = business.GetBusinessFunctions();
                if (allFunctions.Count() == 0)
                {
                }












                return false;
            },
            "Пользователь получает набор бизнес функций применяемых к определённым данным",
            "Пользователь неу умеет работать с данным");
        }
    }
}
