using Console_CommonModel.CommonLib.Others;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace WinFormsApp.AppComponents.AppControls
{
    public class SearchComponent : ComponentBase<SearchView, ISearchControl>
    {

        public SearchComponent([NotNullNotEmpty] SearchView ViewModel, [NotNullNotEmpty] ISearchControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var searchView = GetViewModel();
            var searchControl = GetControl();
            //searchControl.SearchQuery = searchView.SearchQuery;
            searchControl.oninput += searchView.oninput;
            //searchControl.onsearch += searchView.onsearch;
            //searchView.SearchOptions.OnAdded((s, e) =>
            //    searchControl.SearchOptions.Add((string)e)
            //);
            //searchView.SearchOptions.OnRemoved((s, e) =>

            //    searchControl.SearchOptions.Remove((string)e)
            //);
        }
    }
}
