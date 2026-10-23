

using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.Views;

[ViewUsageExample("var view = new ArrayView();\nview.Dock = \"Top\";\nparent.Add(view);", "<array-view Dock=\"Top\" />")]
public class ArrayView<T>  : IList<T>
{
    private MyList items = new();

    public ArrayView(IEnumerable<T> items)
    {
        items.ToList().ForEach(p => this.items.Add((object)p));
    }

    public void AddRange(IEnumerable<T> items)
    {
        items.ToList().ForEach(this.Add);
    }
    public int IndexOf(T item)
    {
        return ((IList<object>)items).IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        ((IList<object>)items).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<object>)items).RemoveAt(index);
    }

    public T this[int index] { get => (T)((IList<object>)items)[index]; set => ((IList<object>)items)[index] = value; }

    public void Add(T item)
    {
        ((System.Collections.Generic.ICollection<object>)items).Add(item);
    }

    public void Clear()
    {
        ((System.Collections.Generic.ICollection<object>)items).Clear();
    }

    public bool Contains(T item)
    {
        return ((System.Collections.Generic.ICollection<object>)items).Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        ((System.Collections.Generic.ICollection<object>)items).Select(p => (T)p).ToArray().CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        return ((System.Collections.Generic.ICollection<object>)items).Remove(item);
    }

    public int Count => ((System.Collections.Generic.ICollection<object>)items).Count;

    public bool IsReadOnly => ((System.Collections.Generic.ICollection<object>)items).IsReadOnly;

    public IEnumerator<T> GetEnumerator()
    {
        return ((IEnumerable<T>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)items).GetEnumerator();
    }


    /*public static bool operator < (MyList<T> a, MyList<T> b)
    {
        return a.Count() > b.Count();
    }
    public static bool operator > (MyList<T> a, MyList<T> b)
    {
        return a.Count() > b.Count();
    }
    public static MyList<T> operator !=(MyList<T> a, IEnumerable<T> items)
    {
        return null;
    }
    public static MyList<T> operator ==(MyList<T> a, IEnumerable<T> items)
    {
        return null;
    }  */
}


[Label("Список")]
[ViewUsageExample("var view = new ArrayView();\nview.Dock = \"Top\";\nparent.Add(view);", "<array-view Dock=\"Top\" />")]
public class MyList : ViewItem, IList<object>
{
    private readonly List<object> _items;
    public int Count => _items.Count();
    public bool IsReadOnly => false;


    private event EventHandler OnListChanges;


    public object Upgrade(object item)
    {
        this.Info("OnUpgrade");
        return item;
    }


    public MyList() : base()
    {
        _items = new List<object>() { };
        //_items.Add(new ListItem() { Title = "Item1" });
        //_items.Add(new ListItem() { Title = "Item2" });
        OnListChanges = new EventHandler((sender, args) => {
            IChangingListEvent message = ((IChangingListEvent)args);
            IChangingListMessage changes = message.GetChanges();
            this.Info($"Изменения {changes.Action} {changes.Item}");
        });
    }

    public MyList(List<object> collection) : base()
    {
        _items = new List<object>(collection);
    }





    public void Add(object item)
    {
        _items.Add(item);
        this.OnAdded(item);
    }
    public void OnAdded(object item)
    {
        Changed = true;
        this.Info($"Added {item}");
        this.OnListChanges.Invoke(this, new IChangingListEvent(new IChangingListMessage()
        {
            Item = item,
            Action = "Add"
        }));
    }


    public void Insert(int index, object item)
    {
        _items.Insert(index, item);
        this.OnInserted(index, item);
    }
    public void OnInserted(int index, object item)
    {
        Changed = true;
    }


    public bool Remove(object item)
    {
        bool removed = _items.Remove(item);
        if (removed)
        {
            OnRemoved(item);
        }
        return removed;
    }
    public void OnRemoved(object item)
    {
        Changed = true;
        this.Info($"Removed {item}");
    }

    public void RemoveAt(int index)
    {
        _items.RemoveAt(index);

        this.OnRemovedAt(index);
    }
    public void OnRemovedAt(int index)
    {
        Changed = true;
        _items.RemoveAt(index);
    }

    public void Clear()
    {
        lock (_items)
        {
            object[] arr = _items.ToArray();
            foreach (object item in arr)
            {
                this.Remove(item);
            }
        }
    }


    public IEnumerator<object> GetEnumerator()=> new MyEnumerator<object>(this);
    

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new MyEnumerator<object>(this);
    }

    public object this[int index]
    {
        get => GetItem(index);
        set => SetItem(index, value);
    }

    public object GetItem(int i)
    {
        return this._items[i];
    }

    public void SetItem(int i, object value)
    {
        this._items[i] = value;
    }




    public int IndexOf(object item)
    {
        return _items.IndexOf(item);
    }











    public bool Contains(object item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(object[] array, int arrayIndex)
    {
        object[] arr = _items.ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            array[i] = arr[i];
        }
    }
}
