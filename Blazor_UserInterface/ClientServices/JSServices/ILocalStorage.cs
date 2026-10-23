using System.Collections.Generic;
using System.Threading.Tasks;

public interface ILocalStorage
{
    Task<TResult> GetItem<TResult>(string Key);
    Task<List<string>> GetKeys();
    Task SetItem(string Key, string Value);
}