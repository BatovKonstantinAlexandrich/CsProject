using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;




/// <summary>
/// Методы упрощают инициализацию объектов
/// </summary>
public class ServiceFactory 
{
    public List<Type> GetDbContexts() =>
        Assemblies.SelectMany(assembly => assembly.GetTypes().Where(ptype => !ptype.IsAbstract && !ptype.IsGenericTypeDefinition && ptype.IsExtendsFrom(typeof(DbContext)))).ToList();
    public List<Type> GetDbContextsInitializers() =>
        this.GetTypesExtended(typeof(DbContextInitiliazer<>)).ToList();
    public List<string> GetAssembliesNames() => 
        this.Assemblies.Select(a =>
        $"{a.GetName().Name}").ToList();
    public List<string> GetAssembliesInfos() =>
        this.Assemblies.Select(a =>
        $"{a.GetName().Name + " v" + a.GetName().Version}").ToList();
    public List<string> GetTypeNames() => ByShortNames.Keys.ToList();
    public List<string> GetTypeNames<T>() where T: class => ByShortNames.Where(p => p.Value.IsExtends(typeof(T))).Select( p => p.Key).ToList();

    private static ServiceFactory Instance = null;
    public static ServiceFactory Get()
    {        
        if (Instance == null)
        {
            Instance = new ServiceFactory();
        }
        /*Instance.AddType(typeof(Dictionary<,>));
        Instance.AddTypes(typeof(ServiceFactory).Assembly);
        Instance.AddTypes(typeof(DataColumnCollection).Assembly);
        Instance.AddTypes(typeof(System.String).Assembly);
        Instance.AddTypes(typeof(System.Data.DataTable).Assembly);
        Instance.AddTypes(typeof(System.Data.DataSet).Assembly);
        Instance.AddTypes(Assembly.GetExecutingAssembly());
        Instance.AddTypes(Assembly.GetCallingAssembly());
        Instance.AddTypes(Assembly.GetEntryAssembly());*/
        return Instance;
    }


    public ConcurrentDictionary<string, Type> ByShortNames =
        new ConcurrentDictionary<string, Type>() {};
    private ConcurrentDictionary<string, Type> ByFullNames =
        new ConcurrentDictionary<string, Type>();

    public void AddType(string name, Type type)
    {
        ByShortNames[name] = type;
    }

    private ConcurrentDictionary<string, Type> ViewComponents =
        new ConcurrentDictionary<string, Type>();
    private ConcurrentDictionary<string, Type> Controllers =
        new ConcurrentDictionary<string, Type>();
    private ConcurrentDictionary<string, Type> DataModels =
       new ConcurrentDictionary<string, Type>();
   

    private ServiceFactory():base()
    {

        ByFullNames["Nulalble<System.DateTime>"] = typeof(DateTime?);
        ByShortNames["Nulalble<System.DateTime>"] = typeof(DateTime?);
        ByFullNames["Nulalble<System.Int32>"] = typeof(int?);
        ByFullNames["Nulalble<System.Int64>"] = typeof(long?);
        ByShortNames["Nulalble<System.Int32>"] = typeof(int?);
        ByShortNames["Nulalble<System.Int64>"] = typeof(long?);
        ByShortNames["Nulalble<DateTime>"] = typeof(DateTime?);
        ByShortNames["Single"] = typeof(Single);
        ByShortNames["Double"] = typeof(Double);
        ByShortNames["Object"] = typeof(Object);
        ByShortNames["DateTime"] = typeof(DateTime);

        ByShortNames["Byte[]"] = typeof(byte[]);
        ByShortNames["byte[]"] = typeof(byte[]);
        ByShortNames["String"] = typeof(System.String);
        ByShortNames["Int32"] = typeof(System.Int32);
        ByShortNames["Int64"] = typeof(System.Int64);
        ByShortNames["string"] = typeof(System.String);
        ByShortNames["int"] = typeof(System.Int32);
        ByShortNames["float"] = typeof(System.Decimal);
        ByShortNames["byte"] = typeof(System.Byte);
        ByShortNames["long"] = typeof(System.Int64);
    }
    /*
    public IMvcBuilder AddParts(IMvcBuilder mvcBuilder)
    {
        
        Assemblies.ToList().ForEach(assembly =>
        {
            mvcBuilder = mvcBuilder.AddApplicationPart(assembly);
        });
        return mvcBuilder;
    }*/
    public List<Type> GetTypesImplements(Type ptype) =>
        ByShortNames.Values.Where(v => v.IsImplementsFrom(ptype)).ToList();
    public List<Type> GetTypesExtendeds<T>() where T : class =>
        ByShortNames.Values.Where(v => v.IsExtendsFrom(typeof(T))).ToList();
    public List<Type> GetTypes()  =>
        ByShortNames.Values.ToList();
    public List<Type> GetTypesImplements<T>() where T : class =>
        ByShortNames.Values.Where(v => v.IsImplementsFrom(typeof(T))).ToList();
    public IEnumerable<string> GetNames() => this.ByShortNames.Keys.ToList();

    /// <summary>
    /// Получение типов абстрации для классов сущностей, 
    /// Наследуется от BaseEntity
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetEntityTypes() => 
        this.ByShortNames.Values.Where(val => 
        val != typeof(BaseEntity) && 
        val.IsAbstract == true && 
        val.IsExtends(typeof(BaseEntity))
    ).Select(P => P.Name);


    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void AddDataModel(Type context)
    {
        DataModels[context.Name] = context;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<string, Type> GetDataModels()
    {
        return DataModels;
    }

    /// <summary>
    /// Компоненты представлений
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<string, Type> GetControllers()
    {
        return Controllers;
    }


    /// <summary>
    /// Компоненты представлений
    /// </summary>
    /// <returns></returns>
    public ConcurrentDictionary<string, Type> GetViewComponents()
    {
        if(ViewComponents.Count()==0)
        {
            ViewComponents = new(GetTypesExtended<AppViewBase>().Where(t => !t.IsAbstract && t.GetGenericArguments().Count() == 0).Select(t => new KeyValuePair<string,Type>(t.GetTypeName(), t)).ToList());
        }
        return ViewComponents;
    }

    public ConcurrentDictionary<string, List<Type>> ViewServices = new();
    /// <summary>
    /// Компоненты представлений
    /// </summary>
    /// <returns></returns>
    public List<Type> GetViewServiceTypes<TView>()
        where TView : AppViewBase
    {
        var key = typeof(TView).GetTypeName();
        if (ViewServices.ContainsKey(key))
        {
            return ViewServices[key];
        }
        else
        {
            ViewServices[key] = GetTypesImplemented(typeof(IControlService<>).MakeGenericType(typeof(TView))).ToList();
        }
        return ViewServices[key];
    }

    public Type GetType(string name)
    {
        if (!ByShortNames.ContainsKey(name))
        {            
            var key = ByShortNames.Select(kv => kv.Key).FirstOrDefault(kv => kv.ToLower() == name.ToLower());
            if (key == null)
            {

                //var stackTrace = StackTraceAnalyzer.GetCallStack();
                //stackTrace.ToJsonOnScreen().WriteToConsole();
                throw new ArgumentNullException(nameof(name), $"Тип с именем: {name} не найден в {nameof(ServiceFactory)}");
            }
            else
            {
                return ByShortNames[key];
            }
        }
        return ByShortNames[name];
    }


    /// <summary>
    /// Регистрация типов объявленных в сборке
    /// </summary>    
    /// <param name="assembly">сборка</param>
    public ConcurrentQueue<Assembly> Assemblies = new ConcurrentQueue<Assembly>();
    public void AddTypes(IEnumerable<Assembly> assembly, bool softMode = true)
    {
        foreach (Assembly asm in assembly)
            AddTypes(asm);
    }
    public void AddTypes<T>()
    {
 
        AddTypes(typeof(T).Assembly);
    }
    public void AddTypes(Assembly assembly, bool softMode=true)
    {
        //this.Info($"Загружаю сборку: {assembly.GetName()}");
        if (Assemblies.Contains(assembly) == true)
        {
            return;
        }        
        Assemblies.Enqueue(assembly);
        foreach (var ptype in assembly.GetTypes())
        {
            if(!ptype.GetTypeName().StartsWith("<"))
                AddType(ptype, softMode);
        }
    }

    public Task AddTypesAsync(Assembly assembly, bool softMode = true)
    {
        return Task.Run(() => {
            //this.Info($"Загружаю сборку: {assembly.GetName()}");
            if (Assemblies.Contains(assembly) == true)
            {
                return;
            }
            Assemblies.Enqueue(assembly);
            foreach (var ptype in assembly.GetTypes())
            {
                if (!ptype.GetTypeName().StartsWith("<"))
                    AddType(ptype, softMode);
            }
        });
    }


    public void BuildViewTypes()
    {
        
    }

    public void AddType(Type ptype, bool softMode = true)
    {
        if (ByShortNames.ContainsKey(ptype.GetTypeName()))
        {
            if(softMode)
            {
                //this.Warn($"Обнаружено повторное имя типа {ptype.GetTypeName()} в сборках: {ByShortNames[ptype.GetTypeName()].Namespace} и {ptype.Namespace}");
            }
            else
            {
                throw new Exception($"Обнаружено повторное имя типа {ptype.GetTypeName()} в сборках: {ByShortNames[ptype.GetTypeName()].Namespace} и {ptype.Namespace}");
            }                        
        }
        ByShortNames[ptype.Name] = ptype;
        ByShortNames[ptype.GetTypeName()] = ptype;
        ByFullNames[ptype.Namespace + "." + ptype.GetTypeName()] = ptype;
        /*if (ptype.Name.EndsWith("ViewComponent"))
        {
            ViewComponents[ptype.Name] = ptype;
        }
        if (ptype.Name.EndsWith("Controller") && ptype.IsAbstract == false)
        {
            Controllers[ptype.Name] = ptype;
        }
        if (ptype.Name.Contains("`") == false)
        {
            ByShortNames[ptype.Name] = ptype;
            ByFullNames[ptype.FullName] = ptype;
        }

        else if (ptype.Name.StartsWith("<") == false)
        {
            string typeName = ParsePropertyType(ptype);

            ByShortNames[typeName] = ptype;
            ByFullNames[ptype.Namespace + "." + typeName] = ptype;
        }
        ByShortNames[ptype.GetOwnTypeName()] = ptype;
        ByShortNames[ptype.GetTypeName()] = ptype;
        ByShortNames[ptype.Name] = ptype;
        ByFullNames[ptype.Namespace + "." + ptype.GetTypeName()] = ptype;*/



        /*if (ptype.GetTypeName().Contains("Dictionary"))
        {
            ptype.Info(ptype.GetTypeName());
        }*/
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ParsePropertyType(Type propertyType)
    {
        string name = propertyType.Name;
        if (name.Contains("`"))
        {
            string text = propertyType.AssemblyQualifiedName;
            text = text.Substring(text.IndexOf("[[") + 1);
            text = text.Substring(0, text.IndexOf(","));
            name = name.Substring(0, name.IndexOf("`")) + "<" + text.Replace("`1", "").Replace("`2", "") + ">";

        }
        return name;
    }

    /// <summary>
    /// Регистрация типов объявленных в сборке
    /// </summary>
    /// <param name="assembly">сборка</param>
    public void AddTypes(Assembly[] assembly)
    {
        foreach (var ptype in assembly)
        {
            AddTypes(ptype);
        }
    }

    /// <summary>
    /// Создание новоги экземпляра класса конструктором по-умолчанию
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public T CreateWithDefaultConstructor<T>(string typeName)
    {
        Type type = null;
        if (typeName.Contains("."))
        {
            type = TypeForName(typeName);
        }
        else
        {
            type = TypeForShortName(typeName);
        }
        return CreateWithDefaultConstructor<T>(type);
    }


    /// <summary>
    /// Поиск конструктора по-умолчанию
    /// </summary>
    /// <param name="type">Ссылка на тип</param>
    /// <returns>конструктор</returns>
    public static ConstructorInfo GetDefaultConstructor(Type type)
    {
        return (from c in new List<ConstructorInfo>(type.GetConstructors()) where c.GetParameters().Length == 0 select c).FirstOrDefault();
    }


    /// <summary>
    /// Создание новоги экземпляра класса конструктором по-умолчанию
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <returns></returns>
    public T CreateWithDefaultConstructor<T>(Type type)
    {
        ConstructorInfo constructor = GetDefaultConstructor(type);
        if (constructor == null)
        {
            throw new Exception($"Тип {type.Name} не объявляет конструктор по умолчанию");
        }         
        return (T)constructor.Invoke(new object[0]);
    }


    /// <summary>
    /// Возвращает тип по имени
    /// </summary>
    /// <param name="type">имя типа</param>
    /// <returns>ссылка на тип</returns>
    public Type TypeForName(string type)
    {
        if (type.IndexOf("List") == 0 || type.IndexOf("List") == 1)
        {
            return typeof(List<object>);
        }
        if (type.IndexOf(".") == -1)
        {
            return TypeForShortName(type);
        }
        if (type == "Single")
        {
            return typeof(Single);
        }
        if (type == "Nullable<System.DateTime>")
        {
            return typeof(Nullable<DateTime>);
        }
        if (type == "Nullable<System.Single>")
        {
            return typeof(Nullable<float>);
        }
        if (type == "Nullable<System.Int32>")
        {
            return typeof(Nullable<int>);
        }
        if (type == "Nullable<System.Int32>")
        {
            return typeof(Nullable<int>);
        }
        return ByFullNames[type];
    }


    /// <summary>
    /// Возвращает тип по имени
    /// </summary>
    /// <param name="type">имя типа</param>
    /// <returns>ссылка на тип</returns>
    public Type TypeForShortName(string type)
    {
        if (type.IndexOf("List") == 0|| type.IndexOf("List") == 1)
        {
            return typeof(List<object>);
        }
        if (type.IndexOf(".") != -1)
        {
            return TypeForName(type);
        }
        if (ByShortNames.ContainsKey(type)) 
        {
            return ByShortNames[type];
        }
        else
        {
            foreach(var key in ByShortNames.Keys)
            {
                if(key.ToLower() == type.ToLower())
                {
                    return ByShortNames[key];
                }
            }
            return null;
        }        
    }


    
    
    public IEnumerable<Type> GetTypesImplemented<T>()
        => ByShortNames.Values.Where(v => v.IsAbstract == false && v != typeof(T) && !v.IsGenericTypeDefinition && v.IsImplements(typeof(T))).OrderByDescending(p => p.Name);
    public IEnumerable<Type> GetTypesExtended<T>()
        => ByShortNames.Values.Where(v => v.IsAbstract == false && v != typeof(T) && !v.IsGenericTypeDefinition && v.IsExtends(typeof(T))).OrderByDescending(p => p.Name);
    public IEnumerable<Type> GetTypesExtended(Type p)
        => ByShortNames.Values.ToHashSet().Where(v => v != p && v.IsExtends(p) && !v.IsGenericTypeDefinition).ToList();
    public IEnumerable<Type> GetTypesImplemented(Type p)
        => ByShortNames.Values.Where(v => v.IsAbstract == false && v != p && !v.IsGenericTypeDefinition && v.IsImplements(p)).OrderByDescending(p => p.Name);
    public IEnumerable<Type> GetGenericTypes(Type p)
        => ByShortNames.Values.Where(v => v.IsAbstract == false && v != p && v.IsGenericTypeDefinition && v.GetGenericArguments().Any(t => t.IsExtends(p))).OrderByDescending(p => p.Name);

    internal bool Has(string text)
    {
        return ByShortNames.ContainsKey(text);
    }
}

