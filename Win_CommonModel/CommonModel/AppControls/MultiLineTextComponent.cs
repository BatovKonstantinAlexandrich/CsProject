using WinFormsApp.AppViews.MultiLineModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class MultiLineTextComponent : ComponentBase<MultiLineTextView, MultiLineTextControl>
    {
        public MultiLineTextComponent([NotNullNotEmpty] MultiLineTextView ViewModel, [NotNullNotEmpty] MultiLineTextControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var control = GetControl();
            control.Text = view.Text;
            view.BindPropertiesChangeSupport(control);
        }
    }

}
