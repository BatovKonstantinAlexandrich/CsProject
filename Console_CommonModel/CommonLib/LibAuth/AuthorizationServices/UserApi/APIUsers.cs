using System.Collections.Generic;

public interface APIUsers: APIActiveCollection<UserContext>
{
    UserContext FindByToken(string token);
    UserContext FindByEmail(string email);


    IEnumerable<UserContext> FindByGroup(string group);   
    IEnumerable<UserContext> FindByRole(string role);
}
