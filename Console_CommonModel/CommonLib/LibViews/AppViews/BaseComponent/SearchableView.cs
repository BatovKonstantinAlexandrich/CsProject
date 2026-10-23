using Console_CommonModel.CommonLib.LibViews.AppViewModules.SearchModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{

    [Label("Представление с возможностью поиска/фильтрации")]
    public class SearchableView : PanelsView, ISearchLine
    {
        [Label("Опции для автоподстановки")]
        public ChangingList<string> SearchOptions { get; set; } = new();
        public string SearchQuery { get => Get<string>(nameof(SearchQuery)); set => Set(nameof(SearchQuery), value); }
        public bool ShowSearchOptions { get => Get<bool>(nameof(ShowSearchOptions)); set => Set(nameof(ShowSearchOptions), value); }

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<string> oninput { get; set; } = text => { };

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]

        public Action<string> onsearch { get; set; } = text => { };

        public SearchableView() : base()
        {
            SearchOptions = new();
            SearchQuery = "";
            ShowSearchOptions = true;
            AddPropertyChangeListener(nameof(Searchable), evt =>
            {
                var searchable = (bool)evt.After;

            });
        }

        public virtual void BindSearchLine(SearchView searchLine)
        {
            
            if (searchLine is not null)
            {
                
                searchLine.SearchQuery = this.SearchQuery;
                searchLine.SearchOptions.BindChangingList(SearchOptions);
                AddPropertyChangeListener(nameof(SearchQuery), evt =>
                {
                    if (searchLine.SearchQuery != $"{evt.After}")
                        searchLine.SearchQuery = $"{evt.After}";
                });
                searchLine.oninput += text =>
                {
                    oninput(text);
                };
                searchLine.onsearch += text =>
                {
                    onsearch(text);
                };
            }
        }

        public void AddInline(ViewItem control)
        {
            var panel = this;
            // Устанавливаем ширину элемента равной ширине панели
            control.Width = panel.Width;
            panel.OnResized += (s, e) => { if (control.Width != panel.Width) control.Width = panel.Width; };

            // Позиционируем элемент под последним добавленным элементом
            if (panel.Controls.Count > 0)
            {
                var lastControl = (ViewItem)panel.Controls[panel.Controls.Count - 1];
                control.X = 0;
                control.Y = lastControl.X + lastControl.Height;
            }
            else
            {
                // Если это первый элемент, размещаем вверху
                control.X = control.Y = 0;

            }

            // Добавляем элемент на панель
            panel.Controls.Add(control);
        }
    }



    /*[Label("Модель иерархического представления для разнородных типов сущностей")]
    public class EntityTreeView<TEntity> : ViewBase<TEntity> where TEntity : HierTable<TEntity>
    {

    }*/

}
