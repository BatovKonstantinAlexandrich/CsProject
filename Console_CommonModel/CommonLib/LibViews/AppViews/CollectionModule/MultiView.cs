using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.CollectionModule
{

    [Label("Фабричное представление")]
    public class MultiViewConstructor<TEntity> : MultiView<TEntity>
        where TEntity : BaseEntity
    {
        protected Func<TEntity, SingleView<TEntity>> CreateSignleView = data => null;

        public MultiViewConstructor()
        {
            CreateSignleView = CreateSignleView;
        }

        public override void OnDataChanged(object sender, IChangingListEvent evt)
        {
            this.Info(Entities);
            var entity = (TEntity)evt.Target.Item;
            switch (evt.Target.Action)
            {
                case "added":
                    var view = CreateSignleView(entity);
                    Add(view);
                    break;
                case "removed":
                    var ctrl = (ViewItem)Layout.Controls.FirstOrDefault(control => control is SingleView<TEntity> && ((SingleView<TEntity>)control).dataModel == entity);
                    Remove(ctrl);
                    break;
            }
            this.Info(Layout.Controls.Count());

        }

        public override void UpdateView(List<string> list, ChangingList<TEntity> entities)
        {
            this.Info(list);
        }
    }

    [Label("Множественная модель представления")]
    public abstract class MultiView<TEntity> : AppViewBase, ILayoutViewNode, ICollection<TEntity>
        where TEntity : BaseEntity
    {
        [Label("Entities")]
        public virtual ChangingList<TEntity> Entities { get; set; }
        public FlexView Layout;
        public MultiView()
        {
            Entities = new();
            Entities.OnChanged += OnDataChanged;
            LayoutFill(Layout = new FlexView());

        }

        public virtual void OnDataChanged(object sender, IChangingListEvent evt)
        {
            this.Info(Entities);
            this.Info(Layout.Controls.Count());
            UpdateView(typeof(TEntity).GetOwnPropertyNames(), Entities);
        }

        public abstract void UpdateView(List<string> list, ChangingList<TEntity> entities);


        private void BindViewOptions(SingleView<TEntity> view)
        {
            view.Draggable = Draggable;
            view.Selectable = Selectable;
            AddPropertyChangeListener(evt =>
            {
                switch (evt.Property)
                {
                    case nameof(Dropable): view.Selectable = evt.After.ToString().ToBool(); break;
                    case nameof(Draggable): view.Selectable = evt.After.ToString().ToBool(); break;
                    case nameof(Selectable): view.Selectable = evt.After.ToString().ToBool(); break;
                    case nameof(Focusable): view.Selectable = evt.After.ToString().ToBool(); break;
                    case nameof(Searchable): view.Selectable = evt.After.ToString().ToBool(); break;
                }
            });
        }

        public TableView CreateTable(IEnumerable<TEntity> dataset)
        {
            TableView table = new TableView();
            table.Title = typeof(TEntity).GetLabel();
            table.Columns = typeof(TEntity).GetInputPropertiesSorted();
            table.DataRows = new ChangingList<object>(dataset.Select(p => (object)p).ToList());
            return table;
        }

        public TreeViewModel CreateTree<TModel>(IEnumerable<TModel> dataset) where TModel : HierTable<TEntity>
        {
            TreeViewModel tree = new TreeViewModel();

            var rootItem = dataset.FirstOrDefault(p => p.ParentId == 0);
            tree.Item = rootItem;

            dataset.Where(p => p.ParentId == tree.Item.Id).Select(p => tree.Append(new TreeViewModel(tree, p)));
            return tree;
        }




        public bool Has(string key)
        {
            return ((ICollection<TEntity>)Entities).Has(key);
        }

        public TEntity Take(string key)
        {
            return ((ICollection<TEntity>)Entities).Take(key);
        }

        public TEntity Remove(string key)
        {
            return ((ICollection<TEntity>)Entities).Remove(key);
        }

        public string Put(TEntity item)
        {
            return ((ICollection<TEntity>)Entities).Put(item);
        }

        public string Find(TEntity item)
        {
            return ((ICollection<TEntity>)Entities).Find(item);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return ((ICollection<TEntity>)Entities).GetAll();
        }

        public void RemoveAll()
        {
            ((ICollection<TEntity>)Entities).RemoveAll();
        }

        public void Add(ViewNode item)
        {
            this.Info(Count());
            Layout.Add(item);
        }

        public void Clear()
        {
            Layout.Clear();
        }

        public int Count()
        {
            return Layout.Count();
        }

        public void Remove(ViewNode item)
        {
            Layout.Remove(item);
        }
    }
}
