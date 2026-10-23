using Blazor_CommonModel.Shared;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.Others;

using MudBlazor;


namespace Blazor_CommonModel.Components.Modal
{
    public interface IAppModalService
    {
        Task<bool> ShowConfirmAsync(string title, string message, string confirmText = "Да", string cancelText = "Отмена");
       
        Task<TResult?> ShowDialogAsync<TDialog, TResult>(string title, DialogParameters parameters = null, DialogOptions options = null)
            where TDialog : Blazor_CommonModel.Shared.BaseComponent;
        Task ShowMessageAsync(string title, string message, string closeText = "OK");
        Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted) where TAppView : AppViewBase;

        
    }
}
