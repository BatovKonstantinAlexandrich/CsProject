using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.LayoutModule;
using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControlsModule
{

    /// <summary>
    /// Компонент отвечает за связывание свойсв
    /// </summary>
    public class LayoutComponent : ComponentBase<LayoutView, LayoutControl>
    {
        public LayoutComponent([NotNullNotEmpty] LayoutView ViewModel, [NotNullNotEmpty] LayoutControl Control) : base(ViewModel, Control)
        {
        }

        /// <summary>
        /// Связаные свйоств позиционирования
        /// </summary>
        public override void Bind()
        {
            var layoutView = GetViewModel();
            var layoutControl = GetControl();
            layoutControl.IsVertical = layoutView.IsVertical;
            layoutView.AddPropertyChangeListener(nameof(layoutView.IsVertical), evt =>
            {
                layoutControl.IsVertical = evt.After.ToString().ToBool();
            });
            layoutControl.WrapContent = layoutView.WrapContent;
            layoutView.AddPropertyChangeListener(nameof(layoutView.WrapContent), evt =>
            {
                layoutControl.WrapContent = evt.After.ToString().ToBool();
            });
            layoutControl.FillContent = layoutView.FillContent;
            layoutView.AddPropertyChangeListener(nameof(layoutView.FillContent), evt =>
            {
                layoutControl.FillContent = evt.After.ToString().ToBool();
            });
            foreach (AppViewBase view in layoutView.Controls)
            {
                var c = view.ToControl();               
                layoutControl.Add(c);             
                this.Info(new {
                    type= view.GetTypeName(),
                    c.Size,
                    c.Location
                });
            }
            layoutView.Controls.OnAdded((index, view) =>
            {
                
                var ctrl = ((AppViewBase)view).ToControl();
                layoutControl.Add(ctrl);
                layoutView.IsVertical = layoutView.IsVertical;
            });
            layoutView.Controls.OnRemoved((index, view) =>
            {
                ((AppViewBase)view).Parent = layoutView;
                layoutControl.Remove(((AppViewBase)view).ToControl());
                layoutView.IsVertical = layoutView.IsVertical;
            });
            layoutView.IsVertical = layoutView.IsVertical;
        }
    }
}
