using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;


public class XmlComponentFactory : IBlazorComponentFactory
{
    public XmlComponentFactory()
    {
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="parent"></param>
    /// <param name="root"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public object Create(
        ComponentParams parameters, 
        ComponentModel parent, 
        DataReport root)
    {
        throw new NotImplementedException();
    }
}


/// <summary>
/// Предоставляет сервисы сконфигурированные согласно окружению
/// </summary>
public class XmlComponentsFactory: IBlazorComponentFactory
{
    /// <summary>
    /// Сериализуемые сборки
    /// </summary>
    private List<Assembly> imported = new List<Assembly>() {
        
        Assembly.GetExecutingAssembly(),
        Assembly.GetCallingAssembly(),
        Assembly.GetEntryAssembly(),
        
        typeof(System.Data.DataColumn).Assembly
    };

    public XmlComponentsFactory()
    {
       
    }



    /// <summary>
    /// Анализ информации полученной из XML документа 
    /// для создание модели компонента представления    
    /// </summary>
    /// <param name="parameters">параметры управляющего потока</param>
    /// <param name="Parent">ссылка на поток данных</param>
    /// <returns></returns>
    public object Create(ComponentParams parameters, ComponentModel Parent, global::DataReport Root)
    {
        object result = null;
        try
        {                        
            if (parameters.Tag is null)
                parameters.Tag = "#text";
            parameters.EnsureIsValide();
            if (parameters.Tag == "button-view")
            {
                int x = 1;
                this.Info(parameters.Tag);
            }
            string TypeName = ToCapitalStyle(parameters.Tag).Filter("#");
            if(TypeName=="ButtonView")
            {
                int x = 2;
                System.Console.WriteLine(1);

            }
                          
            // по-идеи тут мы регистрируем функцию доступа к данному экземпляру
            // но почему-то просто присваиваем ссылки
            if (parameters.GetNameAttribute() != null)
            {
                System.Console.WriteLine(parameters.GetNameAttribute());
                var parentTarget = Parent.GetTarget();
                if (parentTarget == null)
                    throw new Exception("Не проинициализирован родительский компонент для: "+ parameters.Attrs.ToJson());
                Type parentType = parentTarget.GetType();
                string property = ToCapitalStyle(parameters.GetNameAttribute());
                PropertyInfo info = parentType.GetProperty(property);
               
                if(info is not null)
                {
                    object value = info.GetValue(Parent.GetTarget());
                    if (value != null)
                    {
                        return result = value;
                    }
                    else
                    {
                        result = Create(TypeName, parameters.Attrs, Parent, Root);
                        info.SetValue(Parent.GetTarget(), result);
                        return result;
                    }
                    /*result = Create(TypeName, parameters.Attrs, Parent, Root);
                    Root.AddGlobalDeclaration(parameters.GetNameAttribute(), result);
                    return result;*/
                }
                 
            }

            if (Parent != null)
            {

                //если существует свойство с одноимённым идентификатором, то 
                //компонент является указателем на это свойство в случае
                //если свойство определено в единственном числе,
                //если свойство определено во множественном числе( т.е. как коллекция )
                //ссылка будет добавлена в коллекцию
                if (IsCollectionType(Parent.GetTarget()))
                {
                    result = Create(TypeName, parameters.Attrs, Parent, Root);
                    if (Parent.GetTarget() is DataColumnCollection)
                    {
                        DataColumnCollection col = (DataColumnCollection)Parent.GetTarget();
                        col.Add((DataColumn)result);
                        return result;
                    }
                    else
                    {
                        AddCreatedChildToCollection(Parent.GetTarget(), result);
                        return result;

                    }
                }
                PropertyInfo info = Parent.GetTarget().GetType().GetProperty(TypeName);
                if (info != null)
                {
                    if (IsCollectionType(Parent.GetTarget().GetType(), info.Name))
                    {
                        result = Parent.GetTarget().GetType().GetProperty(info.Name).PropertyType.New();
                        info.SetValue(Parent.GetTarget(), result);
                        return result;

                    }
                    else
                    {
                        result = Create(TypeName, parameters.Attrs, Parent, Root);
                        if (Parent.GetTarget().GetType().Name.StartsWith("List"))
                        {
                            AddCreatedChildToCollection(info.GetValue(Parent.GetTarget()), result);
                            return result;
                        }
                        else
                        {

                            info.SetValue(Parent.GetTarget(), result);
                            return result;
                        }

                    }
                }
                else
                {
                    result = Create(TypeName, parameters.Attrs, Parent, Root);
                    if (Parent.GetTarget().GetType().Name.StartsWith("List"))
                    {
                        AddCreatedChildToCollection(Parent.GetTarget(), result);
                        return result;
                    }/*
                    else
                    {
                        throw new Exception("Свойство " + TypeName + " не определено в типе " + Parent.GetTarget().GetType().Name);
                    }*/

                }
            }
            else
            {
                result = Create(TypeName, parameters.Attrs, Parent, Root);
                
            }
        }
        catch(Exception ex)
        {
            //System.Console.WriteLine(JsonConvert.SerializeObject(parameters) + " \n" + GetTags()+ex);
            this.Error("Исключение", ex);
            this.Error($"Исключение при создании объекта: \n => {ex.Message}\n{ex.StackTrace}");
            throw new Exception("Ошибка: "+ 
                JsonConvert.SerializeObject(parameters) + " \n" + GetTags()
            );                      
        }
        finally
        {
            if (result == null && parameters.Tag != "#text")  
                throw new NullReferenceException($"При исп. функции тега {parameters.Tag}. Результат функции не должен вернуть ссылку на Null")  ;
        }
        return result;
    }

    /// <summary>
    /// Обходит dynamic Add(...): для <see cref="ILayoutViewNode"/> нужен строго <see cref="ViewNode"/>, иначе RuntimeBinderException.
    /// </summary>
    private static void AddCreatedChildToCollection(object collectionTarget, object item)
    {
        if (collectionTarget is null)
            throw new ArgumentNullException(nameof(collectionTarget));
        if (collectionTarget is ILayoutViewNode layout)
        {
            if (item is not ViewNode vn)
                throw new InvalidOperationException(
                    $"Элемент разметки XML должен быть ViewNode для контейнера {collectionTarget.GetType().Name}; фактический тип: {item?.GetType().FullName ?? "null"}.");
            layout.Add(vn);
            return;
        }
        if (collectionTarget is IList list)
        {
            list.Add(item);
            return;
        }
        ((dynamic)collectionTarget).Add((dynamic)item);
    }

    private bool IsCollectionType(object p) => Typing.IsCollectionType(p.GetType());

    private object Create(string typeName, IDictionary<string, string> attrs, ComponentModel parent, DataReport root)
    {
        if (typeName != "text")
        {
                        
            var result = typeName.ToType().New();
            result.SetTextValues(attrs);
                

            // IViewNode
            if(result is not null && parent is not null)
            {
                if(result is ViewNode && parent.Target is IViewNode)
                {
                    ((IViewNode)parent.Target).Append((ViewNode)result);
                }
            }
            return result;
          
            
        }
        else
        {
            return null;
        }
    }

    private string ToCapitalStyle(string tag) => tag.ToCapitalStyle();

    private void WriteLine(object v) => LoggerFactory.Create((builder) => { builder.AddConsole(); }).CreateLogger(GetType().GetTypeName()).LogInformation(v.ToString());

    private bool IsCollectionType(Type type, string name)
    {
        var propertyType = type.GetProperty(name);
        if (Typing.IsPrimitive(propertyType.PropertyType) == true)
            return false;
        var propertyInterfaces = propertyType.PropertyType.GetInterfaces();
        var interfacesNames = propertyInterfaces.Select(i => i.Name);        
        return interfacesNames.ToHashSet().Contains(nameof(IEnumerable));
    }

    
    /// <summary>
    /// Определяем признак числено
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsCollectionType(Type type)
    {
        Type p = type;
        while (p != typeof(Object) && p != null)
        {
            if ((from pinterface in new List<Type>(p.GetInterfaces()) where pinterface.Name.StartsWith("ICollection") select p).Count() > 0)
            {
                return true;
            }
            p = p.BaseType;
        }
        return false;
    }

    /// <summary>
    /// Получение значения из свойства с указанным именем
    /// </summary>
    /// <param name="ключ"></param>
    /// <param name="значение"></param>
    /// <returns></returns>
    public object GetValue(object ключ, string значение)
    {
        PropertyInfo propertyInfo = ключ.GetType().GetProperty(значение);
        FieldInfo fieldInfo = ключ.GetType().GetField(значение);
        return
            fieldInfo != null ? fieldInfo.GetValue(ключ) :
            propertyInfo != null ? propertyInfo.GetValue(ключ) :
            null;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="PropertyName"></param>
    /// <param name="Attrs"></param>
    /// <param name="Parent"></param>
    /// <returns></returns>
    private object Create(string PropertyName, Dictionary<string, string> Attrs, ComponentModel Parent, global::DataReport Root)
    {
        
        Type type = FindTypeForShortName(PropertyName);
        if (type == null)
        {
            throw new Exception("Не найден класс с именем " + PropertyName);
        }
        else
        {            
            object Target = CreateWithDefaultConstructor<object>(type);
            Target = Apply(Target, Attrs, Parent, Root);
            return Target;
        }
        throw new Exception("Не найден класс с именем " + PropertyName);
    }


    /// <summary>
    /// Получаем новый экземпляр исп. конструктор по умолчанию
    /// </summary>
    private T CreateWithDefaultConstructor<T>(Type type)
    {
        var constructor = type.GetConstructors().FirstOrDefault();
        if (constructor==null)
            throw new Exception("Требуется реализовать конструктор типа "+type.Name);
        //constructor.GetParameters()
        return (T)constructor.Invoke(new object[0]);
    }


    /// <summary>
    /// Поиск типа в сборках звестных приложению
    /// </summary>
    /// <param name="typeName"></param>
    /// <returns></returns>
    private Type FindTypeForShortName(string typeName)
    {
        foreach(var mod in imported)
        {
            Type t = (from p in mod.GetTypes() where p.Name == typeName select p).SingleOrDefault();         
            if (t != null)
            {
                return t;
            }
        }
        throw new Exception($"Не найден тип "+ typeName);   
    }


    /// <summary>
    /// Ключи для компиляции
    /// </summary>
    private string[] GetTags( )
    {

        var tags = new List<string>();
        foreach (var mod in imported)
        {
            WriteLine(((from p in mod.GetTypes() where IsEng(p.Name) select ToKebabStyle(p.Name)).ToList()));
           //tags.AddRange().ToList()); ;
        }
        return tags.ToArray();
    }

    private string ToKebabStyle(string name)
    {
        return name;
    }

    private bool IsEng(string name)
    {
        return true;
    }


    /// <summary>
    /// Создание предварительных параметров связывания
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attrs"></param>
    /// <param name="parent"></param>
    private static object Apply(object target, Dictionary<string, string> attrs, ComponentModel parent, DataReport Root)
    {
        if(attrs != null)
        {

            System.Console.WriteLine(target);
            /*if (target is ViewItem)
            {
                ((ViewItem)target).Bindings = Collections.Expect(attrs, "name", "id", "Name", "Id");
                //((ViewItem)target).Init();
                ((ViewItem)target).Compile();
            }*/
            
            /*
            if (target.GetType().Name == nameof(AgregatedDataSet))
            {
                target.GetType().GetProperty("TableName").SetValue(target, attrs["TableName"]);
            }
            
            if (target.GetType().Name == "OdbcDataSource")
            {
                OdbcDataSource ds = ((OdbcDataSource)target);
                ds.Alias = attrs["Alias"];
                ds.connectionString = attrs["ConnectionString"];

            }
            if (target.GetType().Name == "DataTable")
            {
                string TableName = attrs["TableName"];
                string DataSource = attrs["DataSource"];
                object pds = (from p in Root.DataSources where ((OdbcDataSource)p).Alias == DataSource select p).SingleOrDefault();
                OdbcDataSource ds = ((OdbcDataSource)pds);
                System.Data.DataTable table = ds.CreateDataTable("select * from "+ TableName);
                table.TableName = TableName;
                return table;
            }
            if ( target.GetType().Name=="DataList" )
            {
                int x = 0;
                string dataset = attrs["Dataset"];
                string bind = attrs["Bind"];
                object table = (from p in Root.DataSets where ((DataTable)p).TableName == dataset select p).SingleOrDefault();
                if(table is AgregatedDataSet)
                {
                    ((DataList)target).Dataset = ((AgregatedDataSet)table).GetData();
                    ((DataList)target).Bindings = JsonConvert.DeserializeObject<Dictionary<string, string>>(bind);
                }
                else
                {
                    System.Data.DataTable dataTable = ((DataTable)table);
                    JArray jarray = convert(dataTable);
                    ((DataList)target).Dataset = jarray;
                    ((DataList)target).Bindings = JsonConvert.DeserializeObject<Dictionary<string, string>>(bind);
                }
            }*/
        }        
        return target;
    }



    /// <summary>
    /// Вытягиваем данные из 
    /// </summary>
    public JArray ReadData(System.Data.DataTable dataTable)
    {
        Dictionary<string, object> resultSet = new Dictionary<string, object>();
        List<Dictionary<string, object>> listRow = new List<Dictionary<string, object>>();
        foreach (DataRow row in dataTable.Rows)
        {
            Dictionary<string, object> rowSet = new Dictionary<string, object>();
            foreach (DataColumn column in dataTable.Columns)
            {
                rowSet[column.Caption] = row[column.Caption];
            }
            listRow.Add(rowSet);
        }
        resultSet["rows"] = listRow;

        JObject jrs = JObject.FromObject(resultSet);
        return (JArray)jrs["rows"];
    }
}

