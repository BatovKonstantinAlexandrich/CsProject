using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.FlexModule;



public class FlexComponent: ComponentBase<FlexView,FlexControl>
{


    public FlexComponent([NotNullNotEmpty] FlexView ViewModel, [NotNullNotEmpty] FlexControl Control) : base(ViewModel, Control)
    {
    }

    public override void Bind()
    {
        try
        {
            FlexControl control = GetControl();
            FlexView view = GetViewModel();
            view.AddPropertyChangeListener(nameof(view.Direction), evt =>
            {
                var direction = evt.After;
                control.SetDirection(view.Direction);
            });

            control.SetDirection(view.Direction);

            /*
            var factory = new ComponentsFactory();
            view.Bind(nameof(FlexControl.WrapContents), this, nameof(FlexControl.WrapContents));
            
            control.SetDirection(view.Direction);

            // Привязка свойства Direction
            control.UpdateFlowDirection(view.Direction);
            view.AddPropertyChangeListener(nameof(FlexView.Direction), evt =>
            {
                control.UpdateFlowDirection(evt.After?.ToString() ?? "LeftToRight");
            });

            var list = control.GetChangingList(control.flowLayoutPanel.Controls);
            //view.items.Bind<Control>(list, view => ((AppViewBase)view).ToControl(), true);
            foreach (var item in view.items)
            {
                control.flowLayoutPanel.Controls.Add(((AppViewBase)item).ToControl());
            }
            
            */

            foreach (var item in view.Controls)
            {
                control.flowLayoutPanel.Controls.Add(((AppViewBase)item).ToControl());
            }

            view.Controls.OnChanged += (s, e) =>
            {
                switch (e.Target.Action)
                {
                    case "added":
                        control.flowLayoutPanel.Controls.Add(((AppViewBase)e.Target.Item).ToControl());
                        break;
                }
            };

            componentsFactory.Bind(view, control);
            componentsFactory.BindEvents(view, control);
            componentsFactory.BindControlEvents(view, control);
            componentsFactory.BindContextMenu(view, control);

        }
        catch (Exception ex)
        {
            this.Error(ex);
        }
    }

   
}