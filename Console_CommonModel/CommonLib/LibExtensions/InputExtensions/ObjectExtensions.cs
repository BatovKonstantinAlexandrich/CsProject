using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


public static class ObjectExtensions
{
    public static List<string> GetCommands(this object target)
    {
        return target.GetType().GetMethods().Select(m => $"{m.Name}{CreateParametersQuery(m)}").ToList();
    }

    private static object CreateParametersQuery(MethodInfo m)
    {
        string q = "?";
        foreach (var p in m.GetParameters())
        {
            q += $"{p.Name}=" + "{" + p.Name + "}&";
        }
        return q;
    }

    public static object InvokeGenericMethod(this object target, string name, List<Type> geneticTypes, List<object> args)
    {
        MethodInfo method = target.GetType().GetMethods().First(m => m.Name == name);
        method = method.MakeGenericMethod(geneticTypes.ToArray());

        object[] pars = args.ToArray();
        return method.Invoke(target, pars);
    }
    public static object InvokeGenericMethod(this object target, string name, List<Type> geneticTypes, Dictionary<string, object> args)
    {
        MethodInfo method = target.GetType().GetMethods().First(m => m.Name == name);
        method = method.MakeGenericMethod(geneticTypes.ToArray());

        object[] pars = method.GetParameters().Select(par => args[name]).ToArray();
        return method.Invoke(target, pars);
    }

    public static T Cast<T>(this object target) => (T)target;
    public static object Invoke(this object target, string name, Dictionary<string, object> args)
    {
        var method = target.GetType().GetMethods().First(m => m.Name == name);
        object[] pars = method.GetParameters().Select(par => args[name]).ToArray();
        return method.Invoke(target, pars);
    }

    public static object Invoke(this object target, string name, Dictionary<string, string> args)
    {
        var method = target.GetType().GetMethods().First(m => m.Name == name);
        object[] pars = method.GetParameters().Select(par => TextDataSetter.FromText(args[name], par.ParameterType.GetTypeName())).ToArray();
        return method.Invoke(target, pars);
    }

    

    public static ActionResponse Invoke(this object target, ActionRequest request)
    {        
        var result = target.Invoke(request.ActionName, request.MessageObject);
        return new ActionResponse()
        {
            RequestMessage = request,
            SerialKey = request.SerialKey,
            ActionSuccess = true,
            ActionResult = result 
        };
    }

    public static object Invoke(this object target, string name, object[] args)
    {
         var methods = target.GetType().GetMethods();
        //target.Info(methods.Select(m => m.Name).ToList().ToJsonOnScreen());
        var method = methods.First(m => m.Name == name);
        object[] pars = args;
        return method.Invoke(target, pars);
    }
    public static object Invoke(this object target, string name, IEnumerable<object> args)
    {
        try
        {
            var ptype = target.GetType();
            target.Info($"{ptype.GetTypeName()}.{name}({args.Count()})");
            var methods = ptype.GetMethods();
            //target.Info(methods.Select(m => m.Name).ToJsonOnScreen());
            MethodInfo method = null;
            foreach(var m in methods)
            {
                //target.Info(new { p1 = m.Name, p2 = name }.ToJsonOnScreen());
                if (m.Name == name)
                {
                    method = m;
                    break;
                }
                
            }
            
            if (method is null)
                throw new ArgumentException(nameof(name), $"Не найден метод с именем {name} в типе {target.GetType().GetTypeName()}");
            if (method.GetParameters().Count() != args.Count())
            {
                throw new Exception($"Передано неправильное количество аргументов для вызова метода с именем {name} в типе {target.GetType().GetTypeName()}");
            }
            return method.Invoke(target, args.ToArray());
        }
        catch(Exception ex)
        {
            string text = "";  foreach (var arg in args) text += $",{arg}"; text = text.Length>0? text.Substring(1): text;
            throw new Exception($"Не удалось выполнить рефлексивный вызов: {target.GetTypeName()}.{name}({text})",ex);
        }
    }
  
}

