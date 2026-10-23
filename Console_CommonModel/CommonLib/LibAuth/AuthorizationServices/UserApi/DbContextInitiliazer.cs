using Microsoft.EntityFrameworkCore;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbConnectionStrings;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

public abstract class DbContextInitiliazer<TDBContext> 
    where TDBContext: DbContext
{


    /// <summary>
    /// Открытие диалога "настройки соединения"
    /// </summary>     
    private static string GetConnectionSetting(string title)
    {
        //    Form form = null;
        //    string connectionString = null;
        //    InputPanel<SqlServerADODataSource> panel = null;
        //    panel = new InputPanel<SqlServerADODataSource>(
        //        typeof(SqlServerConnectionString).GetOwnInputPropertyNames(),
        //    (status, completed) =>
        //    {
        //        if (status)
        //        {
        //            connectionString = completed.ToString();
        //            form.Close();
        //        }
        //    });
        //    panel.SetStrictValidation();
        //    form = panel.ToBaseForm(title);
        //    form.ShowDialog();
        //    return connectionString;
        throw new NotImplementedException();
    }

    public static void InitDatabase()
    {
        Type dbContextType = typeof(TDBContext);


        dbContextType.Info($"Инциаллизирую данные {dbContextType.GetTypeName()}");
        bool completed = false;
        string connectionString = null;
                                
        if (!dbContextType.New<DbContextBase>().GetSqlServerWebApi().CanConnect())
        {

            connectionString = GetConnectionSetting($"Настройки соединения: " +
                $"'{dbContextType.GetLabel()}'");
        }
        else
        {                                        
            var extTypes = dbContextType.GetExtendedTypes().ToList();
            extTypes.Remove(typeof(Object));
            extTypes.Remove(typeof(DbContext));
            extTypes.Remove(typeof(DbContextBase));
            extTypes.Reverse();
            extTypes.Info(extTypes.Select(t => t.GetTypeName()).ToList().ToJsonOnScreen());
            foreach (var extType in extTypes)
            {
                try
                {        
                    var baseType = typeof(DbContextInitiliazer<>).MakeGenericType(extType);
                    var initiallizers = ServiceFactory.Get().GetTypesExtended(baseType).Where(t => !t.IsGenericTypeDefinition);
                    baseType.Info($"Обновляю структуру данных {dbContextType.GetTypeName()} сервисами {baseType.GetTypeName()}");
                    foreach (var ptype in initiallizers)
                    {
                        try
                        {
                            dbContextType.Info(ptype.GetTypeName());
                            object initiallizer = ptype.New();
                            initiallizer.Invoke("ReInit", new object[] { dbContextType.New<DbContext>() });
                        }
                        catch(Exception ex) {
                            ex.Message.WriteToConsole();
                        }
                    }                            
                    completed = true;
                }
                catch (Exception ex)
                {
                    dbContextType.Error($"Ошибка при создании структуры данных: {ex}");
                }
            }
                    
                
        }
       
    
    }
    public abstract Dictionary<string, int> Init(TDBContext context);
    public virtual Dictionary<string, int> ReInit(TDBContext context)
    {
        this.Info(context.Database.GetConnectionString());
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        return Init(context);
    }

    public virtual Dictionary<string, int> CreateDbContext()
    {
        using (var db = typeof(TDBContext).New<TDBContext>())
        {
            db.Validate();
            return ReInit(db);
        }            
    }

    public static void CreateDatabase()
    {
        using (var db = typeof(TDBContext).New<TDBContext>())
        {
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }
    }
}
