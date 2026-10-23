
/// <summary>
/// Методы работы с группами пользователей
/// </summary>
public interface IUserGroupsService
{
    UserGroup GetGroup(int id);
    List<UserGroup> GetGroups();
    List<UserGroup> GetUserGroups();
    List<UserPerson> GetPersons(int groupId);


    int AddUserToGroup(int groupId);
    List<UserGroupMessage> GetGroupMessages(int groupId, int page, int size);
    void PublishIntoGroup(int groupId, UserGroupMessage message);
    void PublishIntoGroup(int groupId, UserMessage message);

    
    bool IsUserInGroup(int groupId);
    void JoinToGroup(int groupId);
    void LeaveGroup(int groupId);
     
}
 