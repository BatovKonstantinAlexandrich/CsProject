
using System.Collections.Concurrent;
using System.Collections.Generic;

public interface APIActiveCollection<T> where T : ActiveObject
{
    void DoCheck();
    string Find(T current);
    ConcurrentDictionary<string, object> GetSession(string key);
    ConcurrentDictionary<string, T> GetMemory();
    T Take(string token);
    T Remove(string key);
    string Put(T user);
    bool Has(string key);
}
