using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{
    public class SearchableViewControl : IControlService<SearchableView>
    {
        public SearchableView Control { get; set; }

        public void SetControl(SearchableView item)
        {
            Control = item;
        }
    }
}
