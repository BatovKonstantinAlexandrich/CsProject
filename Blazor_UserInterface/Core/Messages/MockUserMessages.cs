using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MockUserMessages : IUserMessagesService
{
    public List<UserMessage> GetInbox(int userId)
    {
        return new List<UserMessage>() {
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=1,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=1,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=1,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
        };
    }

    public List<UserMessage> GetOutbox(int userId)
    {
        return new List<UserMessage>() {
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=userId,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=userId,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
            new UserMessage(){
                Created=DateTime.Parse("01.01.2020"),
                FromUserID=userId,
                ToUserID=userId,
                Subject="Информация для пользователей",
                Text="Привет бро"
            },
        };
    }
}
