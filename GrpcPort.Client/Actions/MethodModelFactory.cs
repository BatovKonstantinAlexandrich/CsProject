using System.Reflection;
using GrpcPort.Contracts.Actions;

using WinFormsApp.Console.Console_ClassModule.ClassDataModel;

namespace GrpcPort.Client.Actions;

internal static class MethodModelFactory
{
    public static MethodModel FromMethod(MethodInfo method)
    {
        return new MethodModel
        {
            Name = method.Name,
            Type = method.ReturnType.Name,
            Label = method.Name,
            Description = $"{method.DeclaringType?.Name}.{method.Name}",
            Parameters = method.GetParameters().Select(p => new MethodParameterModel
            {
                Name = p.Name ?? $"arg{p.Position}",
                Type = p.ParameterType.Name                
            }).ToList()
        };
    }
}
