using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ViewFactoryExtensions
{

    /// <summary>
    /// Получение аргументов выполнения метода из
    /// формы ввода пользоватлелем
    /// </summary>    
    public static object[] GetUserInputParameters(this System.Reflection.MethodInfo m)
    {
        
        var inputFormModel = m.DeclaringType.GetInputFormModel(m.Name);
        var controlFormModel = inputFormModel.ToControl();
        List<object> args = new();
        foreach (var name in m.GetParameters().Select(p => p.Name))
        {
            args.Add(inputFormModel.FormFields.FirstOrDefault(f => f.Name == name).Value);
        }
        return args.ToArray();
    }
}

