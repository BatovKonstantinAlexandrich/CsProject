using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TextIOExtensions
{

    /// <summary>
    /// Получение списка файлов
    /// </summary>    
    public static List<string> GetFiles(this string path)
    {
        return new List<string>(System.IO.Directory.GetFiles(path));
    }

    /// <summary>
    /// Вывод в консоль
    /// </summary>    
    public static string WriteToConsole(this string path)
    {
        Writing.ToConsole(path);
        return path;
    }
    

    /// <summary>
    /// Получение списка файлов
    /// </summary>    
    public static string WriteToFile(this string text, string path)
    {
        System.IO.File.WriteAllText(path,text);
        return text;
    }

    /// <summary>
    /// Получение списка файлов
    /// </summary>    
    public static string ReadText(this string path)
    {
        return System.IO.File.ReadAllText(path);
    }
}