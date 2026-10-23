using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ZstdSharp;


public static class CollectionsExtensions
{
    public static T New<T>(this Dictionary<string,object> data)
        where T : class
    {
        T p = typeof(T).New<T>();
        foreach(var kv in data)
        {
            p.SetValue(kv.Key, kv.Value);
        }        
        return p;
    }
    public static IEnumerable<T> AddCollection<T>(this IEnumerable<T> items, IEnumerable<T> another)
    {
        var list = items.ToList();
        list.AddRange(another);
        return list;
    }

    public static Dictionary<K,V> AddRange<K,V>(this Dictionary<K, V> items, Dictionary<K, V> another)
    {
        foreach(var kv in another)
        {
            items[kv.Key] = kv.Value;
        }
        return items;
    }

    /// <summary>
    /// Воздание справочника
    /// </summary>
    public static List<TItem> SortItems<TItem>(this List<TItem> items, Func<TItem, TItem, int> reduce)
    {
        items.Sort((t1, t2) => { 
            return reduce(t1, t2);
        });
        return items;
    }
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TKey> items, Func<TKey, TValue> reduce)
    {
        return new Dictionary<TKey, TValue>(items.Select(item => new KeyValuePair<TKey, TValue>(item, reduce(item))));
    }
    public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer) where TKey : notnull
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (keySelector == null)
        {
            throw new ArgumentNullException(nameof(keySelector));
        }

        int capacity = 0;
        if (source is IEnumerable<TSource> collection)
        {
            capacity = ((IEnumerable<TSource>)collection).Count();
            if (capacity == 0)
            {
                return new Dictionary<TKey, TSource>(comparer);
            }

            if (collection is TSource[] array)
            {
                return ToDictionary(array, keySelector, comparer);
            }

            if (collection is List<TSource> list)
            {
                return ToDictionary(list, keySelector, comparer);
            }
        }

        Dictionary<TKey, TSource> d = new Dictionary<TKey, TSource>(capacity, comparer);
        foreach (TSource element in source)
        {
            d.Add(keySelector(element), element);
        }

        return d;
    }

}

