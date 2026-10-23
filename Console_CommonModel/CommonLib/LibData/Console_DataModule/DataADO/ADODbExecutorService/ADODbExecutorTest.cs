using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbExecutorService;

using System.Data;

[Label("Выполнение операций ADO")]
public class ADODbExecutorTest: TestingElement
{
    public ADODbExecutorTest() : this(AppProviderService.GetInstance()) { }

    public ADODbExecutorTest(IServiceProvider provider) : base(provider)
    {
    }

    public static void Run()
    {
        ADODbExecutorTest test = new ADODbExecutorTest();
        test.SqlServerExecutorTest();
        test.SqlServerExecuteProcedureTest();
        test.MySqlExecutorTest();
        test.MySqlExecutorProcedureTest();
        test.PostgresExecutorTest();
        test.PostgresExecuteProcedureTest();
    }

    private void PostgresExecuteProcedureTest()
    {
        string procedureName = "public.getProduct()";
        using (var executor = new PostgresExecutor())
        {

            
            
        }
    }

    private void PostgresExecutorTest()
    {
        using (var executor = new PostgresExecutor())
        {

        }
    }

    private void MySqlExecutorProcedureTest()
    {
        using (var executor = new MySqlExecutor())
        {

        }
    }

    private void MySqlExecutorTest()
    {
        using (var executor = new MySqlExecutor())
        {

        }
    }

    public void SqlServerExecutorTest()
    {
        using (SqlServerExecutor executor = new SqlServerExecutor())
        {
            executor.PrepareQuery(@"
                DECLARE @COUNT INT
                SET @COUNT=(SELECT Count(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='History')
                IF( @COUNT > 0 )
                BEGIN
	                DROP TABLE History
                END
            ");
            executor.PrepareQuery("CREATE TABLE History( Id INT PRIMARY KEY Identity, CREATED DateTime NOT NULL DEFAULT(GetDate()), COMMAND nvarchar(512) NOT NULL )");
            executor.PrepareQuery("INSERT INTO History( COMMAND ) values ('CREATE DATABASE WORLD')");

            DataTable history = executor.ExecuteQuery("SELECT * FROM History");
            executor.Info(GetTextData(history).ToJsonOnScreen());
            executor.PrepareQuery("DROP TABLE  IF EXISTS History");
            Messages.Add("Операции TSQL выполняются успешно");
        }
    }

    public void SqlServerExecuteProcedureTest()
    {
        using (SqlServerExecutor executor = new SqlServerExecutor())
        {
            executor.TryPrepareQuery(@"DROP PROCEDURE [dbo].[ON_STARTED]");
            executor.TryPrepareQuery(@"
                -- =============================================
                -- Author:		<Author,,Name>
                -- Create date: <Create Date,,>
                -- Description:	<Description,,>
                -- =============================================
                CREATE PROCEDURE [dbo].[ON_STARTED]
	
	                @URL nvarchar(max),
	                @KEY nvarchar(max) OUTPUT
		
                AS
                BEGIN
	                -- SET NOCOUNT ON added to prevent extra result sets from
	                -- interfering with SELECT statements.
	                SET NOCOUNT ON;

                    -- Insert statements for procedure here
	                SET @KEY = @URL
	
                END
            ");
            DataTable Result = executor.ExecuteQuery(@"
                DECLARE @KEY nvarchar(max)
                EXEC [dbo].[ON_STARTED] 'http://localhost:8080',@KEY OUTPUT
                SELECT @KEY AS 'KEY'
            ");
            GetTextData(Result).ToJsonOnScreen().WriteToConsole();
        }
    }


    /// <summary>
    /// Выбираем текстовые данные из результирующего набора
    /// </summary>
    private IEnumerable<IDictionary<string, string>> GetTextData(DataTable dataTable)
    {
        var result = new List<IDictionary<string, string>>();
        foreach (DataRow row in dataTable.Rows)
        {
            IDictionary<string, string> data = new Dictionary<string, string>();
            foreach (DataColumn column in dataTable.Columns)
            {
                object value = row[column.ColumnName];
                data[column.ColumnName] = value == null ? "NULL" : value.ToString();
            }
            result.Add(data);
        }
        return result.ToArray();
    }

    public override void OnTest()
    {
        this.SqlServerExecutorTest();
        this.SqlServerExecuteProcedureTest();
        this.MySqlExecutorTest();
        this.MySqlExecutorProcedureTest();
        this.PostgresExecutorTest();
        this.PostgresExecuteProcedureTest();
    }
}
