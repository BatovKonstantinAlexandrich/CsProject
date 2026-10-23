using System.Collections;

namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{

    public interface IChangingListEvents
    {
        public event EventHandler<IChangingListEvent> OnChanged;
    }

    public class ChangingList : ChangingList<object> { }


    public class ChangingList<T> : IChangingList<T>, IChangingListEvents
    {
        private List<T> items = new();

        public void BindChangingList(ChangingList<T> values)
        {
            Clear();
            AddRange(values);
            values.OnAdded((i, p) => { Add((T)p); });
            values.OnRemoved((i, p) => { Remove((T)p); });
        }

        public Action OnAdded(Action<int, object> onadded)
        {
            EventHandler<IChangingListEvent> p = (s, e) =>
            {
                if (e.Target.Action == "added")
                {
                    if (onadded is not null)
                    {
                        onadded(e.Target.Index, e.Target.Item);
                    }
                }
            };
            OnChanged += p;
            return () => {
                OnChanged -= p;
            };
        }

        public void OnRemoved(Action<int, object> onremoved)
        {
            OnChanged += (s, e) =>
            {
                if (e.Target.Action == "removed")
                {
                    if (onremoved is not null)
                    {
                        onremoved(e.Target.Index, e.Target.Item);
                    }
                }
            };
        }

        public ChangingList()
        {
        }

        public ChangingList(IEnumerable<T> items)
        {
            this.items = new(items);
        }

        public event EventHandler<IChangingListEvent> OnChanged = (s, e) => { };
        public void PushEvent(IChangingListMessage message)
        {
            try
            {
                OnChanged.Invoke(this, new IChangingListEvent(message));
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при обработки события изменения коллекциии: {ex.Message}");
                throw new Exception($"Исключение при обработки события изменения коллекциии: {ex.Message}", ex);
            }
        }

        public void Add(T item)
        {
            var index = items.Count();
            items.Add(item);
            PushEvent(new()
            {
                Action = "added",
                Item = item,
                Index = index
            });
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Set(IEnumerable<T> items)
        {
            Clear();
            foreach (var item in items)
                Add(item);
        }

        public void Clear()
        {
            while (items.Count > 0)
            {
                var item = items.LastOrDefault();
                Remove(item);
            }
        }

        public bool Contains(T item) => items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in items)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item)
        {
            var index = items.IndexOf(item);
            var result = items.Remove(item);
            PushEvent(new()
            {
                Action = "removed",
                Item = item,
                Index = index
            });
            return result;
        }


        public bool RemoveRange(IEnumerable<T> expected)
        {
            List<IChangingListMessage> eventList = new();
            foreach (var item in expected)
            {
                var index = items.IndexOf(item);
                var result = items.Remove(item);
                eventList.Add(new IChangingListMessage()
                {
                    Action = "removed",
                    Item = item,
                    Index = index
                });
            }
            foreach (var evt in eventList)
            {
                PushEvent(evt);
            }

            return true;
        }

        public int Count => items.Count();

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        public int IndexOf(T item) => items.IndexOf(item);

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            PushEvent(new()
            {
                Action = "added",
                Item = item
            });
        }

        public void RemoveAt(int index)
        {
            if (index < items.Count() && index >= 0)
            {
                var item = items[index];
                items.RemoveAt(index);
                PushEvent(new()
                {
                    Action = "removed",
                    Item = item,
                    Index = index
                });
            }

        }

        public T this[int index] { get => items[index]; set => items[index] = value; }

        public void Bind<Model>(IChangingList<Model> dataset, Func<T, Model> convert, bool hasdata = false)
        {
            if (convert is null)
                throw new ArgumentNullException(nameof(convert));
            if (dataset is null)
                throw new ArgumentNullException(nameof(dataset));
            if (!hasdata)
                if (dataset.Count() > 0)
                    throw new ArgumentException(nameof(dataset), "Коллекция должна быть пустая");
            OnChanged += (s, e) =>
            {
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
            if (hasdata)
                foreach (var item in items)
                {
                    dataset.Add(convert(item));
                }
        }

        public IEnumerable<T> GetAll()
            => items;

        public void RemoveAll()
        {
            foreach (var item in items.ToList())
            {
                items.Remove(item);
            }
        }

        internal void Sort(Func<T, T, int> comparer)
        {

            var list = this.ToList();
            list.Sort((x1, x2) => comparer(x1, x2));
            Clear();
            AddRange(list);
        }
    }
}
