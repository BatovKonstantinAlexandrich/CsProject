using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibViews.ViewEvents.Views;

namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels
{

    [Label("Список")]
    public class MyListViews : MyListView<ViewItem> { }
    public class MyListView<T> : ViewItem, IList<T>
    {
        private readonly List<T> _items;
        public int Count => _items.Count();
        public bool IsReadOnly => false;


        public event EventHandler OnListChanged;


        public T Upgrade(T item)
        {
            this.Info("OnUpgrade");
            return item;
        }


        public MyListView() : base()
        {
            _items = new List<T>();
            OnListChanged = new EventHandler((sender, args) =>
            {
                IChangingListEvent message = (IChangingListEvent)args;
                IChangingListMessage changes = message.GetChanges();
                sender.Info($"Изменения {changes.Action} {changes.Item}");
            });
        }

        public MyListView(List<T> collection) : base()
        {
            _items = new List<T>(collection);
        }





        public void Add(T item)
        {
            _items.Add(item);
            OnAdded(item);
        }
        public void OnAdded(T item)
        {
            Changed = true;
            this.Info($"Added {item}");
            OnListChanged.Invoke(this, new IChangingListEvent(new IChangingListMessage()
            {
                Item = item,
                Action = "Add"
            }));
        }


        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            OnInserted(index, item);
        }
        public void OnInserted(int index, T item)
        {
            Changed = true;
        }


        public bool Remove(T item)
        {
            bool removed = _items.Remove(item);
            if (removed)
            {
                OnRemoved(item);
            }
            return removed;
        }
        public void OnRemoved(T item)
        {
            Changed = true;
            this.Info($"Removed {item}");
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);

            OnRemovedAt(index);
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
                T[] arr = _items.ToArray();
                foreach (T item in arr)
                {
                    Remove(item);
                }
            }
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new MyEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new MyEnumerator<T>(this);
        }

        public T this[int index]
        {
            get => GetItem<T>(index);
            set => SetItem<T>(index, value);
        }

        public T GetItem<T>(int i)
        {
            throw new Exception();
        }

        public void SetItem<T>(int i, object value)
        {
            throw new Exception();
        }




        public int IndexOf(T item)
        {
            return _items.IndexOf(item);
        }











        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            T[] arr = _items.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                array[i] = arr[i];
            }
        }
    }



}
