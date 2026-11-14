using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API;

using Google.Protobuf.WellKnownTypes;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules
{




    [Label("Представление с интегрированной коллекцией данных")]
    public class ChangingDataView<TEntity> : AppViewBase, IEnumerable<TEntity>, IChangingList<TEntity>
        where TEntity : BaseEntity, IChangeSupport
    {
        private readonly TableView table;
        private ChangingList<TEntity> entities;
        private ChangingList<string> columns = new();

        public ChangingDataView()
        {
            LayoutFill(table = new TableView());
            columns = new ChangingList<string>();
            entities = new ChangingList<TEntity>();
        }

        public ChangingDataView(IApi api)
        {
            //this.columns.AddRange(typeof(TEntity).GetInputPropertiesSorted());
            //if (this.columns.Count() == 0)
            //    throw new ArgumentException($"Не определены свойства для ввода в сущности {typeof(TEntity).GetTypeName()}");
            LayoutFill(table = new TableView());
            var fasade = api.GetFasade<TEntity>();
            entities = new ChangingDataList<TEntity>(fasade);
            //this.entities.OnChanged += OnDataChanged;
            table.SetDataRows(typeof(TEntity).GetOwnPropertyNames(), entities);
        }




        public void AddData(TEntity item)
        {
            entities.Add(item);
        }


        private void OnDataChanged(object sender, ListChangedEvent e)
        {
            this.Info(table.VisibleColumns);
            table.SetDataRows(table.VisibleColumns, entities);
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
