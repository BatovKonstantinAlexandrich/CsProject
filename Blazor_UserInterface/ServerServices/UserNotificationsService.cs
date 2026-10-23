
using ApplicationDb.Entities;
using System.Collections.Concurrent;




public class UserNotificationsService: BaseService
{

    private readonly APIUsers _users;
    private readonly APIAuthorization _authorization;
 

    public UserNotificationsService(APIAuthorization authorization, APIUsers users)
    {

        this._users = users;
        this._authorization = authorization;
    
    }

    public void Error( string title )
    {
        AddNotification(new NotificationMessage() { 
            Title = title,
            Type="Error"
        });
    }

    public void Info( string title )
    {
        AddNotification(new NotificationMessage() { 
            Title = title
        });
        
    }


    public ConcurrentDictionary<string, object> GetSession()
    {
        if (this._authorization.IsSignin() == false)
        {
            return new ConcurrentDictionary<string, object>();
        }
        User user = this._authorization.Verify();
        return user!=null? this._users.GetSession(this._authorization.Verify().SecretKey): null;
    }


    /**
    * Передать сообщение пользователю, через функцию уведомлений
    */
    public void PushMessage<T>(T message)
    {
        var queue = this.GetMessageQueue<T>();
        queue.Enqueue(message);
    }


    public ConcurrentQueue<T> GetMessageQueue<T>()
    {
        if (this._authorization.IsSignin() == false)
        {
            return new ConcurrentQueue<T>();
        }
        var session = GetSession();
        if (session.ContainsKey(typeof(T).FullName) == false)
        {
            session[typeof(T).FullName] = new ConcurrentQueue<T>();
        }
        return (ConcurrentQueue<T>)
            session[typeof(T).FullName];

    }


    /// <summary>
    /// Запросить уведомления для данного пользователя
    /// </summary>
    /// <returns> Уведомления </returns>
    public T[] GetMessages<T>()
    {
        ConcurrentQueue<T> q = GetMessageQueue<T>();
        T[] messages = q.ToArray();
        q.Clear();         
        return messages;
    }

    public NotificationMessage[] GetNotifications()
    {
        return this.GetMessages<NotificationMessage>();
    }

    public void AddNotification(NotificationMessage message)
    {
        var queue = this.GetMessageQueue<NotificationMessage>();
        queue.Enqueue(message);
    }


}

