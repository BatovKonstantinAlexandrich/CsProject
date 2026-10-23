using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Расширения для выполнения HTTP-запросов
/// </summary>
public static class TextHttpExtensions
{

    /// <summary>
    /// Выполнение HTTP-запроса методом GET
    /// </summary>
    public static string Get(this string query)
    {
        var result = new HttpClient().GetAsync(query).Result;
        return result.Content.ReadAsStringAsync().Result;
    }
}