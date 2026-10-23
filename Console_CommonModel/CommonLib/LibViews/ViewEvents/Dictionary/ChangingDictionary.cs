using Console_CommonModel.CommonLib.LibViews.ViewEvents.Dictionary;

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

/// <summary>
/// Словарь с поддержкой событий изменений.
/// </summary>    
public class ChangingDictionary<K,V> : IDictionary<K,V> where V: class
{
    /// <summary>
    /// Обработчик событий изменения
    /// </summary>    
    public event EventHandler<ChangingDictionaryEvent<K,V>> OnChanged = (s, e) => { };

    /// <summary>
    /// Потокобезопасная коллекция
    /// </summary>
    private ConcurrentDictionary<K, V> _dictionary = new();

    public ChangingDictionary()
    {
    }

    public ChangingDictionary(ConcurrentDictionary<K, V> dictionary)
    {
        _dictionary = dictionary;
    }

    public ChangingDictionary([NotNullNotEmpty]IEnumerable<KeyValuePair<K, V>> dictionary)
    {
        
        _dictionary = new(dictionary.ToList());
    }

    public void PushEvent(ChangingDictionaryMessage<K, V> message)
    {        
        OnChanged.Invoke(this, new ChangingDictionaryEvent<K, V>(message));
    }

    public void Add(K key, V value)
    {
        ((IDictionary<K, V>)_dictionary).Add(key, value);
        this.PushEvent(new()
        {
            Action = "removed",
            Key = key,  
            Value = value
        });
    }

    public bool ContainsKey(K key)
    {
        return ((IDictionary<K, V>)_dictionary).ContainsKey(key);
    }

    public bool Remove(K key)
    {
        V val = ((IDictionary<K, V>)_dictionary).ContainsKey(key)?
            ((IDictionary<K, V>)_dictionary)[key]: null;
        var result = ((IDictionary<K, V>)_dictionary).Remove(key);
        if(result)
        {
            this.PushEvent(new()
            {
                Action = "removed",
                Key = key,
                Value = val
            });
        }            
        return result;
    }

    public bool TryGetValue(K key, [MaybeNullWhen(false)] out V value)
    {
        return ((IDictionary<K, V>)_dictionary).TryGetValue(key, out value);
    }
        
    public V this[K key] 
    { 
        get => ((IDictionary<K, V>)_dictionary)[key];
        set
        {
            if(((IDictionary<K, V>)_dictionary).ContainsKey(key))
            {
                ((IDictionary<K, V>)_dictionary)[key] = value;
                this.PushEvent(new()
                {
                    Action = "updated",
                    Key = key,
                    Value = value
                });
            }
            else
            {
                ((IDictionary<K, V>)_dictionary)[key] = value;
                this.PushEvent(new()
                {
                    Action = "added",
                    Key = key,
                    Value = value
                });
            }                                
        }
    }

    public System.Collections.Generic.ICollection<K> Keys => 
        ((IDictionary<K, V>)_dictionary).Keys;
    public System.Collections.Generic.ICollection<V> Values => 
        ((IDictionary<K, V>)_dictionary).Values;

    public void Add(KeyValuePair<K, V> item)
    {
        ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).Add(item);
        this.PushEvent(new()
        {
            Action = "added",
            Key = item.Key,
            Value = item.Value
        });
    }

    public void Clear()
    {
        lock (_dictionary)
        {
            foreach (var key in _dictionary.Keys.ToList())
            {
                this.Remove(key);
            }
        }
          
    }

    public bool Contains(KeyValuePair<K, V> item)
    {
        return ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).Contains(item);
    }

    public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
    {
        ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<K, V> item)
    {           
        var result = ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).Remove(item);
        if (result)
        {
            this.PushEvent(new()
            {
                Action = "removed",
                Key = item.Key,
                Value = item.Value
            });
        }
        return result;
    }

    public int Count => ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).Count;
    public bool IsReadOnly => ((System.Collections.Generic.ICollection<KeyValuePair<K, V>>)_dictionary).IsReadOnly;

    public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<K, V>>)_dictionary).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }

    internal void SetData(Dictionary<K, V> dictionary)
    {
        this.Clear();
        foreach (var kv in dictionary)
        {
            this[kv.Key] = kv.Value;
        }
    }
}

/*public class ChangingList<T> : Component, IChangingList<T>
{

    public ChangingList items = new();
    public event EventHandler<ListChangedEvent> OnChanged = (s, e) => { };
    public ChangingList() { }
    public ChangingList(IEnumerable<T> items)
    {
        this.items = new();
        this.items.AddRange(items);
        this.items.OnChanged += OnChanged;
    }





    public void Clear()
    {
        items.Clear();
    }

    public bool Contains(T item) => items.Contains(item);



    public bool Remove(T item) => items.Remove(item);

    public int Count => items.Count();

    public bool IsReadOnly => false;

    public IEnumerator<T> GetEnumerator() => new List<T>(items.Select(item => (T)item).ToList()).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

    public int IndexOf(T item) => items.IndexOf(item);

    public void Insert(int index, T item) => items.Insert(index, item);

    public void RemoveAt(int index) => items.RemoveAt(index);
    public void Add(T item) => items.Add(item);
    public void AddRange(params object[] vals) => items.AddRange(vals);
    public void CopyTo(T[] array, int arrayIndex) => items.Select(item => (T)item).ToList().CopyTo(array, arrayIndex);

    public T this[int index] { get => (T)items[index]; set => items[index] = value; }

    public void Bind<Model>(IChangingList<Model> dataset, Func<T, Model> convert)
    {
        if (convert is null)
            throw new ArgumentNullException(nameof(convert));
        if (dataset is null)
            throw new ArgumentNullException(nameof(dataset));
        if (dataset.Count() > 0)
            throw new ArgumentException(nameof(dataset), "Коллекция должна быть пустая");
        this.OnChanged += (s, e) => {
            switch (e.Target.Action)
            {
                case "added":
                    dataset.Add(convert((T)e.Target.Item));
                    break;
                case "removed":
                    dataset.Add(convert((T)e.Target.Item));
                    break;
            }
        };
    }

    public IChangingList<Model> CreateBindingList<Model>( Func<T, Model> convert)
    {
        var results = new ChangingList<Model>();
        Action<T> add = (item) => results.Add(convert(item));
        this.ToList().ForEach(add);
        if (convert is null)
            throw new ArgumentNullException(nameof(convert));         
        this.OnChanged += (s, e) => {
            switch (e.Target.Action)
            {
                case "added":
                    results.Add(convert((T)e.Target.Item));
                    break;
                case "removed":
                    results.Add(convert((T)e.Target.Item));
                    break;
            }
        };
        return results;
    }
}


*/

