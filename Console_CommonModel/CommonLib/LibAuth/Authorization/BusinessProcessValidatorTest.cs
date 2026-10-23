using Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization;

public class BusinessProcessValidatorTest : TestingService<BusinessProcessValidator<DeliveryDbContext>>
{
    public BusinessProcessValidatorTest(IServiceProvider sp) : base(sp)
    {
    }

    public override bool OnServiceTest(BusinessProcessValidator<DeliveryDbContext> service)
    {
        return false;
    }
}
 