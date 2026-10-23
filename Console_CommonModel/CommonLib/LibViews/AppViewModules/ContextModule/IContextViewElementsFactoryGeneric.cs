namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ContextModule
{
    public interface IContextViewElementsFactoryGeneric<TModel>
    {
        ContextViewButton CreateButton(TModel model);
        ContextViewCheckbox CreateCheckbox(TModel model);
        ContextView CreateRadioGroup(TModel model);
        ContextViewTextbox CreateTextbox(TModel model);
    }
}