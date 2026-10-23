using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TypedModule
{
    [Label("Типизированное представление")]
    public class TypedView : AppViewBase
    {


        [Label("Модель представления")]
        [InputConstructor<AppViewBase>()]
        public AppViewBase ViewModel { get => Get<AppViewBase>(nameof(ViewModel)); set => Set(nameof(ViewModel), value); }

        public TypedView()
        {
            ViewModel = new ButtonView();
            AutoSize = true;
        }


    }
}
