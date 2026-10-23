using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{
    /// <summary>
    /// Инициализация всех контекстов баз данных
    /// </summary>
    public class DbContextLoad
    {

        /// <summary>
        /// Пересоздание структуры баз данных
        /// </summary>
        public static void DoInit(bool interactive = false)
        {
            foreach (var dbContextType in ServiceFactory.Get().GetTypesExtended<DbContext>().Where(p => p != typeof(DbContextBase)))
            {
                try
                {
                    dbContextType.Info($"Создаю схему данных {dbContextType.GetTypeName()}");
                    Console.Title = $"Создаю схему данных {dbContextType.GetTypeName()}";
                    if (interactive && ConfirmContinue())
                    {
                        DoInit(dbContextType);
                    }
                }
                catch (Exception ex)
                {
                    ex.Error($"Ошибка при инициализации контекста данных: {dbContextType} {ex.Message}");
                    if (interactive && ConfirmContinue())
                    {
                        continue;
                    }
                }
            }
        }


        /// <summary>
        /// Инициализация набора данных
        /// </summary>
        public static void DoInit(Type dbContextType)
        {
            dbContextType.Info($"Инциаллизирую данные {dbContextType.GetTypeName()}");
            bool completed = false;
            string connectionString = null;
            do
            {
                using (var dbContext = connectionString is null ?
                    dbContextType.New<DbContext>() :
                    dbContextType.New<DbContext>(new object[] { connectionString }))
                {

                    dbContext.Database.EnsureCreated();
                    if (!dbContext.Database.CanConnect())
                    {

                        connectionString = ConnectionSettingDialog($"Настройки соединения: " +
                            $"'{dbContext.GetType().GetLabel()}'");
                    }
                    else
                    {

                        dbContext.Info("Обновляю структуру данных");
                        try
                        {
                            var baseType = typeof(DbContextInitiliazer<>).MakeGenericType(dbContextType);

                            var initiallizers = ServiceFactory.Get().GetTypesExtended(baseType);
                            if (initiallizers.Count() == 0)
                            {
                                dbContext.Database.EnsureDeleted();
                                dbContext.Database.EnsureCreated();
                            }
                            else
                            {
                                foreach (var ptype in initiallizers)
                                {
                                    object initiallizer = ptype.New();
                                    initiallizer.Invoke("ReInit", new object[] { dbContext });
                                }
                            }

                            completed = true;
                        }
                        catch (Exception ex)
                        {
                            ex.Error($"Ошибка при создании структуры данных: {ex}");
                        }
                    }
                }
            }
            while (!completed);
        }


        /// <summary>
        /// Обработка каждой сущности
        /// </summary> 
        public static void OnEntity(Action<DbContext, Type> onentity)
        {
            foreach (var dbContextType in ServiceFactory.Get().GetTypesExtended<DbContext>())
            {
                using (var dbContext = (DbContext)dbContextType.New())
                {
                    foreach (var entityType in dbContext.GetEntitiesTypes())
                    {
                        try
                        {
                            onentity(dbContext, entityType);
                        }
                        catch (Exception ex)
                        {
                            dbContext.Error($"Ошибка при обходе сущности: {dbContext.GetType().GetTypeName()} {entityType.GetTypeName()}");
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Запрос подтверждения выполнения через консоль
        /// </summary>
        public static bool ConfirmContinue()
        {
            Console.WriteLine("Чтобы продолжить ENTER, чтобы отменить нажмите ESCAPE?");
            do
            {
                ConsoleKey k;
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.Escape: return false;
                    case ConsoleKey.Enter: return true;
                }
            } while (true);
        }


        /// <summary>
        /// Открытие диалога "настройки соединения"
        /// </summary>     
        public static string ConnectionSettingDialog(string title)
        {
            throw new NotImplementedException();
            //Form form = null;
            //string connectionString = null;
            //InputPanel<SqlServerADODataSource> panel = null;
            //panel = new InputPanel<SqlServerADODataSource>(
            //    typeof(SqlServerConnectionString).GetOwnInputPropertyNames(),
            //(status, completed) =>
            //{
            //    if (status)
            //    {
            //        connectionString = completed.ToString();
            //        form.Close();
            //    }
            //});
            //panel.SetStrictValidation();
            //form = panel.ToBaseForm(title);
            //form.ShowDialog();
            //return connectionString;
        }
    }
}
