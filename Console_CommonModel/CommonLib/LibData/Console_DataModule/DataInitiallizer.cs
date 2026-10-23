using Microsoft.EntityFrameworkCore;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule
{

    /// <summary>
    /// Отвечает за добавление первичных данных
    /// </summary>
    [Label("Инициаллизация первичного набора данных")]
    public class DataInitiallizer
    {

        /// <summary>
        /// Ищет инициализаторы баз данных и запускает их
        /// </summary>        
        public static Dictionary<string, Dictionary<string, int>> InitDatabases()
        {
            var instance = new DataInitiallizer();
            Dictionary<string, Dictionary<string, int>> results = new();
            var logMessage = ServiceFactory.Get().GetDbContextsInitializers().Select(p => p.GetTypeName()).ToJson();
            instance.Info($"Найдены инициализаторы баз данных: ${logMessage}");

            foreach (var typeDbContext in ServiceFactory.Get().GetTypesExtended<DbContext>())
            {
                try
                {
                    var dbInitializerType = typeof(DbContextInitiliazer<>).MakeGenericType(typeDbContext);
                    var typeInitiallizer = ServiceFactory.Get().GetTypeNames().FirstOrDefault(name =>
                    {
                        if (name.IndexOf(typeDbContext.GetTypeName()) == -1)
                            return false;
                        var res = name.ToType().IsExtends(dbInitializerType);
                        return res;
                    });
                    if (typeInitiallizer is null)
                    {
                        throw new Exception($"Не реализован класс инициализации первичных данных базы {dbInitializerType.GetTypeName()}");
                    }
                    if (typeInitiallizer is not null && typeInitiallizer == $"DbContextInitiliazer<{typeDbContext.GetTypeName()}>")
                    {
                        throw new Exception("Тип инициализатора определён неправильно: " + typeInitiallizer);
                    }
                    try
                    {
                        typeDbContext.Info(typeInitiallizer);
                        object pinitializer = typeInitiallizer.New();
                        using (var db = typeDbContext.New<DbContext>())
                        {
                            results[typeDbContext.GetTypeName()] = (Dictionary<string, int>)
                                pinitializer.Invoke("Init", new List<object>() { db });
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
                }
                catch (Exception ex)
                {
                    instance.Error($"Ошибка при инициализации данных для {typeDbContext.GetTypeName()} => {ex.ToDocument()}");
                }
                finally
                {
                    //instance.Info(typeDbContext.GetTypeName());
                }
            }
            return results;
        }
    }
}