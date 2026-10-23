using Console_CommonModel.CommonLib.LibData;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

using Console_CommonModel.CommonLib.LibTesting;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DbSetViewModule
{

    [Label("Поисковые фильтры")]
    public class InputSearchFilters : AppViewBase
    {
        private IEnumerable<FilterBase> filters;
        private string selectedFilter;

        public InputSearchFilters() : this(new List<FilterBase>()) { }
        public InputSearchFilters(IEnumerable<FilterBase> filters)
        {
            this.filters = filters;
            InitControls();
        }

        //public override Control ToControl()
        //{
        //    return base.ToControl<ColumnsView>();
        //}

        private void InitControls()
        {
            this.Info(filters.Select(f => $"{f.Label} {f.Indicator}").ToList().ToJsonOnScreen());
            LayoutFill(CreateCombobox("Тип фильтра",
                new(filters.Select(f => $"{f.Label} {f.Indicator}").ToList()),
                filter => selectedFilter = filter));
            LayoutBottom(CreateButton("добавить", () =>
            {
                CreateFilter(selectedFilter);
            }));
        }

        private void CreateFilter(string selectedFilter)
        {
            var filter = filters.First(f => $"{f.Label} {f.Indicator}" == selectedFilter);
            if (filter is FilterEquals fe)
            {

            }
            else if (filter is FilterRange fr)
            {
            }
            else if (filter is FilterSet fs)
            {
            }
            else if (filter is FilterNot fn)
            {

            }
        }
    }
}