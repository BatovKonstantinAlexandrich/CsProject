using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RootForms.Core.ViewServices
{
    public class UserModels: IUserModelsService
    {
        public static int Counter = StartupApplication.AddScopedConfiguration<IUserModelsService,UserModels>();

    }
}
