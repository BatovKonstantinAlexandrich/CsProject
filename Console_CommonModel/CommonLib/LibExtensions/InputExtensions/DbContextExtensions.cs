using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


public static class DbContextExtensions
{
    public static IEnumerable<string> GetNavigationPropertiesNames(this Type target)
    {
        using (var db = target.GetDbContextWithEntityType().New<DbContext>())
        {
            var types = db.Model.GetEntityTypes();
            IEntityType entity = (from navs in types where navs.Name == target.FullName select navs).SingleOrDefault();
            if (entity is null)
                throw new ArgumentNullException("Ссылка недействительна");
            return entity.GetNavigations().Select(p => p.Name);
        }

    }

    public static int Create(this DbContext db, BaseEntity p)
    {
        db.Add(p);
        db.SaveChanges();
        return p.Id;
    }
    public static IEnumerable<INavigation> GetNavigation<TDBContext>(this Type target ) where TDBContext : DbContext
    {
        using (var db = typeof(TDBContext).New<DbContext>())
        {
            IEntityType entity = (from navs in db.Model.GetEntityTypes() where navs.Name == target.FullName select navs).SingleOrDefault();
            if (entity is null)
                throw new ArgumentNullException("Ссылка недействительна");
            return entity.GetNavigations();
        }
    }
    public static IEnumerable<INavigation> GetNavigationProperties<TDbContext>(this Type target )
        where TDbContext : DbContextBase
    {
        using(var db = typeof(TDbContext).New<TDbContext>())
        {
            IEntityType entity = (from navs in db.Model.GetEntityTypes() where navs.Name == target.FullName select navs).SingleOrDefault();
            if (entity is null)
                throw new ArgumentNullException("Ссылка недействительна");
            return entity.GetNavigations();
        }
        
    }
    /// <summary>
    /// Получение типа в котором реализован DbSet для вызывающей стороны
    /// </summary>
    public static Type GetDbContextWithEntity(this object p)
    {
        var target = p is Type ? (Type)p : p.GetType();
        if (target is Type)
            throw new ArgumentException("Метод GetDbContextWithEntity нужно использовать на ссылки на объект" );
        foreach(var pdbtype in ServiceFactory.Get().GetTypesExtended<DbContext>())
        {
            using (var pdb = (DbContextBase)pdbtype.New())
            {
                //pdb.GetEntitiesTypes().Select(t => t.GetTypeName()).ToJsonOnScreen().WriteToConsole();
                var pentity = pdb.GetEntitiesTypes().FirstOrDefault(e => e.GetTypeName() == target.GetType().GetTypeName());
                if (pentity != null)
                    return pdbtype;
            }            
        }
        throw new Exception("не найден контекст данных для " + target.GetType().GetTypeName());
    }

    public static Type GetDbContextWithEntityType(this Type p)
    {
        var target = p is Type ? (Type)p : p.GetType();
        if (p is not Type)
            throw new ArgumentException("Метод GetDbContextWithEntity нужно использовать на ссылки на объект");
        foreach (var pdbtype in ServiceFactory.Get().GetTypesExtended<DbContext>().Where(t => t.GetGenericArguments().Count()==0))
        {             
            using (var pdb = (DbContextBase)pdbtype.New())
            {
                //pdb.GetEntitiesTypes().Select(t => t.GetTypeName()).ToJsonOnScreen().WriteToConsole();
                //pdb.GetEntitiesTypes().Select(p => p.GetTypeName()).ToJsonOnScreen().WriteToConsole();
                var pentity = pdb.GetEntitiesTypes().FirstOrDefault(e => e.GetTypeName() == target.GetTypeName());
                if (pentity != null)
                    return pdbtype;
            }
        }
        throw new Exception("не найден контекст данных для " + target.GetTypeName());
    }

    /// <summary>
    /// Нехороший способ извеления наименований сущностей
    /// </summary>
    /// <param name="dbContext"> контекст данных </param>
    /// <returns> множество наименований сущностей </returns>
    public static HashSet<Type> GetEntitiesTypes(this Type dbContextType)
    {
        HashSet<Type> entities = new HashSet<Type>();

        // Используем свойства вместо методов для более надежного определения
        var properties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            // Проверяем, что свойство возвращает DbSet<>
            if (prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                // Пропускаем MigrationHistory
                if (prop.Name.IndexOf("MigrationHistory", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    var entityType = prop.PropertyType.GetGenericArguments()[0];

                    // Проверяем, что это действительно класс сущности
                    if (IsValidEntityType(entityType))
                    {
                        entities.Add(entityType);
                    }
                }
            }
        }
        if (entities.Count() == 0)
            throw new Exception($"Не найдены классы сущностей для {dbContextType.GetTypeName()}");
        return entities;
    }

     
    public static HashSet<Type> GetEntitiesTypes(this DbContext dbContext)
    {
        
        if(dbContext is null)
            throw new ArgumentNullException(nameof(dbContext));
         
        Type type = dbContext.GetType();
        HashSet<Type> entities = new HashSet<Type>();
        
        // Используем свойства вместо методов для более надежного определения
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            // Проверяем, что свойство возвращает DbSet<>
            if (prop.PropertyType.IsGenericType && 
                prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                // Пропускаем MigrationHistory
                if (prop.Name.IndexOf("MigrationHistory", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    var entityType = prop.PropertyType.GetGenericArguments()[0];
                    
                    // Проверяем, что это действительно класс сущности
                    if (IsValidEntityType(entityType))
                    {
                        entities.Add(entityType);
                    }
                }
            }
        }
        //if (entities.Count() == 0)
          //  throw new Exception("Не найдены классы сущностей");
        return entities;
    }
    
    /// <summary>
    /// Проверяет, является ли тип валидным типом сущности
    /// </summary>
    private static bool IsValidEntityType(Type type)
    {
        if (type == null)
            return false;
            
        // Не должен быть делегатом
        if (typeof(Delegate).IsAssignableFrom(type))
            return false;
            
        // Не должен быть интерфейсом
        if (type.IsInterface)
            return false;
            
        // Не должен быть примитивным типом или значением
        if (type.IsPrimitive || type.IsValueType)
            return false;
            
        // Должен быть классом
        if (!type.IsClass)
            return false;
            
        // Не должен быть абстрактным (опционально, но обычно сущности не абстрактные)
        // if (type.IsAbstract)
        //     return false;
            
        return true;
    }
    public static IEnumerable<INavigation> GetNavigationPropertiesForType(this DbContext dbContext, Type type)
    {
        foreach(var nav in dbContext.Entry(type).Navigations)
        {
            
        }
        return new List<INavigation>();
    }


    public static DbContextInitiliazer<TDbContext> GetInitializer<TDbContext>(this DbContext dbContext) where TDbContext : DbContextBase
    {
        var p = ServiceFactory.Get().GetTypesExtended<DbContextInitiliazer<TDbContext>>().FirstOrDefault();
        if (p is not null)
            return p.New<DbContextInitiliazer<TDbContext>>();
        return null;
    }
    public static DbContextInitiliazer<TDbContext> _GetInitializer<TDbContext>() where TDbContext : DbContextBase
    {
        var p = ServiceFactory.Get().GetTypesExtended<DbContextInitiliazer<TDbContext>>().FirstOrDefault();
        if (p is not null)
            return p.New<DbContextInitiliazer<TDbContext>>();
        return null;
    }
    public static object GetInitializerBase(this DbContext dbContext) 
    {        
        var action = typeof(DbContextExtensions).GetMethod("_GetInitializer").MakeGenericMethod(dbContext.GetType());
       
        object result = action.Invoke(null, new object[0]);
        return result;        
    }
    public static List<Type> GetDbSetServices (this DbContext dbContext, string entity, Type TService)
    {
        var attrs = dbContext.GetDbSetAttributes(entity);
        //dbContext.Info(attrs.ToJsonOnScreen());
        var keys = attrs.Keys.Where(key => {
            var ptype = key.ToType();
            return ptype.IsExtends(TService) && ptype.IsAbstract == false;
        }).ToList();
        return keys.Select(key => key.ToType()).ToList();
    }

    public static List<TService> GetDbSetServices<TService>(this DbContext dbContext, string entity)
    {
        var attrs = dbContext.GetDbSetAttributes(entity);
        var keys = attrs.Keys.Where(key => key.ToType().IsExtends(typeof(TService))).ToList();
        return keys.Select(key => key.New< TService>(attrs[key])).ToList();
    }

    public static Dictionary<string,string> GetDbSetAttributes(this DbContext dbContext, string entity)
    {
        Type type = dbContext.GetType();
        var properties = dbContext.GetType().GetProperties();
        
        //dbContext.Info(properties.Select(p=>p.PropertyType.GetTypeName()).ToJsonOnScreen());
        var prop = properties.FirstOrDefault(p => p.PropertyType.GetTypeName() == $"DbSet<{entity.ToType().GetTypeName()}>");
        var helper = new TypeHelperStrict();
        var attrs = helper.GetPropertyAttributes(dbContext.GetType(), prop.Name);
        return attrs;        
    }
    public static dynamic GetDbSet(this DbContext dbContext, string entity)
    {
        Type type = dbContext.GetType();
        HashSet<Type> entities = new HashSet<Type>();
        foreach (MethodInfo info in type.GetMethods())
        {
            var rs = new ReflectionService();
            var all = dbContext.GetType().GetProperties().Where(prop => prop.PropertyType.IsExtendsFrom(typeof(DbSet<>))).Select(prop => prop.PropertyType.GetGenericArguments()[0].GetTypeName()).ToList();
            var properties = dbContext.GetType().GetProperties();
            var dbsets = properties.Where(prop => prop.PropertyType.IsExtendsFrom(typeof(DbSet<>).MakeGenericType(entity.ToType()))).ToList();
            var prop = dbsets.FirstOrDefault(prop => prop.PropertyType.GetGenericArguments().Count() > 0 && prop.PropertyType.GetGenericArguments()[0].GetTypeName() == entity);
            if(prop is null)
            {
                throw new ArgumentException(nameof(entity), $"Не удалось найти набор данных для {entity}");
            }
            var dbset = prop.GetValue(dbContext);

            if (dbset is null)
            {
                throw new ArgumentException("entity", $"Не удалось найти массив данных для {entity}");
            }
            else
            {
                return dbset;
            }
        }
        throw new ArgumentException("entity", $"Не удалось найти массив данных для {entity}");
    }
    public static dynamic GetDbSet(this DbContext dbContext, Type entity)
    {
        Type type = dbContext.GetType();
        HashSet<Type> entities = new HashSet<Type>();
        foreach (MethodInfo info in type.GetMethods())
        {
            var rs = new ReflectionService();
            var all = dbContext.GetType().GetProperties().Where(prop => prop.PropertyType.IsExtendsFrom(typeof(DbSet<>))).Select(prop => prop.PropertyType.GetGenericArguments()[0].GetTypeName()).ToList();
            var properties = dbContext.GetType().GetProperties();
            var dbsets = properties.Where(prop => prop.PropertyType.IsExtendsFrom(typeof(DbSet<>).MakeGenericType(entity))).ToList();
            var prop = dbsets.FirstOrDefault(prop => prop.PropertyType.GetGenericArguments().Count() > 0 && prop.PropertyType.GetGenericArguments()[0].GetTypeName() == entity.GetTypeName());
            if (prop is null)
            {
                throw new ArgumentException(nameof(entity), $"Не удалось найти набор данных для {entity}");
            }
            var dbset = prop.GetValue(dbContext);

            if (dbset is null)
            {
                throw new ArgumentException("entity", $"Не удалось найти массив данных для {entity}");
            }
            else
            {
                return dbset;
            }
        }
        throw new ArgumentException("entity", $"Не удалось найти массив данных для {entity}");
    }
}
    
