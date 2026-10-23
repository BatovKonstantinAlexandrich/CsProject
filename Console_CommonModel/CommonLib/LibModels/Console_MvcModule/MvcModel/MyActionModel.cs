using Newtonsoft.Json;


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

/// <summary>
/// Модель параметров вызова удаленной процедуры
/// </summary>
public static class MyActionModelExtensions
{
    public static IEnumerable<string> GetOwnParameters(this MethodInfo p)
        => p.GetParameters().Select(p => TypeExtensions2.GetTypeName(p.ParameterType));

    public static IEnumerable<string> GetMethodParameters(this Type p, string name)
        => p.GetOwnMethod(name).GetParameters().Select(p => p.Name );

    public static MyActionModel GetActionModel(this Type p, string name)
    {
        var info = p.GetOwnMethod(name);

        
        var result = new MyActionModel()
        {
            Name = info.Name,
            Type = TypeExtensions2.GetTypeName(info.ReturnType),
            Attributes = p.GetMethodAttributes(name),
            Method = "POST"            
        };
        return result;
    }

}

[Label("Бизнес функция")]
public class MyActionModel: HierTable<MyControllerModel>
{

    [NotMapped]
    public int? ControllerId { get => this.ParentId; }    

    /// <summary>
    /// Http-метод
    /// </summary>

    [Label("Наименование")]
    [InputEngWord("Использщуйте идентьификаторы написанные латиницей")]
    public override string Name { get; set; } = GetProcessName();

    private static string GetProcessName()
    {
        return Process.GetCurrentProcess().ProcessName;
    }
  
    [Label("Http метод")]
    [InputSelect("GET,POST,PUT,OPTIONS,PATCH")]
    public string Method { get; set; } = "POST";

    [Label("Тип возвращаемого значения")]    
    public string Type { get; set; } = "object";

    [Label("Тип возвращаемого значения")]
    [NotInput]
    [NotMapped]
    public object Result { get; set; }



    [NotInput()]
    [NotMapped]    
    public IDictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();

    [NotInput()]
    [NotMapped]
    public IDictionary<string, MyParameterDeclarationModel> Parameters { get; set; } = new Dictionary<string, MyParameterDeclarationModel>();


}
