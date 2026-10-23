using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsApp.AppViews.SearchModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class SearchComponent : ComponentBase<SearchView, SearchControl>
    {
        public SearchComponent() : base(new(), new()) { }



        public SearchComponent([NotNullNotEmpty] SearchView ViewModel, [NotNullNotEmpty] SearchControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var searchView = GetViewModel();
            var searchControl = GetControl();
            searchView.SearchOptions.OnAdded((s, e) =>
                searchControl.SearchOptions.Add((string)e)
            );
            searchView.SearchOptions.OnRemoved((s, e) =>

                searchControl.SearchOptions.Remove((string)e)
            );
        }
    }
}
