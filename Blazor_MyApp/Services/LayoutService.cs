using ActionModels;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Blazor_MyApp.Services
{


    /// <summary>
    /// Обслуживает главную страницу layout
    /// </summary>
    /// <param name="navigationManager"></param>
    public class LayoutService(NavigationManager navigationManager): ChangeSupport
    {

        [Parameter]
        public List<ILink> Menus { get; set; } = new();

        public AppViewRoot viewRoot = new();

        public async Task OnInitiallizedAsync()
        {
            navigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            this.Info(navigationManager.Uri);
            
        }
    }
}
