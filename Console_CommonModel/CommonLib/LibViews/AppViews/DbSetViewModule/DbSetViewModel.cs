using pickpoint_delivery_service;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule
{
    [Label("Представление моделирования структуры данных")]
    public class DbSetViewModel<TDbContext, TEntity> :
        AppViewBase, IDisposable
        where TDbContext : DbContextBase
        where TEntity : BaseEntity
    {
        private SearchView searchView;




        [NotNullNotEmpty]
        [Label("класс")]
        private Repository<TEntity, TDbContext> repository { get; set; }
        private TDbContext dbContext { get; set; }
        [Label("properties")]
        public ChangingList<string> properties { get; set; } = new();


        public DbSetViewModel() : this(typeof(TDbContext).New<TDbContext>()) { }
        public DbSetViewModel(TDbContext dbContext)
        {
            this.dbContext = dbContext;
            repository = new Repository<TEntity, TDbContext>(dbContext);
            properties = new(typeof(TEntity).GetOwnInputPropertyNames());

            InitView();
        }



        protected override void OnDesignModeChanged(PropertyChangedMessage message)
        {
            base.OnDesignModeChanged(message);
            if (DesignMode)
            {
                if (ViewDesigner is null)
                {
                    ViewDesigner = new DbSetViewDesigner<TDbContext, TEntity>(this);
                }
                ViewDesigner.Activates(this);
            }
            else
            {
                ViewDesigner.Deactivate(this);
            }
        }



        //public override Control ToControl()
        //    => ToControl<AppViewBase>();

        public void InitView()
        {


            searchView = new SearchView() { Height = 60 };
            searchView.onsearch += OnSearch;
            searchView.oninput += OnInput;
            searchView.oninput += q =>
            {
                //this.Info(((SearchControl)this.searchView.control).SearchOptions.ToJsonOnScreen());
            };
            searchView.onsearch("");


            Top = searchView;
            //this.BindSearchLine(search);


            AppViewBase side = this;


            var filters = repository.GetFilters();
            side.Left = new InputSearchFilters(filters);
            side.LeftWidth = 200;
            side.keyboardConfig["`"] = () =>
            {
                side.ToggleLeft();
            };
            side.BottomHeight = 75;

        }

        private void OnInput(string query)
        {
            var last = string.IsNullOrWhiteSpace(query) ? "" : $"{query}".SplitWords().Last().Trim().ToLower();


            searchView.SearchOptions.Clear();
            searchView.SearchOptions.AddRange(repository.GetRepositoryOptions(last).Select((p) => p.Text));


        }

        private void OnSearch(string q)
        {
            Trace();

            //Controls.Clear();
            //StatusView.PutAction("data", () => { Thread.Sleep(1000); });
            var results = repository.Search(q);
            GetOrCreateAppRootView().StatusView.Text = $"Найдено: {results.Count()}";
            this.Info(Controls.Count());
            Controls.Clear();
            foreach (var item in results)
            {
                AppViewBase card = CreateViewItem(item);

                Add(card);

                card.Width = 333;
                card.Height = 333;
            };
        }

        public override void Add(ViewNode item)
        {
            Controls.Add(item);
        }

        public override void Remove(ViewNode item)
        {
            Remove(item);
        }

        private AppViewBase CreateViewItem(TEntity item)
        {
            var id = item.Id;
            /*var card = new CardView();
            card.AutoSize = true;
            foreach (var property in properties)
            {
                var field = side.CreateFormField(item, property);
                card.AddInline(field);
                card.AddInline(side.CreateLabel("123"));
            }
            properties.OnAdded((i, p) => { card.AddInline(side.CreateFormField(item, p.ToString())); });
            properties.OnRemoved((i, p) => { card.Controls.RemoveAt(i); });*/
            //flex.Add(card);
            var card = CreateImage(typeof(DeliveryDbContext).New<DeliveryDbContext>().ProductImages.FirstOrDefault(i => i.ProductId == id).ImageData);
            return card;
        }

        public void Dispose()
            => dbContext.Dispose();
    }
}
