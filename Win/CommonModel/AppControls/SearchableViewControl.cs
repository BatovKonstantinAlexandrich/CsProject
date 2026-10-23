using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{
    public class SearchableViewControl : IControlService<SearchableView>
    {
        public SearchableView programDbContext { get; set; }

        public void SetControl(SearchableView item)
        {
            programDbContext = item;
        }
    }
}
