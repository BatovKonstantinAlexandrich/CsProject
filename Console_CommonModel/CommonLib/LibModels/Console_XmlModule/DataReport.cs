using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// Модель документа представления (отчёт)
/// </summary>
public class DataReport 
{
    public List<object> DataSources { get; set; }  
    public List<object> DataSets { get; set; }
    public List<object> DataRelations { get; set; }    
    public List<object> Parameters { get; set; }
    public List<object> Setup { get; set; }
    public List<object> Body { get; set; }


    public DataReport()
    {
        //logger.LogInformation("CREATED");
    }

    
    /// <summary>
    /// промежуточная память
    /// </summary>
    private ConcurrentDictionary<string, object> Scope = new ConcurrentDictionary<string, object>();


    /// <summary>
    /// объявить переменную и присовить ей значение
    /// </summary>
    public void AddGlobalDeclaration( string key, object value)
    {
        Scope[key] = value;
    }

    /// <summary>
    /// получить значение переменной по ключу
    /// </summary>
    public object Select( string query )
    {
        if (Scope.ContainsKey(query))
        {
            return Scope[query];
        }
        else
        {
            return null;
        }        
    }


    /// <summary>
    /// Связывание выражений прописаных в атрибутах
    /// </summary>
    /// <param name="target"></param>
    /// <param name="attrName"></param>
    /// <param name="attrValue"></param>
    public void BindInterpolate(object target, string attrName, string attrValue)
    {
        //TODO: 
        /*if(target is ViewItem)
        {

            ((ViewItem)target).InterpolationBindings[attrName] = attrValue;
            
        }
        else
        {
            throw new Exception("Выполнить интерполяцию может только компонент представления");
        }*/

    }


    private ILogger logger = LoggerFactory.Create((builder) => { builder.AddConsole(); }).CreateLogger(nameof(DataReport));

}