using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

/// <summary>
/// Расширения по сущностям
/// </summary>
public static class EntityExtensions
{


    /// <summary>
    /// Создание экземпляра сервиса интерфейса IControlService<ТИП ВЫЗЫВАЮЩЕГО ОБЪЕКТА> 
    /// </summary>
    /// <typeparam name="TService">тип сервиса</typeparam>
    /// <param name="target"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static TService CreateControlService<TService>(this object target)
    {
        //typeof(TService) == typeof(IControlService<>).MakeGenericType(target.GetType())
        var allInterfaces = 
            target.GetType().GetExtendedTypesWithSelf()
                .Select(type => typeof(IControlService<>).MakeGenericType(type));
        if( !allInterfaces.Any(i => typeof(TService).IsImplements(i)))
        {
            throw new ArgumentException(nameof(TService), 
                $"Сервис типа {typeof(TService).GetTypeName()} не поддерживается для {target.GetTypeName()}");
        }
        TService service = (TService)AppProviderService.GetInstance().GetService(typeof(TService));
        var setControlMethod = service.GetType().GetMethods().First(m => m.Name == "SetControl");
        setControlMethod.Invoke(service, new object[] { target });
        return service;
    }

    /// <summary>
    /// 
    /// </summary>
    public static List<EntityRelations> GetEntityRelations(this Type ptype)
    {
        return ptype.New<BaseEntity>().GetEntityRelations();
    }


    public static List<EntityRelations> GetManyToOneRelations(this Type ptype)
    {
        return ptype.New<BaseEntity>().GetEntityRelations().Where(r => r.RelationshipType == "many-to-one").ToList();
    }




    public static List<EntityRelations> GetOneToOneRelations(this Type ptype)
    {
        return ptype.New<BaseEntity>().GetEntityRelations().Where(r => r.RelationshipType == "one-to-one").ToList();
    }

    public static List<EntityRelations> GetOneToManyRelations(this Type ptype)
    {
        return ptype.New<BaseEntity>().GetEntityRelations().Where(r => r.RelationshipType == "one-to-many").ToList();
    }


    /// <summary>
    /// Наследуемые типы
    /// </summary>
    public static List<Type> GetExtendedTypesWithSelf(this Type ptype)
    {
        List<Type> res = new() { ptype };
        res.AddRange(ptype.GetExtendedTypes());        
        return res;
    }
    public static List<Type> GetExtendedTypes(this Type ptype)
    {
        Type typeOfObject = new object ().GetType();
        Type p = ptype;
        List<Type> extTypes = new();
        while (p != typeOfObject && p != null)
        {            
            p = p.BaseType;
            if(p is not null)
            {
                extTypes.Add(p);
            }
        }
        return extTypes;
    }


    /// <summary>
    /// Проверка наличия атрибута NotMapped
    /// </summary>
    public static bool IsMapped(this Type type, string property)
    {
        return
            Utils.ForProperty2(type, property).ContainsKey(nameof(NotMappedAttribute)) == false &&
            Utils.ForProperty2(type, property).ContainsKey("NotMapped") == false;
    }


    /// <summary>
    /// Создание фасада сущности
    /// </summary>
    public static dynamic GetEntityFasadeForType(this Type entityType)
    {
        return (dynamic) typeof(EfEntityFasade<>).MakeGenericType(entityType).New(new object[] {
            entityType.GetDbContextWithEntityType().New<DbContext>()
        });
    }



    /// <summary>
    /// Функции для работы с именованым справочником
    /// </summary>
    public static dynamic GetNamedEntityFasadeForType(this Type entityType)
    {
        return (dynamic)typeof(EfNamedObjectEntityFasade<>).MakeGenericType(entityType).New(new object[] {
            entityType.GetDbContextWithEntityType().New<DbContext>()
        });
    }


    /// <summary>
    /// Создание фасада сущности
    /// </summary>
    public static EfEntityFasade<TModel> GetEntityFasade<TDbContext, TModel>(this Type entityType) 
        where TModel : BaseEntity
        where TDbContext : DbContext
    {
        return new EfEntityFasade<TModel>(typeof(TDbContext).New<DbContextBase>());
    }


    /// <summary>
    /// Создание фасада сущности
    /// </summary>
    public static EfEntityFasade<TModel> GetEntityFasade<TModel>(this Type entityType) where TModel : BaseEntity
    {
        try
        {
            using (var db = typeof(TModel).GetDbContextWithEntityType().New<DbContextBase>())
            {
                return new EfEntityFasade<TModel>(db);
            }
        }
        catch(Exception ex)
        {
            throw new Exception($"Не удалось создать EfEntityFasade<{typeof(TModel).GetTypeName()}>() => {ex.Message}");
        }        
    }
 

    /// <summary>
    /// Получение контекста данных
    /// </summary>
    public static DbContext GetDbContext(this Type entityType)  
    {
        return entityType.GetDbContextWithEntityType().New<DbContext>();
    }
}

