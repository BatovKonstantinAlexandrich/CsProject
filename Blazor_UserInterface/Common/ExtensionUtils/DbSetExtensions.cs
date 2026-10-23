using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class DbSetExtensions
{
    /// <summary>
    /// Нехороший способ извеления наименований сущностей
    /// </summary>
    /// <param name="subject"> контекст данных </param>
    /// <returns> множество наименований сущностей </returns>
    public static HashSet<Type> GetEntitiesTypes(this DbContext subject)
    {
        Type type = subject.GetType();
        HashSet<Type> entities = new HashSet<Type>();
        foreach (MethodInfo info in type.GetMethods())
        {
            if (info.Name.StartsWith("get_") == true && info.ReturnType.Name.StartsWith("DbSet"))
            {
                if (info.Name.IndexOf("MigrationHistory") == -1)
                {
                    entities.Add(info.ReturnType);
                }
            }
        }
        return entities;
    }


    public static HashSet<string> GetEntityTypeNames(this Type type)
    {       
        HashSet<string> entities = new HashSet<string>();
        foreach (MethodInfo info in type.GetMethods())
        {
            if (info.Name.StartsWith("get_") == true && info.ReturnType.Name.StartsWith("DbSet"))
            {
                if (info.Name.IndexOf("MigrationHistory") == -1)
                {
                    entities.Add(Typing.ParseCollectionType(info.ReturnType));
                }
            }
        }
        return entities;
    }







    public static Dictionary<string, object> GetDbSets(this DbContext _context)
    {
        var res = new Dictionary<string, object>();
        foreach (MethodInfo info in _context.GetType().GetMethods())
        {
            if (info.Name.StartsWith("get_") == true && info.ReturnType.Name.StartsWith("DbSet"))
            {
                if (info.Name.IndexOf("MigrationHistory") == -1)
                {
                    string displayName = info.ReturnType.ShortDisplayName();
                    string entityTypeName = displayName.Substring(displayName.IndexOf("<") + 1);
                    entityTypeName = entityTypeName.Substring(0, entityTypeName.IndexOf(">"));
                    res[entityTypeName] = (dynamic)info.Invoke(_context, new object[0]);
                }

            }
        }
        return res;
    }
 
    public static dynamic GetDbSet(this DbContext _context, string entityTypeShortName)
    {
        try
        {
            foreach (MethodInfo info in _context.GetType().GetMethods())
            {
                if (info.Name.StartsWith("get_") == true && info.ReturnType.Name.StartsWith("DbSet"))
                {
                    if (info.Name.IndexOf("MigrationHistory") == -1)
                    {
                        string displayName = info.ReturnType.ShortDisplayName();
                        string entityTypeName = displayName.Substring(displayName.IndexOf("<") + 1);
                        entityTypeName = entityTypeName.Substring(0, entityTypeName.IndexOf(">"));
                        if (entityTypeShortName == entityTypeName)
                        {
                            return (dynamic)info.Invoke(_context, new object[0]);
                        }
                    }

                }
            }
        }
        catch (Exception)
        {
            return (dynamic)null;
        }
        

        throw new Exception($"Сущность [{entityTypeShortName}] не определена в контексте базы данных "+_context.GetType().Name);
    }

 
    public static DbSet<T> GetDbSet<T>(this DbContext _context) where T: class
    {
        return (DbSet<T>)_context.GetDbSet(typeof(T).Name);
    }
}
 