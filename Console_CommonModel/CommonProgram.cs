using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Console_CommonModel.CommonLib.LibTesting;
using System.Data;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using System.Xml.Serialization;
using Console_CommonModel.CommonLib.LibModels.Console_InputModule.Console_FileModule; 
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using System.Reflection;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Org.BouncyCastle.Asn1.X509;


namespace Console_CommonModel
{
    /// <summary>
    /// Главный класс. Точка входа.
    /// </summary>
    public class CommonProgram
    {
        static void Main(string[] args) => RunTests<CommonProgram>(args);

        public static void Run<TProgram>(string[] args)
        {
            Load();

            ServiceFactory.Get().AddTypes(typeof(TProgram).Assembly);
            ServiceFactory.Get().AddTypes(typeof(CommonProgram).Assembly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TProgram"></typeparam>
        /// <param name="args"></param>
        public static void RunTests<TProgram>(string[] args)
        {
            var commonProgram = new CommonProgram();
            commonProgram.Trace();

            Load();

            ServiceFactory.Get().AddTypes(typeof(TProgram).Assembly);
            ServiceFactory.Get().AddTypes(typeof(CommonProgram).Assembly);

 

            TestGetIntegrationServices();


            var builder = new TableMetadataBuilder();
            builder.InvokeMethodWithInput<ColumnMetaData>(
                nameof(builder.AddIndicators),
                target => {
                    builder.Info(target);
                },
                error => {
                    builder.Info(error);
                }
            );



            //Console_ClassDataModelConsoleProgram.Run( );
            //InteractiveInvokeProgram.UserInvokeInteractive(new TableMetadataBuilder());
            //ConsoleProgram.RunInteractive<TableMetadataBuilder>();

           
            var tests = typeof(CommonProgram).Assembly.GetTestElements();
            foreach (var test in tests)
            {                
                var instance = test.New<TestingElement>();
            }

            bool interactive = args.Any(a =>
                string.Equals(a, "--interactive", StringComparison.OrdinalIgnoreCase)
                || string.Equals(a, "-i", StringComparison.OrdinalIgnoreCase));
            bool force = !args.Any(a =>
                string.Equals(a, "--no-force", StringComparison.OrdinalIgnoreCase));

            var unit = new CommonModelUnit();
            var report = unit.DoTest(false, true);
            report.PrintDocument();

            var reportPath = TestingReportFileWriter.Save(report);
            
            Console.WriteLine();
            Console.WriteLine(TestingReportFileWriter.BuildSummary(report));
            Console.WriteLine($"Полный отчёт: {reportPath}");
            Console.WriteLine($"Копия для анализа: {Path.Combine(TestingReportFileWriter.DefaultDirectory, "TestingReport_latest.txt")}");

            if (interactive)
                report.PrintDocument();
            else
                Console.WriteLine(report.ToDocument());
        }

        private static void TestGetIntegrationServices()
        {
            DbContextInitiliazer<ServiceDbContext>.CreateDatabase();
            var sdb = new ServiceDbContext();
            var services = sdb.GetIntegrationServices(typeof(ServiceResource));
            sdb.Info(services.Select(s => s.GetTypeName()));
        }

        private static void TestFasade(ServiceDbContext db)
        {
            var fasade = db.GetSqlServerWebApi().CreateFasade<FileCatalog>(typeof(FileCatalog));
            fasade.Create(new FileCatalog()
            {
                Name = "test"

            });
        }

        static void Install()
        {
        }

        public void Trace()
        {
            Console.WriteLine(new { 
                calling = Assembly.GetCallingAssembly().FullName,
                executing = Assembly.GetExecutingAssembly().FullName,
                entry = Assembly.GetEntryAssembly().FullName
            }.ToJson());
        }

        static void Load()
        {
            var program = new CommonProgram();
            program.InfoTime("Загрузка модулей", () => {
                Task.WaitAll(
                    new Task[] {
                        
                        ServiceFactory.Get().AddTypesAsync(typeof(System.Text.Json.Serialization.JsonIgnoreAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(XmlIgnoreAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(BindableAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(CategoryAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(DisplayAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(IndexAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(NamedSeriesArray).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(MessageProperty).Assembly),
                        ServiceFactory.Get().AddTypesAsync(typeof(ParameterAttribute).Assembly),
                        ServiceFactory.Get().AddTypesAsync(typeof(DisplayNameAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(NotMappedAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(CommonProgram).Assembly),
                        ServiceFactory.Get().AddTypesAsync(typeof(KeyAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(System.Object).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(string).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(JsonPropertyAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(Newtonsoft.Json.JsonArrayAttribute).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(System.Data.DataTable).Assembly, true),
                        ServiceFactory.Get().AddTypesAsync(typeof(System.Data.SqlClient.SqlClientPermission).Assembly, true)
                    }
                );
            });
            ServiceFactory.Get().BuildViewTypes();
        }
    }
}
