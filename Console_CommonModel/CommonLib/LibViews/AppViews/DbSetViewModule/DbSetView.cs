using pickpoint_delivery_service;



using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule
{
    [Label("Представление набора данных")]
    public class DbSetView<TDbContext, TEntity> : AppViewBase, IDisposable
        where TDbContext : DbContextBase
        where TEntity : BaseEntity
    {
        private GridView grid;

        [NotNullNotEmpty]
        [Label("класс")]
        private Repository<TEntity, TDbContext> repository { get; set; }
        [Label("Create View")]
        public Func<TEntity, AppViewBase> CreateView { get; }
        private TDbContext dbContext { get; set; }
        [Label("properties")]
        public ChangingList<string> properties { get; set; } = new();


        public DbSetView() : this(typeof(TDbContext).New<TDbContext>()) { }
        public DbSetView(TDbContext dbContext, Func<TEntity, AppViewBase> createView = null)
        {
            CreateView = createView;
            if (CreateView is null)
                CreateView = item => new ButtonView() { Text = item.ToString() };
            this.dbContext = dbContext;
            repository = new Repository<TEntity, TDbContext>(dbContext);
            properties = new(typeof(TEntity).GetOwnInputPropertyNames());

            InitView();

            OnLoad += (s, e) =>
            {
                OnSearch("");
            };
        }



        protected override void OnDesignModeChanged(PropertyChangedMessage message)
        {
            base.OnDesignModeChanged(message);
            if (DesignMode)
            {
                ViewDesigner = new DbSetViewDesigner<TDbContext, TEntity>(this);
            }
            else
            {
                ViewDesigner.Deactivate(this);
            }
        }



        //public override Control ToControl()
        //    => ToControl<AppViewBase>();

        public AppViewBase InitView()
        {
            AppViewBase side = this;

            LayoutFill(grid = new GridView());
            var search = new Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule.SearchView() { Height = 60 };
            search.onsearch += OnSearch;
            search.onsearch("");

            search.oninput += query =>
            {
                //var last = string.IsNullOrWhiteSpace(query) ? "" : $"{query}".SplitWords().Last().Trim().ToLower();
                //var suggestions = repository.GetAutocompleteSuggestions(last).Select((p) => p.Text);
                var options = repository.GetOptions(query);
                search.SearchOptions.Clear();
                search.SearchOptions.AddRange(options);
            };
            search.oninput += q =>
            {
                //this.Info(((SearchControl)search.control).SearchOptions.ToJsonOnScreen());
            };
            //this.BindSearchLine(search);

            side.Top = search;

            var filters = repository.GetFilters();
            side.Left = new InputSearchFilters(filters);
            side.LeftWidth = 200;
            side.keyboardConfig["`"] = () =>
            {
                side.ToggleLeft();
            };
            side.BottomHeight = 75;
            return side;
        }



        private void OnSearch(string q)
        {
            Trace();

            //Controls.Clear();
            //StatusView.PutAction("data", () => { Thread.Sleep(1000); });
            var results = repository.Search(q);
            GetOrCreateAppRootView().StatusView.Text = $"Найдено: {results.Count()}";
            this.Info(Controls.Count());
            grid.Controls.Clear();
            foreach (var item in results)
            {
                AppViewBase card = CreateView(item);

                grid.Add(card);

                card.Width = 333;
                card.Height = 333;
            };
        }


        public void Dispose()
            => dbContext.Dispose();

        internal AppViewBase Edit<TDbContext, TEntity>(DbSetView<TDbContext, TEntity> view, Action<bool, object> value)
            where TDbContext : DbContextBase
            where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }
    }
}
