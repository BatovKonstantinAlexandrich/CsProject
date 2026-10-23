
/// <summary>
/// Методы работы с сообщениями пользователя
/// </summary>
public interface IUserMessagesService
{

    /// <summary>
    /// Получение справочника пользователей
    /// </summary>    
    Dictionary<string, int> GetUsers();

    /// <summary>
    /// Получение входящих сообщенимй
    /// </summary>
    List<UserMessage> GetInbox();

    /// <summary>
    /// Получение исходящих сообщенимй
    /// </summary>
    List<UserMessage> GetOutbox();

    /// <summary>
    /// Отправка нового сообщения
    /// </summary>
    Task<int> Send(UserMessage p, List<ProductImage> files);

    /// <summary>
    /// Кол-во непрочитанных сообщений
    /// </summary>    
    int GetInboxNotReaded();
}
