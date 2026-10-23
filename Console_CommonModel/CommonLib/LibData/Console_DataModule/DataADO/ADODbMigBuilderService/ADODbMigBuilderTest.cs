using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService
{

    [Label("Создание миграций ADO")]
    public class ADODbMigBuilderTest : TestingElement
    {
        public ADODbMigBuilderTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            TestMigrationBuilder(new SqlServerMigBuilder());
            TestMigrationBuilder(new PostgresMigBuilder());
        }

        private static void TestMigrationBuilder(IDbMigBuilder builder)
        {
            Type[] Entities = new Type[]
            {
                typeof(AccounModelTest),
                typeof(PersonModelTest),
                typeof(AppUserModelTest)
            };
            var messages = new List<string>();
            foreach (var Entity in Entities)
            {
                builder.AddEntityType(Entity);
                foreach (var mig in builder.DropAndCreate())
                {
                    string message = mig.Up
                            .ReplaceAll("  ", " ")
                            .ReplaceAll("  ", " ");
                    builder.Info(message.Split("/n"));
                    messages.AddRange(new List<string>(message.Split("/n")));
                }
            }
        }

        class AccounModelTest
        {
            public int Id { get; set; }
            public string Email { get; set; }
        }

        class PersonModelTest
        {
            public int Id { get; set; }
            public string Email { get; set; }
        }

        class AppUserModelTest
        {
            public int Id { get; set; }
            public int AccountId { get; set; }
            public int PersonId { get; set; }
            public AccounModelTest Account { get; set; }
            public PersonModelTest Person { get; set; }
        }



        public static List<string> Run()
        {
            SqlServerMigBuilder builder = new SqlServerMigBuilder();
            Type[] Entities = new Type[]
            {
                typeof(AccounModelTest),
                typeof(PersonModelTest),
                typeof(AppUserModelTest)
            };
            var messages = new List<string>();
            foreach (var Entity in Entities)
            {
                builder.AddEntityType(Entity);
                foreach (var mig in builder.DropAndCreate())
                {
                    string message = mig.Up
                            .ReplaceAll("  ", " ")
                            .ReplaceAll("  ", " ");
                    builder.Info(message.Split("/n"));
                    messages.AddRange(new List<string>(message.Split("/n")));
                }
            }
            return messages;
        }
    }
}
