namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{
    public class OdbcTest : TestingElement
    {
        public override void OnTest()
        {
            var manager = new OdbcDriverManager();
            var odbc = manager.ConnectDatasource("DeliveryDbContext");
            Console.WriteLine(odbc.GetDatabaseMetadata().ToJsonOnScreen());
            Console.WriteLine(manager.GetOdbcDatasources().ToJsonOnScreen());
            Console.WriteLine(manager.GetOdbcDatasources().ToJsonOnScreen());

            //System.Console.WriteLine(manager.RegOdbcDataSource("Delivery", new SqlServerWebApi(), "SQL Server"));

            Console.WriteLine(manager.GetOdbcDrivers().ToJsonOnScreen());
            Console.WriteLine(manager.GetOdbcDatasourceNames().ToJsonOnScreen());
        }
    }
}
