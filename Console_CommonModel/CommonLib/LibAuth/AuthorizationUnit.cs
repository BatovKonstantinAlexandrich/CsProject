using Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization;

[Label("Модуль тестирования авторизации")]
public class AuthorizationUnit : TestingUnit
{
    public AuthorizationUnit() : this(AppProviderService.GetInstance()) { }

    public AuthorizationUnit(IServiceProvider sp) : base(sp)
    {
        Append(new AuthorizationUsersTest(sp));
        Append(new AuthorizationServiceTest(sp));
        Append(new AuthorizationCollectionTest(sp));
        Append(new UserMessagesTest(sp));
        Append(new UserBusinessServiceTests(sp));
    }
}
