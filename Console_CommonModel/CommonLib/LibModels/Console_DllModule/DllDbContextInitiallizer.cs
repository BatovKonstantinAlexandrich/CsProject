using System;
using System.Collections.Generic;
using System.Linq;

namespace Console_CommonModel.CommonLib.LibModels.Console_DllModule
{
    [Label("Инициализация каталога DLL")]
    public class DllDbContextInitiallizer : DbContextInitiliazer<DllDbContext>
    {
        public override Dictionary<string, int> Init(DllDbContext context)
        {
            var result = new Dictionary<string, int>();
            result[nameof(DllAssemblyRecord)] = InitDemoAssemblies(context);
            return result;
        }

        [Label("Демо-записи сборок")]
        public int InitDemoAssemblies(DllDbContext context)
        {
            if (context.DllAssemblies.Any())
                return 0;

            context.DllAssemblies.Add(new DllAssemblyRecord
            {
                Name = "DemoCompiledService",
                DbContextTypeName = typeof(DeliveryDbContext).GetTypeName(),
                IncludedClassNames = "MessageSeedEnvironment,MessageSeedCompany",
                IncludedClassIds = "1,2",
                ExtensionInterfaceFullName = "WinFormsApp.IDemoExtension",
                ExtensionServiceFieldName = "_demoExtension",
                ExtensionGlueSource = "// demo glue",
                DllBytes = Array.Empty<byte>(),
                CreatedUtc = DateTime.UtcNow
            });

            return context.SaveChanges();
        }
    }
}
