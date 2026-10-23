using Console_CommonModel.CommonLib.LibModels.XmlAppModel;

using System.Collections.Generic;
using System.Linq;


namespace Console_CommonModel.CommonLib.LibModels.Console_XmlModule
{
    [Label("Инициализация XML-приложений")]
    public class XmlAppDbContextInitiallizer : DbContextInitiliazer<XmlAppDbContext>
    {
        public override Dictionary<string, int> Init(XmlAppDbContext context)
        {
            return new Dictionary<string, int>
            {
                [nameof(XmlAppStartup)] = InitDemoStartups(context)
            };
        }

        [Label("Демо XML startup")]
        public int InitDemoStartups(XmlAppDbContext context)
        {
            if (context.XmlAppStartups.Any())
                return 0;

            context.XmlAppStartups.Add(new XmlAppStartup
            {
                Name = "DemoApp",
                Label = "Демо XML-приложение",
                Description = "Минимальный startup для тестирования XmlAppDbContext",
                Xml =
                    """
                    <application name="DemoApp">
                      <startup>
                        <form name="MainForm" />
                      </startup>
                    </application>
                    """
            });

            context.XmlAppStartups.Add(new XmlAppStartup
            {
                Name = "PayGwShell",
                Label = "NgAppPayGwManager (embedded)",
                Description = "Оболочка для встраивания PayGwManager",
                Xml =
                    """
                    <application name="PayGwShell">
                      <iframe url="http://localhost:4200/" />
                    </application>
                    """
            });

            return context.SaveChanges();
        }
    }
}
