public interface ISigninUser<TDbContextUser> where TDbContextUser : DeliveryDbContext
{
    UserContext GetBy(string username);
    UserContext GetBy(UserAccount userAccount);
    TDbContextUser GetDbContext();
    T GetFromSession<T>(string key);
    T GetFromSession<T>(string key, T defaultValue);
    bool IsSignin();
    void PutIntoSession(string key, object item);
    void SetDbContext(TDbContextUser db);
    MethodResult<UserContext> Signin([InputEngWord] string user, [InputPassword] string password);
    MethodResult<UserContext> Signin(UserAccount account);
    bool Signout();
    bool Signout(string key);
    bool Validate(string key);
    UserContext Verify();
}