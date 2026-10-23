using ApplicationDb.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


public class UserMessagesService : BaseService, IUserMessagesService
{
    private readonly IAuthorizationModel _context;

    public UserMessagesService(IAuthorizationModel context)
    {
        _context = context;
    }

    public List<UserMessage> GetInbox(int userId)
    {
        return (from p in _context.Messages.Include(m => m.ToUser) where p.ToUser.ID == userId select p).ToList();
    }

    public List<UserMessage> GetOutbox(int userId)
    {
        return (from p in _context.Messages.Include(m => m.FromUser) where p.FromUser.ID == userId select p).ToList();
    }
}

