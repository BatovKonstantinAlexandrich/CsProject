


[Label("События входа в систему")]
public class UserLogin : ActionTable<UserContext, UserAccount, UserContext>
{
    [InputTcpIpAddress]
    [NotNullNotEmpty]
    public string IpAddress { get; set; }


    public UserLogin( ) : base(null)
    {
    }
    public UserLogin(UserAccount login): base(login)
    {
    }
}
 