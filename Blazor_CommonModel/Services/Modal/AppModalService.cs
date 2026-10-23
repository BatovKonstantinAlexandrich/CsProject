namespace Blazor_CommonModel.Components.Modal
{
    using Blazor_CommonModel.Components.Dialogs;
    using Blazor_CommonModel.Shared;
    using Blazor_CommonModel.Shared.Components;

    using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

    using Microsoft.AspNetCore.Components;
    using Microsoft.Extensions.Options;
    using MudBlazor;

    using System;
    using System.ComponentModel;
    using System.ServiceModel.Channels;

    public sealed class AppModalService(IDialogService dialogService) : IAppModalService
    {


        public async Task<bool> ShowConfirmAsync(
            string title,
            string message,
            string confirmText = "Да",
            string cancelText = "Отмена")
        {
            var parameters = new DialogParameters<ConfirmDialog>
            {
                { x => x.Message, message },
                { x => x.ConfirmText, confirmText },
                { x => x.CancelText, cancelText },
                { x => x.ShowCancel, true }
            };

            var dialog = await dialogService.ShowAsync<ConfirmDialog>(title, parameters, new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            });
            var result = await dialog.Result;
            return result is { Canceled: false };
        }

        public async Task ShowMessageAsync(
            string title,
            string message,
            string closeText = "OK")
        {
            var parameters = new DialogParameters<ConfirmDialog>
        {
            { x => x.Message, message },
            { x => x.ConfirmText, closeText },
            { x => x.ShowCancel, false }
        };

            var dialog = await dialogService.ShowAsync<ConfirmDialog>(title, parameters, new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            });
            await dialog.Result;
        }

  




        /// <summary>
        /// Открыть диалоговое окно с представлением
        /// </summary>
        public async Task<bool> ShowView<TAppView>(string title, TAppView model, Action<bool, TAppView> oncompleted)
            where TAppView : AppViewBase

        {
            var dialogParameters = new DialogParameters<ButtonControl>();
            var dp = new Dictionary<string, object>();
            var value = await ((AppModalService)this).ShowDialogAsync<DemoInputDialog, string>(
                "Редактирование",
                dialogParameters);

            return false;
           

            //var success = clientModal.ShowView<ButtonView>("test", new ButtonView(), (s, p) => {
            //}).Result;


            //var xml = model.ToXmlText();
            //var parameters = new DialogParameters<ViewModal>
            //{
            //    { x => x.Xml, xml },
            //    { x => x.Title, title }
            //};

            //var dialog = await dialogService.ShowAsync<ViewModal>(title, parameters, new DialogOptions
            //{
            //    CloseButton = true,
            //    MaxWidth = MaxWidth.Small,
            //    FullWidth = true
            //});
            //var result = await dialog.Result;
            //return result is { Canceled: false };
        }

        public async Task<TResult?> ShowDialogAsync<TDialog, TResult>(string title, DialogParameters parameters = null, DialogOptions options = null)
            where TDialog : BaseComponent
        {
            var dialog = await dialogService.ShowAsync<TDialog>(title, parameters ?? new DialogParameters(), new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            });
            var result = await dialog.Result;

            if (result is null || result.Canceled)
                return default;

            return result.Data is TResult typed ? typed : default;
        }
    }

}
