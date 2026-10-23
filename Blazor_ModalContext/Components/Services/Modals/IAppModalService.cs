using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace BlazorDesignerApp.Web.Services.Modals;

public interface IAppModalService
{
    Task<bool> ShowConfirmAsync(
        string title,
        string message,
        string confirmText = "Да",
        string cancelText = "Отмена",
        DialogOptions? options = null);

    Task ShowMessageAsync(
        string title,
        string message,
        string closeText = "OK",
        DialogOptions? options = null);

    Task<TResult?> ShowDialogAsync<TDialog, TResult>(
        string title,
        DialogParameters? parameters = null,
        DialogOptions? options = null)
        where TDialog : ComponentBase;
}
