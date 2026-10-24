
using Console_CommonModel.CommonLib.LibAuth.AppAuth;

using System;
using System.Linq;

/// <summary>
/// 
/// </summary>
[Label("Проверка методов работы с сообщениями пользователя")]
public class UserMessagesTest: TestingService<UserMessagesService<DeliveryDbContext>>
{
    public UserMessagesTest(IServiceProvider sp) : base(sp)
    {
    }

    public override void OnTest()
    {
        AssertService<UserMessagesService<DeliveryDbContext>>(OnServiceTest,
            "Умеет получать, отправлять сообщения",
            "Не умеет получать, отправлять сообщения");
        OnUserTest();
    }

    public override bool OnServiceTest(UserMessagesService<DeliveryDbContext> service)
    {
       
        var signup = provider.Get<SignupUser<DeliveryDbContext>>();
        if(!signup.HasWith(new()))
        {
            signup.Signup(new("customer@gmail.com", "customer@gmail.com"),new());
        }
        var signin = provider.Get<SigninUser<DeliveryDbContext>>();
        signin.Signin("customer@gmail.com", "customer@gmail.com");
        var before = service.GetInbox().Count();
        var userId = signin.Verify().Id;
        var message = new UserMessage()
        {
            FromUserId = userId,
            ToUserId = userId,
            Subject = "Новая тема",
            Text = "Тест"
        };
        service.Send(message, null);
        if ((service.GetInbox().Count() - before) != 1)
            return false;
        if (service.GetOutbox().Any(m => m.Id == message.Id) == false)
            return false;
        return true;
        
    }
    public void OnUserTest()
    {
        var context = provider.Get<DeliveryDbContext>();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        var signup = provider.Get<SignupUser<DeliveryDbContext>>();

        if (context.UserContexts.Count() == 0)
        {
            var res1 = signup.Signup(new UserAccount()
            {
                Email = "putin@gmail.com",
                Password = "putin@gmail.com",
            }, 
            new UserPerson()
            {
                FirstName = "Владимир",
                LastName = "Владимирович",
                SurName = "Путин",
                Birthday = DateTime.Parse("26.08.1989"),
                Tel = "7-921-090-3533"
            });


            var res2 = signup.Signup(new UserAccount()
            {
                Email = "putin2@gmail.com",
                Password = "putin2@gmail.com",
            }, new UserPerson()
            {
                FirstName = "Александр",
                LastName = "Александрович",
                SurName = "Батов",
                Birthday = DateTime.Parse("26.08.1989"),
                Tel = "7-921-090-3534"
            });
        }
        context.Add(new UserMessage()
        {
            Subject = "Тест",
            Text = "Это тестовое сообщение",
            FromUserId = context.UserContexts.First().Id,// context.UserContexts.Include(u => u.Account).FirstOrDefault(user => user.Account.Email.ToLower() == "eckumoc@gmail.com").Id,
            ToUserId = context.UserContexts.First().Id//context.UserContexts.Include(u => u.Account).FirstOrDefault(user => user.Account.Email.ToLower() == "eckumocuk@gmail.com").Id
        });
        context.SaveChanges();
        Messages.Add("Регистрация отправки сообщения прошла успешно");


    }
}
