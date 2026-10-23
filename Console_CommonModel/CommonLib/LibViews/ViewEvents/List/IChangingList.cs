namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{
    public interface IChangingList<T> : IList<T>
    {
        public void Bind<Model>(IChangingList<Model> dataset, Func<T, Model> convert, bool hasdata);

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
}
