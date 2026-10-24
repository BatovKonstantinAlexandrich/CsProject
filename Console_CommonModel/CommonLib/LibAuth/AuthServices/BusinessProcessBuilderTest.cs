using Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BusinessProcessBuilderTest: TestingService<BusinessProcessBuilder<DeliveryDbContext>>
{
    public BusinessProcessBuilderTest(IServiceProvider sp) : base(sp)
    {
    }

    public override bool OnServiceTest(BusinessProcessBuilder<DeliveryDbContext> service)
    {
        return false;
    }
}
