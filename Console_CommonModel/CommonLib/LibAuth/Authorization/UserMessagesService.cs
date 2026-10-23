using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;



/// <summary>
/// Реализация методов работы с сообщениями
/// </summary>
public class UserMessagesService<TDeliveryDbContext> : UserBusinessService<TDeliveryDbContext>, IUserMessagesService
    where TDeliveryDbContext : DeliveryDbContext
{
    private readonly SigninUser<TDeliveryDbContext> _signin;

    public UserMessagesService(SigninUser<TDeliveryDbContext> signin): base( signin)
    {
        _signin = signin;
    }


    [Label("Отправить сообщение")]
    public async Task<int> Send(UserMessage p, List<ProductImage> files)
    {
        await Task.CompletedTask;
        p.EnsureIsValide();
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            _context.Add(p);
            _context.SaveChanges();
            return 1;
        }
    }

    [Label("Входящие сообщения")]
    public List<UserMessage> GetInbox()
    {
        if (_signin.IsSignin() == false)
            throw new UnauthorizedAccessException("Метод доступен только авторизованным пользователям");
        int userId = _signin.Verify().Id;
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            return (from p in _context.UserMessages where p.ToUserId == userId select p).ToList();
        }
    }


    [Label("Исходящие сообщения")]
    public List<UserMessage> GetOutbox()
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            if (_signin.IsSignin() == false)
                throw new UnauthorizedAccessException("Метод доступен только авторизованным п6ользователям");
            int userId = _signin.Verify().Id;
            return (from p in _context.UserMessages where p.FromUserId == userId select p).ToList();
        }
    }

    public Dictionary<string, int> GetUsers()
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            var results = new Dictionary<string, int>(_context.UserContexts.Include(u => u.Person).Select(u => new KeyValuePair<string, int>("#" + u.Id + " " + u.Person.GetFullName(), u.Id)));
            return results;
        }
    }

    [Label("Непрочитанные сообщения")]
    public int GetInboxNotReaded()
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            if (_signin.IsSignin() == false)
                throw new UnauthorizedAccessException("Метод доступен только авторизованным п6ользователям");
            int userId = _signin.Verify().Id;
            return (from p in _context.UserMessages where p.FromUserId == userId && p.Readed == false select p).Count();
        }
    }
}