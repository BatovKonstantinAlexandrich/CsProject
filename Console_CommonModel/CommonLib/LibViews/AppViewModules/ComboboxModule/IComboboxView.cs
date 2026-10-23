using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule
{
    public interface IComboboxView: IControlView
    {
        Action<string> OnSelected { get; set; }
        ChangingList<string> Options { get; set; }
        string Value { get; set; } 
    }
}