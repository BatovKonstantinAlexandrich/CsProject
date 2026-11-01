using static System.Console;
using static Newtonsoft.Json.JsonConvert;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;


public class EfNamedObjectEntityFasade<TEntity> : EfEntityFasade<TEntity> where TEntity : NamedObject
{
    public EfNamedObjectEntityFasade(DbContextBase context) : base(context)
    {
    }

    public TEntity FindByName(string name)
    {
        return GetDbSet().FirstOrDefault(x => x.Name == name);
    }
}
public class EfEntityFasade : IEfEntityFasade<Dictionary<string, object>>
{
    private readonly DbContextBase dbContextBase;

    public EfEntityFasade(DbContextBase dbContextBase, Type entityType)
    {
        this.dbContextBase = dbContextBase;
    }

    private Type entityType { get; set; }



    public int Create(Dictionary<string, object> item)
    {
        var pobject = entityType.New();
        pobject.Set(item);
        dbContextBase.SetEntity(entityType).Add(pobject);
        dbContextBase.SaveChanges();
        return pobject.GetValue<int>("Id");
    }

    public int CreateObjects(params Dictionary<string, object>[] items)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> Get(params int[] ids)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> GetAll()
    {
        throw new NotImplementedException();
    }

    public int Count()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<string> GetOptions(string searchQuery)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> GetPage(int page, int size)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> GetPage(int[] ids, int page, int size)
    {
        throw new NotImplementedException();
    }

    public int Remove(int id)
    {
        throw new NotImplementedException();
    }

    public int RemoveIds(params int[] ids)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> Search(string query)
    {
        throw new NotImplementedException();
    }

    public int TotalPages(string searchQuery, int v)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> SearchColumnsInPropertiesPage(string query, string[] properties, string[] columns, int page, int size)
    {
        throw new NotImplementedException();
    }

    public int TotalResults(string searchQuery)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> SearchInProperties(string query, string[] properties)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> SearchInPropertiesPage(string query, string[] properties, int page, int size)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Dictionary<string, object>> SearchPage(string query, int page, int size)
    {
        throw new NotImplementedException();
    }

    public int Update(params Dictionary<string, object>[] items)
    {
        throw new NotImplementedException();
    }
}

public class EfEntityFasade<TEntity> : IEfEntityFasade<TEntity> where TEntity : BaseEntity
{

    protected DbContext _context;
    public virtual IQueryable<TEntity> GetDbSet() 
    {
        try
        {            
            if(!_context.GetEntitiesTypes().Contains(typeof(TEntity)))
            {
                throw new Exception($"В контексте данных не зарегистрирована сущность {typeof(TEntity).GetTypeName()}");
            }            
            return _context.Set<TEntity>(); ;
        }
        catch
        {
            var message = $"Не удалось получить набор данных сущности {typeof(TEntity).GetTypeName()} через контекст {_context.GetTypeName()} соединенный: {_context.Database.GetConnectionString()}";
            this.Error(message);
            throw new Exception(message);
        }
        
    }

    public EfEntityFasade()
    {
         
    }
    public EfEntityFasade(DbContext context)
    {
        if((_context = context.GetType().New<DbContext>()) is null)
        {
            throw new ArgumentNullException(nameof(context));   
        }
    }

    public string GetTableName()
    {
        var tableAttribute  = typeof(TEntity).GetAttributes().Where(a => a.Key == nameof(TableAttribute)).Select(kv => kv.Value).FirstOrDefault();
        if(tableAttribute is not null)
        {
            throw new ArgumentNullException(nameof(tableAttribute));
        }
        else
        {
            var propertyType = typeof(DbSet<>).MakeGenericType(typeof(TEntity));
            var property = _context.GetType().GetProperties().FirstOrDefault(p => propertyType == p.PropertyType);
            return property.Name;
        }
    }

    public int Count()
    { 
        return GetDbSet().Count();
    }
    public IEnumerable<TEntity> Select(Predicate<object> filter)
    {
        return this.GetDbSet().Where(item => filter(item)).ToList();
    }
    public TEntity FirstOrDefault(Predicate<object> filter)
    {
        return Select(filter).FirstOrDefault();
    }
    public int Create(TEntity item)
    {
      
         _context.Add(item);
        
        return _context.SaveChanges();
    }

    

    public IQueryable<TEntity> Filter(params Predicate<TEntity>[] filters)
    {
        var p = GetDbSet();
        foreach (Predicate<TEntity> filter in filters)
        {
            p = p.Where( i => filter(i) );
        }
        return p;
    }

    public TableMetadata GetTableMetadata()
    {
        var api = ((DbContextBase)_context).GetSqlServerWebApi();
        var prop = _context.GetType().GetProperties().FirstOrDefault(p => p.PropertyType.GetTypeName() == $"DbSet<{typeof(TEntity).GetTypeName()}>");
        var table = prop.Name;
        var metadata = api.GetTableMetadata(table);
        return metadata;
    }

    public int CreateObjects(params TEntity[] items)
    {
        foreach (var item in items)
        {
            _context.Add(item);
        }
        return _context.SaveChanges();
    }

    public TEntity Get(int ids)
        => GetDbSet().FirstOrDefault(item => ids == (item.Id));
    public IEnumerable<TEntity> Get(int[] ids)
        => GetDbSet().Where(item => ids.Contains(item.Id));

 
    public IEnumerable<TEntity> GetPage(int[] ids, int page, int size)
        => GetDbSet().Where(item => ids.Contains(item.Id)).Skip((page - 1) * size).Take(size);

    public IEnumerable<TEntity> GetAll() => GetDbSet().ToList();

    public IEnumerable<TEntity> GetPage(int page, int size = 10) => GetDbSet().Skip((page - 1) * size).Take(size);

    public int Remove(int id)
    {
        _context.Remove(Get(id));
        return _context.SaveChanges();
    }
    public int RemoveIds(params int[] ids)
    {
        Get(ids).ToList().ForEach(item => _context.Remove(item));
        return _context.SaveChanges();
    }
    public int Update(params TEntity[] items)
    {
        foreach (var item in items)
            _context.Update(item);
        return _context.SaveChanges();
    }




    //TODO: нужно реализовать разбиение по словам
    public IEnumerable<TEntity> Search(string query) 
    {

        List<TEntity> results = new();
        if (String.IsNullOrWhiteSpace(query))
            return GetDbSet().ToList();
        foreach(var item in GetDbSet())
        {
            string text = item.ToDictionary().Values.ToList().ToJson();
            if ( text.ToLower().IndexOf(query.ToLower()) != -1)
            {
                results.Add(item);
            }
        }
        return results;
    }


    public IEnumerable<TEntity> SearchPage(string query, int page, int size)
    {
        List<TEntity> result = new List<TEntity>();

        foreach(var item in GetDbSet().ToList())
        {
            if (string.IsNullOrEmpty(query) || SerializeObject(item).Contains(query))
            {
                result.Add(item);
                if(result.Count >= size)
                    break;
            }
        };

        WriteLine(SerializeObject(result.Skip((page - 1) * size).Take(size).ToArray()));
        return result;//.Skip((page - 1) * size).Take(size).ToArray();
    }

    public IEnumerable<object> SearchColumnsInPropertiesPage(string query, string[] properties, string[] columns, int page, int size)
        => SearchInPropertiesPage(query, properties, page, size).Select(next => SelectProperties(next, columns));

    private object SelectProperties(object next, string[] columns)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<object> SearchInPropertiesPage(string query, string[] properties, int page, int size)
        => GetDbSet().Select(item => item.ToDictionary().Where(kv => properties.Contains(kv.Key))).Skip((page - 1) * size).Take(size);

    public IEnumerable<object> SearchInProperties(string query, string[] properties)
        => GetDbSet().Select(item => item.ToDictionary()).Where(dic => SerializeObject(dic.Values).ToLower().Contains(query.ToLower()));

    public int TotalPages(string query, int size)
    {
        List<TEntity> result = new List<TEntity>();
        GetDbSet().ToList().ForEach(item =>
        {
            if (string.IsNullOrEmpty(query) || SerializeObject(item).Contains(query))
            {
                result.Add(item);
            }
        });
        return 1 + (int)Math.Floor((decimal)result.Count() / size);
    }

    public int TotalResults(string query)
    {
        List<TEntity> result = new List<TEntity>();
        GetDbSet().ToList().ForEach(item =>
        {
            if (string.IsNullOrEmpty(query) || SerializeObject(item).Contains(query))
            {
                result.Add(item);
            }
        });
        return result.Count();
    }

    public IEnumerable<string> GetOptions(string query)
    {
        HashSet<string> result = new HashSet<string>();
        var list = GetDbSet().ToList();
        foreach(TEntity item in list)
        {
      
            IEnumerable<string> keywords = GetKeywords(item);
            //this.Info(new { p1 = item.ToJson(), p2 = keywords });
            keywords.ToList().ForEach(i =>
            {
                if (String.IsNullOrWhiteSpace(query) == false && i.ToLower().IndexOf(query.ToLower()) != -1)
                {
                    result.Add(i.ToLower());
                }

            });
        }
         
        return result;
    }

    private IEnumerable<string> GetKeywords(TEntity item)
    {
        try
        {
            List<string> words = new();
            var parser = new StupidKeywordsParserService();
            foreach (var val in item.ToTextDictionary().Values.ToList())
            {
                words.AddRange(parser.ParseKeywords($"{val}").Keys.Select(key => key.ToUpper()));
            }           
            return words.ToHashSet();
        }
        catch(Exception)
        {
            return new List<string>(); 
        }
    }
}

public interface IKeywordsParserService
{
    public IDictionary<string, int> ParseKeywords(string Resource);
}


public class StupidKeywordsParserService : IKeywordsParserService
{


    private IDictionary<string, int> keywords = new Dictionary<string, int>();

    public StupidKeywordsParserService()
    {
    }

    public IDictionary<string, int> ParseKeywords(string Resource)
    {

        var statisticsForThisRecord = new Dictionary<string, int>();

        foreach (string text in Resource.SplitWords())
        {
            string word = text.ToUpper();
            if (keywords.ContainsKey(word))
            {
                keywords[word]++;
            }
            else
            {
                keywords[word] = 1;
            }

            if (statisticsForThisRecord.ContainsKey(word))
            {
                statisticsForThisRecord[word]++;
            }
            else
            {
                statisticsForThisRecord[word] = 1;
            }
        }
        return statisticsForThisRecord;
    }
}