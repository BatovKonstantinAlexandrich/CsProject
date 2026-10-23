using Microsoft.AspNetCore.Components;
using MudBlazor;
using BlazorDesignerApp.Web.Components.Dialogs;

namespace BlazorDesignerApp.Web.Services.Modals;

public sealed class AppModalService(IDialogService dialogService) : IAppModalService
{
    public async Task<bool> ShowConfirmAsync(
        string title,
        string message,
        string confirmText = "Да",
        string cancelText = "Отмена",
        DialogOptions? options = null)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Message, message },
            { x => x.ConfirmText, confirmText },
            { x => x.CancelText, cancelText },
            { x => x.ShowCancel, true }
        };

        var dialog = await dialogService.ShowAsync<ConfirmDialog>(title, parameters, MergeOptions(options));
        var result = await dialog.Result;
        return result is { Canceled: false };
    }

    public async Task ShowMessageAsync(
        string title,
        string message,
        string closeText = "OK",
        DialogOptions? options = null)
    {
        var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Message, message },
            { x => x.ConfirmText, closeText },
            { x => x.ShowCancel, false }
        };

        var dialog = await dialogService.ShowAsync<ConfirmDialog>(title, parameters, MergeOptions(options));
        await dialog.Result;
    }

    public async Task<TResult?> ShowDialogAsync<TDialog, TResult>(
        string title,
        DialogParameters? parameters = null,
        DialogOptions? options = null)
        where TDialog : ComponentBase
    {
        var dialog = await dialogService.ShowAsync<TDialog>(title, parameters ?? new DialogParameters(), MergeOptions(options));
        var result = await dialog.Result;

        if (result is null || result.Canceled)
            return default;

        return result.Data is TResult typed ? typed : default;
    }

    private static DialogOptions MergeOptions(DialogOptions? options) =>
        options ?? new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
}
