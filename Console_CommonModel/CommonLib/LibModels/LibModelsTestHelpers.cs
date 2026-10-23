using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Console_CommonModel.CommonLib.LibModels;

internal static class LibModelsTestHelpers
{
    internal static int CountDbSetProperties(Type dbContextType) =>
        dbContextType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Count(p =>
                p.PropertyType.IsGenericType &&
                p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

    internal static void LogDbSetSummary(this TestingElement test, Type dbContextType, string contextLabel)
    {
        var count = CountDbSetProperties(dbContextType);
        test.LogMessage($"{contextLabel}: обнаружено свойств DbSet — {count}");
    }
}
