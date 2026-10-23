
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

public static class FunctionExtensions
{

    /// <summary>
    /// Создание ссылок на метод действия
    /// </summary>
    public static Action<T1, T2> CreateFunction<T1, T2>(this object context, string expression)
    {
        return (a1, a2) =>
        {
            try
            {
                context.Info(expression);
                var result = context.Compile(expression);
            }
            catch (Exception ex)
            {
                context.Error($"Исключение при выполнении выражения: {expression} => {ex.Message}");
            }
        };
    }



    /// <summary>
    /// Создание ссылок на метод действия
    /// </summary>
    public static Func<Dictionary<string, object>, TResult> CreateFunction<TResult>(this object context, string expression)
    {
        var ctrl = context;
        return (Dictionary<string,object> input) => {
            try
            {
                ctrl.Info(input.Interpolate(expression));
                var result = ctrl.Compile(input.Interpolate(expression));
                return (TResult)result;
            }
            catch (Exception ex)
            {
                context.Error($"Исключение при выполнении выражения: {expression} => {ex.Message}");
                throw new Exception($"Исключение при выполнении выражения: {expression} => {ex.Message}");
            }
        };
    }
}