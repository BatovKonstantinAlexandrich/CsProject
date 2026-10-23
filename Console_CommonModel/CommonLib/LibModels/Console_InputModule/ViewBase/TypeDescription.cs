using Console_CommonModel.CommonLib.Others;

using Namotion.Reflection;

using System.Collections.Concurrent;
using System.Reflection;
 


[Label("Документация по типу")]
//[Label("Базовый класс для атрибутов ввода")]
//[Description("Используется как для валидации данных, так и для ввода.")]
//[HelpMessage("Используйте в качестве базового класса для реализации атрибутов валидации и внедрения элементов управления.")]
//[Icon("home")]
public class TypeDescription : AttributesData
{
    public static TypeDescription Get<TModel>() => new TypeDescription(typeof(TModel));
    public static TypeDescription Get(Type p) => new TypeDescription(p);
    public static IDictionary<Type, TypeDescription> GetDescriptions(Assembly assembly)
    {
        ConcurrentDictionary<Type, TypeDescription> results = new();

        var description = new TypeDescription(typeof(TypeDescription));
        description.InfoTime("Получение документации", () => {
            var doc = new TypeDescription(typeof(TypeDescription));
            var classTypes = assembly.GetClassTypes();
            int total = classTypes.Count();
            List<Task> tasks = new();
            foreach (var ptype in classTypes)
            {
                tasks.Add(Task.Run(() => {
                    try
                    {
                        doc.InfoTime(ptype.GetTypeName(), () => {
                            results[ptype] = Get(ptype);
                        });
                    }
                    catch (Exception ex)
                    {
                        InputConsole.Error($"{ptype.GetTypeName()} => {ex.Message}");
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

        });
        return results;
    }
    public static IDictionary<Type, Dictionary<string, List<string>>> ValidateTypes() => ValidateTypes(typeof(TypeDescription).Assembly);
    public static IDictionary<Type, Dictionary<string, List<string>>> ValidateTypes(Assembly assembly)
    {
        ConcurrentDictionary<Type, Dictionary<string, List<string>>> messages = new();
        var description = new TypeDescription(typeof(TypeDescription));
        description.InfoTime("Получение документации", () => {
            var doc = new TypeDescription(typeof(TypeDescription));
            var classTypes = assembly.GetClassTypes();
            int total = classTypes.Count();
            List<Task> tasks = new();
            foreach (var ptype in classTypes)
            {
                tasks.Add(Task.Run(() => {
                    try
                    {
                        doc.InfoTime(ptype.GetTypeName(), () => {
                            var documentation = Get(ptype);
                            messages[ptype] = documentation.Validate();
                        });
                    }
                    catch (Exception ex)
                    {
                        InputConsole.Error($"{ptype.GetTypeName()} => {ex.Message}");
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

        });
        return messages;
    }

    

    [Label("Подсказка")]
    [NotNullNotEmpty]
    public string? Help { get; internal set; }


    [Label("Описание")]
    [NotNullNotEmpty]
    public string? Description { get; internal set; }


    [Label("Надпись")]
    [NotNullNotEmpty]
    public string? Label { get; internal set; }


    [Label("Иконка")]
    [NotNullNotEmpty]
    public string? Icon { get; internal set; }


    private Dictionary<string, AttributesData> Properties = new();
    private Dictionary<string, AttributesData> Methods = new();

    public TypeDescription(object target): base(target)
    {
        foreach (var name in target.GetOwnInputPropertyNames())
        {
            Properties[name] = new AttributesData(target.GetPropertyAttributes(name));
        }
        foreach (var name in target.GetOwnMethodNames())
        {
            Methods[name] = new AttributesData(target.GetMethodAttributes(name));
        }
    }

    public Dictionary<string, List<string>> ValidateOptional()
    {
        Dictionary<string, List<string>> results = new();
        foreach (var kv in this.Properties)
        {
            foreach (var pkv in kv.Value.Validate())
            {
                results[$"{kv.Key}.{pkv.Key}"] = pkv.Value;
            }
        }
        foreach (var kv in this.Methods)
        {
            foreach (var pkv in kv.Value.Validate())
            {
                results[$"{kv.Key}.{pkv.Key}"] = pkv.Value;
            }
        }
        return results;
    }
}