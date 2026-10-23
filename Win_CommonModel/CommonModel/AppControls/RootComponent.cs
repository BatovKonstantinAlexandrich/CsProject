using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{
    /*public class RootView: FormView
    {

    }
    public class RootControl : FormControl
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public class RootComponent : ComponentBase<RootView, RootControl>
    {
        public RootComponent([NotNullNotEmpty] RootView ViewModel, [NotNullNotEmpty] RootControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            ViewModel.AddPropertyChangeListener(nameof(FormView.Title), evt => Control.GetForm().Text = evt.After.ToString());
            ViewModel.AddPropertyChangeListener(nameof(FormView.Icon), evt => Control.GetForm().Icon = new Icon(evt.After.ToString()));
            var view = this.GetViewModel();
            view.Title = view.Title;
            view.Icon = view.Icon;
        }
    }*/
}
