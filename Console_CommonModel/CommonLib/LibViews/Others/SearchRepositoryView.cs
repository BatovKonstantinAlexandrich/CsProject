using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibData.RepositoryPattern;

namespace Console_CommonModel.CommonLib.LibViews.Others
{

    [Label("Представление для отображения и поиска ")]

    public class SearchRepositoryView<TDbContext, TEntity> : AppViewBase
        where TDbContext : DbContextBase
        where TEntity : BaseEntity
    {
        public readonly Repository<TEntity, TDbContext> entityRepository;
        private readonly GridView gridView;
        private readonly Func<TEntity, AppViewBase> createView;

        [Label("Search Line")]
        public SearchView SearchLine { get; }

        public static AppViewBase CreateSearchRepository(Type dbContextType, Type entityType, Action<object> onselected)
        {
            return typeof(SearchRepositoryView<,>).MakeGenericType(new List<Type>() { dbContextType, entityType }.ToArray()).New<AppViewBase>();
        }



        public SearchRepositoryView(Func<TEntity, AppViewBase> createView)
        {
            this.createView = createView;
            gridView = new GridView();

            SearchLine = new SearchView();

            gridView = new GridView();

            BindSearchLine(SearchLine = new SearchView());
            AppViewBase p = new();
            p.Top = SearchLine;
            p.LayoutFill(gridView);
            LayoutFill(p);
            entityRepository = new Repository<TEntity, TDbContext>();
            oninput += OnInputSearchQuery;
            onsearch += OnSearchSubmitted;
            OnLoad += (s, e) =>
            {

                OnSearchSubmitted("");
            };
        }


        public void CreateMenuView()
        {
            var menu = new MenuView();
            Top = menu;
            menu.CreateMenuView("Создать", new() {
                { "создать",()=>{

                    var p = this;
                    LayoutFill(CreateEditFormView(typeof(TEntity).New<TEntity>(), target => {

                        using(var db=typeof(TDbContext).New<DbContextBase>())
                        {
                            db.Add(target);
                            db.SaveChanges();
                        }

                        entityRepository.Save();
                        p.GoBack();
                        p.OnSearchSubmitted(p.SearchQuery);
                    }));

                } }
            });
        }


        private void OnSearchSubmitted(string query)
        {
            List<TEntity> results = null;
            var ctrls = gridView.Controls.ToList();
            ctrls.Reverse();
            foreach (var p in ctrls)
                gridView.Remove(p);

            results = entityRepository.Search(query);
            foreach (var result in results)
            {

                var view = createView(result);
                gridView.Add(view);
            }


        }

        public AppViewBase CreateRecordView(TEntity entity)
        {
            if (entity is Product p)
                return new ProductCardView(p);
            //var view = new AppViewBase() { Dock = "None", Width = 200, Height = 200 };
            /*var card = new CardView()
            {
                Margin = 20,
                Width = 120,
                Height = 120,
                MinHeight = 120
            };*/

            var viewBase = new AppViewBase()
            {
                Margin = 10,
                Width = 120,
                Height = 120,
                MinWidth = 120,
                MinHeight = 120,
                BorderWidth = 1
            };
            if (entity is NamedObject no)
            {
                viewBase.Add(viewBase.CreateTextCenter(no.Name));
            }
            return viewBase;
        }

        public void OnInputSearchQuery(string query)
        {
            SearchLine.SearchOptions.Clear();
            SearchLine.SearchOptions.AddRange(entityRepository.GetRepositoryOptions(query).Select(p => query + p.Text).Where(p => p.IsEng() || p.IsRus()));
            SearchLine.Height = SearchLine.SearchOptions.Count() * 30 + 70;



        }

    }
}
