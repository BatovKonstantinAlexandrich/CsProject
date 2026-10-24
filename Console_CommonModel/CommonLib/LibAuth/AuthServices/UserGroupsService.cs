using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Console_CommonModel.CommonLib.LibAuth.AppAuth;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;

/// <summary>
/// Управление группами пользователя
/// </summary>
public class UserGroupsService<TDeliveryDbContext> : IUserGroupsService
    where TDeliveryDbContext: DeliveryDbContext
{
     
    private TDeliveryDbContext _context;
    private SigninUser<TDeliveryDbContext> _signin;

    public UserGroupsService(TDeliveryDbContext context, SigninUser<TDeliveryDbContext> signin)
    {
        _context = context;
        _signin = signin;
    }

    public int AddUserToGroup([InputSearch<DbContextUser, UserGroup>(nameof(UserGroup)+",Name")] int groupId)
    {

        if (!this._signin.IsSignin())
            throw new AuthorizationException();
        int userId = this._signin.Verify().Id;
        var user = _context.UserContexts.Find(userId);
        if (user is null)
            throw new ArgumentException("userId", $"Не найден пользователь с ид {userId}");
        var group = _context.UserGroups.Find(groupId);
        if (group is null)
            throw new ArgumentException("groupId", $"Не найдена группа пользователей с ид {groupId}");
        if (_context.UserGroupsUserGroup.Any(ug => ug.UserId == userId && ug.GroupId == groupId))
        {
            this.Warn($"userId={userId},groupId={groupId}", "Пользователь уже находится в этой группе");
        }
        if(!_context.UserGroupsUserGroup.Any(ug => ug.UserId==userId && ug.GroupId==groupId))
        {
            UserGroupBinding target = new UserGroupBinding()
            {
                GroupId = groupId,
                UserId = userId
            };
            _context.UserGroupsUserGroup.Add(target);
            return _context.SaveChanges();
        }
        return 0;
    }

    public List<UserGroupMessage> GetGroupMessages(int groupId, int page, int size)
    {
        return _context.UserGroupMessages.Where(m => m.GroupId == groupId).OrderByDescending(m => m.Created).Skip((page - 1) * size).Take(size).ToList();
    }


    
    public string GetUsername(int userId)
    {
        return (from p in _context.UserContexts.Include(p => p.Person) where p.Id == userId select p.Person.FirstName + " " + p.Person.LastName + " " + p.Person.SurName).SingleOrDefault();
    }

    public UserGroup GetGroup(int id)
    {
        var group = _context.UserGroups.Find(id);
        group.People = this.GetPersons(id);

        return group;
    }

    public List<UserGroup> GetGroups()
    {
        return _context.UserGroups.ToList();
    }

    public List<UserGroup> GetUserGroups( )
    {
        int userId = this._signin.Verify().Id;
        return _context.UserGroups.Where(g => (from p in _context.UserGroupsUserGroup where p.UserId == userId select p.GroupId).Contains(g.Id)).ToList();
    }

    public List<UserPerson> GetPersons(int groupId)
    {
        return _context.UserContexts.Include(ctx => ctx.Person).Where(ctx => _context.UserGroups.Include(g => g.UserGroups).First(g => g.Id == groupId).UserGroups.Select(gr => gr.UserId).Contains(ctx.Id)).Select(ctx => ctx.Person).ToList();
    }

    public bool IsUserInGroup(int groupId )
    {
        int userId = this._signin.Verify().Id;
        return (from p in _context.UserGroupsUserGroup where p.UserId == userId && groupId == p.GroupId select p).SingleOrDefault() != null;
    }

    public void JoinToGroups(params string[] groups)
    {
        foreach (string name in groups) 
        {
            var group = this._context.CheckRegistration<UserGroup>(name);
            this.JoinToGroup(group.Id);
        }
    }

    public void JoinToGroup(int groupId )
    {
        int userId = this._signin.Verify().Id;
        _context.UserGroupsUserGroup.Add(new UserGroupBinding()
        {
            GroupId = groupId,
            UserId = userId
        });
        _context.SaveChanges();
        //_notifications.Info($"Вы добавлены в группу: {GetGroup(groupId).Name }");
        PublishIntoGroup( groupId, new UserMessage()
        {
            Created = DateTime.Now,
            Subject = "Состав группы",
            Text = $"Пользователь {GetUsername(userId)} вступил в группу"
        });
    }

    public void LeaveGroup(int groupId )
    {
        int userId = this._signin.Verify().Id;
        var connection = (from p in _context.UserGroupsUserGroup where p.UserId == userId && groupId == p.GroupId select p).SingleOrDefault();
        if (connection == null)
            throw new ArgumentException("groupId,userId");
        _context.UserGroupsUserGroup.Remove(connection);
        _context.SaveChanges();
        //_notifications.Info($"Вы покинули группу: {GetGroup(groupId).Name }");

        PublishIntoGroup(groupId, new UserMessage()
        {
            Created = DateTime.Now,
            Subject = "Состав группы",
            Text = $"Пользователь {GetUsername(userId)} покинул группу"
        });
    }

    public void PublishIntoGroup( int groupId, UserMessage message)
    {
        int userId = this._signin.Verify().Id;
        UserGroupMessage newRecord = JsonConvert.DeserializeObject<UserGroupMessage>(JsonConvert.SerializeObject(message));
        newRecord.GroupId = groupId;
        _context.UserGroupMessages.Add(newRecord);
        _context.SaveChanges();
        //_notifications.Info($"Вы успешно опубликовали сообщение в группе {GetGroup(groupId).Name}");
    }

    public void PublishIntoGroup( int groupId, UserGroupMessage message)
    {
        int userId = this._signin.Verify().Id;
        message.GroupId = groupId;
        _context.UserGroupMessages.Add(message);
        _context.SaveChanges();
        //_notifications.Info($"Вы успешно опубликовали сообщение в группе {GetGroup(groupId).Name}");
    }

    public void CheckGroupsHasRegistered(params string[] names )
    {
        foreach (string name in names)
        {
            if(this._context.UserGroups.Any(group => group.Name == name))
            {
                this._context.UserGroups.Add(new()
                {
                    Name = name,
                    Label = name,
                    Description = name
                });
                this._context.SaveChanges();
            }
        }
        
    }

    internal void SetDbContext(TDeliveryDbContext context)
    {
        this._context = context;
    }
}
 