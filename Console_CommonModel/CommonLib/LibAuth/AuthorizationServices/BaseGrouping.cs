using Console_CommonModel.CommonLib.LibData.AbstractionModel;

public abstract class BaseGrouping<TItem, TParent, TGroup>: BaseEntity
    where TGroup: BaseEntity 
    where TParent: NamedObject
{
    public abstract MethodResult<TGroup> RemoveFromGroup(TItem item, TParent parent);
    public abstract MethodResult<TGroup> AddToGroup(TItem item, TParent parent);    
    public abstract MethodResult<TGroup> SendToGroup(TItem item, UserGroupMessage Message,  TParent parent);
    public abstract MethodResult<UserGroupBinding> AddToGroup(UserContext item, string name);


    public abstract IEnumerable<TParent> GetSelectedOptions(TItem item);
    public abstract IEnumerable<TParent> GetAvailableOptions(TItem item);

    public abstract string[] GetGroups();
    public abstract string[] GetGroupsForItem(TItem item);



    public ListView SelectGroups(TItem item)
    {
        var all = GetGroups();
        var current = GetGroupsForItem(item);
        var available = all.Where(item => !current.Contains(item));
        var selected = GetSelectedOptions(item);
        var groups = GetAvailableOptions(item);
        var list = new ListView();
        list.Options.AddRange(groups.Select(g => new Option() { 
            Item = g,
            Value = g.Name
        }));
        list.Options.AddRange(selected.Select(g => new Option()
        {
            Item = g,
            Value = g.Name
        }));        
        return list;
    }


    public abstract bool RemoveFromGroup(TItem item, string name);
}