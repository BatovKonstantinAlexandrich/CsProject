using MySqlX.XDevAPI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Создание DMl запросов
/// </summary>
public interface IDMLFactory
{
    string CreateInsertStatement(string schema, string table, Dictionary<string, object> values);
    string CreateUpdateStatement(string schema, string table, Dictionary<string, object> values);
}
 