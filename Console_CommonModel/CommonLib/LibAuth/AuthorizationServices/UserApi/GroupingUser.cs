using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Реализовация управления группами пользователя
/// </summary>
public class GroupingUser : BaseGrouping<UserContext, UserGroup, UserGroupBinding>
{
    private readonly DbContextUser _model;

    public GroupingUser(DbContextUser model)
    {
        this._model = model;
    }

    public GroupingUser()
    {
        this._model = new DbContextUser();
    }

    public int GetUsersCountInGroup(int groupId)
    {
        return _model.UserGroupsUserGroup.Where(p => p.GroupId == groupId).Count();
    }
    public override MethodResult<UserGroupBinding> AddToGroup(UserContext item, UserGroup parent)
    {
        try
        {
            UserGroupBinding res;
            this._model.Add(res=new UserGroupBinding(){
                Group = parent,
                User = item
            });
            this._model.SaveChanges();
            return MethodResult<UserGroupBinding>.OnResult(res);
        }
        catch(DbUpdateException ex)
        {
            return MethodResult<UserGroupBinding>.OnError(ex);
        }
    }

  
    public override MethodResult<UserGroupBinding> AddToGroup(UserContext item, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name", "Содержит ссылку на null");
            var group = this._model.UserGroups.FirstOrDefault(g => g.Name.ToUpper() == name.ToUpper());
            if(group is null)
            {
                _model.UserGroups.Add(new()
                {
                    Name = name,
                    Description = name
                });
                _model.SaveChanges();
            };
            group = this._model.UserGroups.FirstOrDefault(g => g.Name.ToUpper() == name.ToUpper());
            UserGroupBinding res;
            this._model.Add(res=new UserGroupBinding(){
                Group = group,
                User = item
            });
            this._model.SaveChanges();
            return MethodResult<UserGroupBinding>.OnResult(res);
        }
        catch(DbUpdateException ex)
        {
            return MethodResult<UserGroupBinding>.OnError(ex);
        }
    }

 
    public override IEnumerable<UserGroup> GetAvailableOptions(UserContext item)    
        => _model.UserGroups.Where( g => (_model.UserGroupsUserGroup.Select(ug => ug.GroupId)).Contains(g.Id) == false);


    public override string[] GetGroups()
        => _model.UserGroups.Select(g => g.Name).ToArray();
    public IEnumerable<string> GetRoles() => _model.BusinessResources.Select(g => g.Code);
    public IEnumerable<string> GetRoles(int userId)
    {
        var user = _model.UserContexts.Include(u => u.UsersResources).First(u => u.Id == userId);
        return _model.BusinessResources.Where(r => _model.UsersResources.Where(bf => bf.UserId == user.Id).Select(bf => bf.RoleId).Contains(r.Id)).Select(r => r.Code);
    }
    public int AddRole(int userId,string role)
    {
        var u = _model.UserContexts.First(r => r.Id == userId);
        var r = _model.BusinessResources.First(r => r.Code == role);
        _model.UsersResources.Add(new()
        {
            UserId = userId,
            RoleId = r.Id
        });
        return _model.SaveChanges();
    }
    public int RemoveRole(int userId, string role)
    {
        var u = _model.UserContexts.First(r => r.Id == userId);
        var r = _model.BusinessResources.First(r => r.Code == role);
        _model.UsersResources.Remove(_model.UsersResources.First(ur =>
        
            ur.UserId == userId &&
            ur.RoleId == r.Id
        ));
        _model.SaveChanges();
        return _model.SaveChanges();
    }


    public override string[] GetGroupsForItem(UserContext item)
        => _model.UserGroups.Where( g => (_model.UserGroupsUserGroup.Select(ug => ug.GroupId)).Contains(g.Id)).Select(g => g.Name).ToArray();


     
    public override IEnumerable<UserGroup> GetSelectedOptions(UserContext item)
        => _model.UserGroups.Where( g => (_model.UserGroupsUserGroup.Select(ug => ug.GroupId)).Contains(g.Id) == true);


  
    public override MethodResult<UserGroupBinding> RemoveFromGroup(UserContext item, UserGroup parent)
    {
        try
        {
            var ug = _model.UserGroupsUserGroup.FirstOrDefault( ug => ug.UserId==item.Id && ug.GroupId == parent.Id);
            if(ug == null){
                return MethodResult<UserGroupBinding>.OnError(new ArgumentException());
            }
            _model.UserGroupsUserGroup.Remove(ug);
            _model.SaveChanges();
            return MethodResult<UserGroupBinding>.OnResult(ug);

        }catch(Exception ex){
            return MethodResult<UserGroupBinding>.OnError(ex);
        }
    }

   
    public override bool RemoveFromGroup(UserContext item, string name)
    {
    
        var group = _model.UserGroups.FirstOrDefault( g => g.Name==name);
        var ug = _model.UserGroupsUserGroup.FirstOrDefault( ug => ug.UserId==item.Id && ug.GroupId == group.Id);
        if(ug == null){
            return false;
        }
        _model.UserGroupsUserGroup.Remove(ug);
        _model.SaveChanges();
        return true;
 
    }

    
    public override MethodResult<UserGroupBinding> SendToGroup(UserContext item, UserGroupMessage Message, UserGroup parent)
    {        
        try
        {
            _model.Add(new UserGroupMessage(){
                FromUser = item,
                Group = parent
            });
            _model.SaveChanges();
            return MethodResult<UserGroupBinding>.OnResult(null);

        }catch(Exception ex){
            return MethodResult<UserGroupBinding>.OnError(ex);
        }
    }
}