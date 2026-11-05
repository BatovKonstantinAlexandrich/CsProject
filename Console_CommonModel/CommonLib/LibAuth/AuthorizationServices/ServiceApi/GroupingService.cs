using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationServices.ServiceApi
{
     
    public class GroupingService : BaseGrouping<UserContext, UserGroup, UserGroupBinding>
    {
        private readonly DbContextUser _model;

        public GroupingService()
        {
            _model = new();
        }

        public GroupingService(DbContextUser model)
        {
            _model = model;
        }

        public override MethodResult<UserGroupBinding> AddToGroup(UserContext item, UserGroup parent)
        {
            try
            {
                UserGroupBinding res;
                _model.Add(res = new UserGroupBinding()
                {
                    Group = parent,
                    User = item
                });
                _model.SaveChanges();
                return MethodResult<UserGroupBinding>.OnResult(res);
            }
            catch (DbUpdateException ex)
            {
                return MethodResult<UserGroupBinding>.OnError(ex);
            }
        }

        public override MethodResult<UserGroupBinding> AddToGroup(UserContext item, string name)
        {
            try
            {
                UserGroupBinding res;
                _model.Add(res = new UserGroupBinding()
                {
                    Group = _model.UserGroups.FirstOrDefault(g => g.Name.ToUpper() == name.ToUpper()),
                    User = item
                });
                _model.SaveChanges();
                return MethodResult<UserGroupBinding>.OnResult(res);
            }
            catch (DbUpdateException ex)
            {
                return MethodResult<UserGroupBinding>.OnError(ex);
            }
        }

        public override IEnumerable<UserGroup> GetAvailableOptions(UserContext item)
            => _model.UserGroups.Where(g => _model.UserGroupsUserGroup.Select(ug => ug.GroupId).Contains(g.Id) == false);

        public override string[] GetGroups()
            => _model.UserGroups.Select(g => g.Name).ToArray();

        public override string[] GetGroupsForItem(UserContext item)
            => _model.UserGroups.Where(g => _model.UserGroupsUserGroup.Select(ug => ug.GroupId).Contains(g.Id)).Select(g => g.Name).ToArray();

        public override IEnumerable<UserGroup> GetSelectedOptions(UserContext item)
            => _model.UserGroups.Where(g => _model.UserGroupsUserGroup.Select(ug => ug.GroupId).Contains(g.Id) == true);


        public override MethodResult<UserGroupBinding> RemoveFromGroup(UserContext item, UserGroup parent)
        {
            try
            {
                var ug = _model.UserGroupsUserGroup.FirstOrDefault(ug => ug.UserId == item.Id && ug.GroupId == parent.Id);
                if (ug == null)
                {
                    return MethodResult<UserGroupBinding>.OnError(new ArgumentException());
                }
                _model.UserGroupsUserGroup.Remove(ug);
                _model.SaveChanges();
                return MethodResult<UserGroupBinding>.OnResult(ug);

            }
            catch (Exception ex)
            {
                return MethodResult<UserGroupBinding>.OnError(ex);
            }
        }

        public override bool RemoveFromGroup(UserContext item, string name)
        {

            var group = _model.UserGroups.FirstOrDefault(g => g.Name == name);
            var ug = _model.UserGroupsUserGroup.FirstOrDefault(ug => ug.UserId == item.Id && ug.GroupId == group.Id);
            if (ug == null)
            {
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
                _model.Add(new UserGroupMessage()
                {
                    FromUser = item,
                    Group = parent
                });
                _model.SaveChanges();
                return MethodResult<UserGroupBinding>.OnResult(null);

            }
            catch (Exception ex)
            {
                return MethodResult<UserGroupBinding>.OnError(ex);
            }
        }
    }
}