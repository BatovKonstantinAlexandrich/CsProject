using System.Collections.Generic;

public interface IUserMessagesService
{
    List<UserMessage> GetInbox(int userId);
    List<UserMessage> GetOutbox(int userId);
}