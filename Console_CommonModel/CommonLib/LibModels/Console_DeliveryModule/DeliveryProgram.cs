using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryTests;
using pickpoint_delivery_service;

using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.AppBusiness;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryAppServices;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;

using Console_CommonModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule
{

    /// <summary>
    /// Программа доставки системы интернет продажи товаров
    /// </summary>
    public class DeliveryProgram
    {
        /// <summary>
        /// Консольное меню (без WinForms), для ProgAgent / LibModelsConsoleEngine.
        /// </summary>
        public static void Run()
        {
            var args = Array.Empty<string>();
            while (true)
            {
                var choice = InputConsole.SingleSelect(
                    "DeliveryProgram",
                    new List<string>
                    {
                        "Инициализировать DeliveryDbContext",
                        "Запустить интеграционные тесты (консоль)",
                        "Вывести метаданные хранимых процедур",
                        "UserBusinessFunctionsTest",
                        "DataModelServiceTest",
                        "Назад"
                    },
                    ref args);

                if (choice == "Назад")
                    return;

                try
                {
                    switch (choice)
                    {
                        case "Инициализировать DeliveryDbContext":
                            InitDeliveryDbContext();
                            InputConsole.Write("Инициализация завершена.");
                            break;
                        case "Запустить интеграционные тесты (консоль)":
                            RunTest();
                            break;
                        case "Вывести метаданные хранимых процедур":
                            ListProceduresMetadata();
                            break;
                        case "UserBusinessFunctionsTest":
                            new UserBusinessFunctionsTest().DoTest(false).ToDocument().WriteToConsole();
                            break;
                        case "DataModelServiceTest":
                            new DataModelServiceTest().DoTest(false).ToDocument().WriteToConsole();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    InputConsole.Error(ex.InnerException?.Message ?? ex.Message);
                }

                InputConsole.ConfirmContinue();
            }
        }

        private static void ListProceduresMetadata()
        {
            using var db = new DeliveryDbContext();
            db.Database.SetCommandTimeout(15);
            foreach (var kv in db.GetProceduresMetadata())
            {
                kv.Value.EnsureIsValide();
                db.Info(kv.Value);
            }
        }

        /// <summary>
        /// Выполнение тестов
        /// </summary>
        private static void RunTest()
        {
            var provider = AppProviderService.GetInstance();
            var unit = new TestingUnit(provider);

            unit.Append(new UserBusinessFunctionsTest(provider));
            unit.Append(new RecruitmentCompanyBusinessProcessUnit(provider));

            unit.Append(new CustomerOrderCheckoutUnit(provider));
            unit.Append(new HolderServicesTestUnit(provider));
            unit.Append(new ReservationOrderCheckoutUnit(provider));
            unit.Append(new TransportOrderDeliveryUnit(provider));
            Console.WriteLine(unit.DoTest(false).ToDocument());
        }


        /// <summary>
        /// Загрузка минимального набора данных
        /// </summary>
        public static void InitDeliveryDbContext()
        {
            using (var db = new DeliveryDbContext())
            {
                db.Database.SetCommandTimeout(120);
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();


                var dbContextUserInitiallizer = new DbContextUserInitializer();
                dbContextUserInitiallizer.Init(db);

                var businessDbContextInitiallizer = new BusinessDbContextInitiallizer();
                businessDbContextInitiallizer.Init(db);

                var messagesInitiallizer = new MessagesDbContextInitializer();
                messagesInitiallizer.Init(db);

                var initiallizer = new DeliveryDbContextInitiallizer();
                initiallizer.InitFromFiles(db, "D:\\System-Config\\ProjectsConsole\\Console_BusinessModel\\wwwroot\\Resources");
            }
        }
    }
}
