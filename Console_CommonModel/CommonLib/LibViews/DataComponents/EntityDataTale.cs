using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System.Collections;
using System.Data;

namespace Console_CommonModel.CommonLib.LibViews.DataComponents
{

    [Label("Таблица сущностей")]
    [Description("Гибридный класс, объединяющий функциональность DataTable и IList<TEntity>, позволяющий работать с сущностями как с таблицей данных и как со списком")]
    public class EntityDataTable<TEntity> : DataTable, IList<TEntity>
    {
        private ChangingList<TEntity> list = new();

        public int IndexOf(TEntity item)
        {
            return ((IList<TEntity>)list).IndexOf(item);
        }

        public void Insert(int index, TEntity item)
        {
            ((IList<TEntity>)list).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<TEntity>)list).RemoveAt(index);
        }

        public TEntity this[int index] { get => ((IList<TEntity>)list)[index]; set => ((IList<TEntity>)list)[index] = value; }

        public void Add(TEntity item)
        {
            ((System.Collections.Generic.ICollection<TEntity>)list).Add(item);
        }

        public bool Contains(TEntity item)
        {
            return ((System.Collections.Generic.ICollection<TEntity>)list).Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            ((System.Collections.Generic.ICollection<TEntity>)list).CopyTo(array, arrayIndex);
        }

        public bool Remove(TEntity item)
        {
            return ((System.Collections.Generic.ICollection<TEntity>)list).Remove(item);
        }

        public int Count => ((System.Collections.Generic.ICollection<TEntity>)list).Count;

        public bool IsReadOnly => ((System.Collections.Generic.ICollection<TEntity>)list).IsReadOnly;

        public IEnumerator<TEntity> GetEnumerator()
        {
            return ((IEnumerable<TEntity>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}
