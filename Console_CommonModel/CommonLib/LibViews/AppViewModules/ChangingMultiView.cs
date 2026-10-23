using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using System.Collections;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules
{

    [Label("Контейнер с интегрированными представлениями данных")]
    public class ChangingMultiView<TEntity, TMultiView> : AppViewBase, IEnumerable<TEntity>, IChangingList<TEntity>
        where TEntity : BaseEntity, IChangeSupport
        where TMultiView : MultiView<TEntity>
    {
        private ChangingList<TEntity> entities;
        public ChangingList<string> Columns = new();
        public TMultiView view = typeof(TMultiView).New<TMultiView>();

        public ChangingMultiView()
        {
            LayoutFill(view);
            entities = new ChangingList<TEntity>();
            Columns = new ChangingList<string>();
        }

        public ChangingMultiView(IApi api)
        {
            LayoutFill(view);
            var fasade = api.GetFasade<TEntity>();
            entities = new ChangingDataList<TEntity>(fasade);
            Columns.AddRange(typeof(TEntity).GetInputPropertiesSorted());
            entities.OnChanged += OnColumnsChanged;
            if (Columns.Count() == 0)
                throw new ArgumentException($"Не определены свойства для ввода в сущности {typeof(TEntity).GetTypeName()}");
            Columns.OnChanged += OnColumnsChanged;
        }



        public void AddData(TEntity item)
        {
            entities.Add(item);
        }


        private void OnColumnsChanged(object sender, IChangingListEvent e)
        {
            this.Info(Columns);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return ((IEnumerable<TEntity>)entities).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)entities).GetEnumerator();
        }

        public void Bind<Model>(IChangingList<Model> dataset, Func<TEntity, Model> convert, bool hasdata)
        {
            ((IChangingList<TEntity>)entities).Bind(dataset, convert, hasdata);
        }

        public int IndexOf(TEntity item)
        {
            return ((IList<TEntity>)entities).IndexOf(item);
        }

        public void Insert(int index, TEntity item)
        {
            ((IList<TEntity>)entities).Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ((IList<TEntity>)entities).RemoveAt(index);
        }

        public TEntity this[int index] { get => ((IList<TEntity>)entities)[index]; set => ((IList<TEntity>)entities)[index] = value; }

        public void Add(TEntity item)
        {
            ((System.Collections.Generic.ICollection<TEntity>)entities).Add(item);
        }

        public void Clear()
        {
            ((System.Collections.Generic.ICollection<TEntity>)entities).Clear();
        }

        public bool Contains(TEntity item)
        {
            return ((System.Collections.Generic.ICollection<TEntity>)entities).Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            ((System.Collections.Generic.ICollection<TEntity>)entities).CopyTo(array, arrayIndex);
        }

        public bool Remove(TEntity item)
        {
            return ((System.Collections.Generic.ICollection<TEntity>)entities).Remove(item);
        }

        public int Count => ((System.Collections.Generic.ICollection<TEntity>)entities).Count;

        public bool IsReadOnly => ((System.Collections.Generic.ICollection<TEntity>)entities).IsReadOnly;
    }
}
