namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
    public interface IContextViewElementsFactory
    {
        ContextViewButton CreateButton(string Icon, string Label, Action onclick);
        ContextViewCheckbox CreateCheckbox(string Icon, string Label, bool Checked, Action<bool> onchanged);
        ContextView CreateRedioGroup(string Icon, string Label, string value, object service, List<string> options, Action<string> onchanged);
        ContextViewTextbox CreateTextbox(string Icon, string Label, string Value, Action<string> onchanged);
    }
}