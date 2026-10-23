using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetPages(this Assembly assembly, string PagesNameSpace)
    {        
        return assembly.GetTypes().Where(t => t.Namespace == PagesNameSpace && t.IsAutoClass == false && t.IsClass == true && t.Name.Contains("+") == false && t.IsClass == true && t.Name.Contains("<") == false).ToList();
    }

    public static Assembly LoadAssembly(this string path)
    {
        return Assembly.LoadFile(path);
    }

    /// <summary>
    /// Метод получения контроллеров объявленных в сборке, находящейся в файле по заданному адресу
    /// </summary>
    /// <param name="filename"> адрес файла сборки </param>
    /// <returns> множество контроллеров </returns>
    public static HashSet<Type> GetControllers(this Assembly assembly)
    {
        return assembly.GetTypes().Where(t => Typing.IsExtendedFrom(t, "ControllerBase")).ToHashSet();
    }
}