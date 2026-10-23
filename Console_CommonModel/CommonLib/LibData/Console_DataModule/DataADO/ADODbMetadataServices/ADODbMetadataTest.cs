using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;



namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices
{

    [Label("Структуры данных ADO")]
    public class ADODbMetadataTest : TestingElement
    {

        public ADODbMetadataTest(IServiceProvider provider) : base(provider)
        {
        }

        public ADODbMetadataTest() : this(AppProviderService.GetSingletonInstance())
        {
        }

        public override void OnTest()
        {
            SqlServerDatabase_GetTableMetadata_Test();
            SqlServerDatabase_GetProcedureMetadata_Test();
            PostgresDatabase_GetTableMetadata_Test();
            PostgresDatabase_GetProcedureMetadata_Test();
        }
        public static void Run()
        {
            ADODbMetadataTest test = new ADODbMetadataTest();
            test.DoTest();
        }

        public void SqlServerDatabase_GetTableMetadata_Test()
        {
            using (var database = new SqlServerDbMetadata())
            {
                database.GetTablesMetadata().ToJsonOnScreen().WriteToConsole();
            }
            Messages.Add("Сведения о структуре таблиц получены успешно");
        }

        public void SqlServerDatabase_GetProcedureMetadata_Test()
        {
            using (var database = new SqlServerDbMetadata())
            {
                foreach (var KeyValuePair in database.GetProceduresMetadata("dbo"))
                {
                    KeyValuePair.Value.ToJsonOnScreen().WriteToConsole();
                }
            }

            Messages.Add("Сведения о хранимых процедурах получены успешно");
        }

        public void PostgresDatabase_GetTableMetadata_Test()
        {
            using (var database = new PostgresDbMetadata())
            {
                database.GetTablesMetadata().ToJsonOnScreen().WriteToConsole();
            }
            Messages.Add("Сведения о структуре таблиц получены успешно");
        }

        public void PostgresDatabase_GetProcedureMetadata_Test()
        {
            using (var database = new PostgresDbMetadata())
            {
                foreach (var KeyValuePair in database.GetProceduresMetadata("public"))
                {
                    KeyValuePair.Value.ToJsonOnScreen().WriteToConsole();
                }
            }

            Messages.Add("Сведения о хранимых процедурах получены успешно");
        }
    }
}
