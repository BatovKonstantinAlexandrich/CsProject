using Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization;

namespace Console_CommonModel.CommonLib.LibAuth.AuthServices
{
    public class AuthServicesUnit : TestingUnit
    {
        public AuthServicesUnit(IServiceProvider parent) : base(parent)
        {
            this.Append(new BusinessProcessBuilderTest(parent));
            this.Append(new BusinessProcessValidatorTest(parent));
            this.Append(new UserBusinessServiceTests(parent));
            this.Append(new UserMessagesTest(parent));
            
        }
    }
}
