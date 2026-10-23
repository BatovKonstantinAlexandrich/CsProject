using Console_CommonModel.API;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.ViewControl
{
    /// <summary>
    /// Контракт представления WinForms: те же операции, что и у <see cref="API.IConsoleСontrol"/>,
    /// для последующей подмены реализации (консоль ↔ UI) через SignalR.
    /// </summary>
    public interface IViewControl : IConsoleСontrol
    {
    }
}
