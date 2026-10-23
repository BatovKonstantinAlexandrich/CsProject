namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectorService
{

    [Label("Проверка соединений ADO")]
    public class ADODbConnectorsTest : TestingElement
    {
        public ADODbConnectorsTest() : this(AppProviderService.GetInstance())
        {
        }

        public ADODbConnectorsTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            SqlServerDbConnector_Test();
            //MySqlDbConnector_Test();
            PostgresDbConnector_Test();
        }

        public static void Run()
        {
            ADODbConnectorsTest test = new ADODbConnectorsTest();
            test.SqlServerDbConnector_Test();
            test.MySqlDbConnector_Test();
            test.PostgresDbConnector_Test();
        }

        private void PostgresDbConnector_Test()
        {
            PostgresDbConnector connector = new PostgresDbConnector();
            using (var connection = connector.CreateAndOpenConnection())
            {
                connector.Info(connection.ConnectionString);
            }
        }

        private void MySqlDbConnector_Test()
        {
            MySqlDbConnector connector = new MySqlDbConnector();
            using (var connection = connector.CreateAndOpenConnection())
            {
                connector.Info(connection.ConnectionString);
            }
        }

        private void SqlServerDbConnector_Test()
        {
            SqlServerDbConnector connector = new SqlServerDbConnector();
            using (var connection = connector.CreateAndOpenConnection())
            {
                connector.Info(connection.ConnectionString);
                Messages.Add("Соединение с SQlServer установлено успешно: " + connection.ConnectionString);
            }
        }


    }
}
