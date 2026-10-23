using WinFormsApp.AppViews.AppRootModule;

namespace WinFormsApp.API
{
    /// <summary>
    /// Инструмент для перемещения представлений в конструкторе
    /// </summary>
    public class DesignerViewMoveTool : ViewTool
    {
        private bool moveActive;
        private AppViewBase viewBase;
        private Point dragOffset;
        private DesignerViewSubtreeEvents.Subscription _mouseSubscription;

        public DesignerViewMoveTool() : base(new DesignerView()) { }

        public DesignerViewMoveTool(DesignerView view) : base(view)
        {

        }

        protected override void Activating(ViewNode view)
        {
            viewBase = AppViewBase.ResolveDesignerFocusTarget((AppViewBase)view);

            Action<object, EventArgs> onClick = (_, _) => active = false;
            viewBase.OnClick += onClick;

            _mouseSubscription = new DesignerViewSubtreeEvents.Subscription();
            _mouseSubscription.Wire(viewBase, SetMoveActive, SetMoveApply, SetMoveDeactive);

            rollback.Add(() =>
            {
                viewBase.OnClick -= onClick;
                _mouseSubscription?.Unwire(SetMoveActive, SetMoveApply, SetMoveDeactive);
                _mouseSubscription = null;
                var ctrl = viewBase?.ToControl();
                if (ctrl is not null)
                    ctrl.Capture = false;
            });
        }

        public void SetMoveApply(object s, MouseEventArgs evt)
        {
            if (!moveActive || viewBase is null || viewBase.Dock != "None")
                return;

            var control = viewBase.ToControl();
            var parent = control?.Parent;
            if (parent is null)
                return;

            //var pt = parent.PointToClient(System.Windows.Forms.Control.MousePosition);
            //viewBase.X = pt.X - dragOffset.X;
            //viewBase.Y = pt.Y - dragOffset.Y;
        }

        public void SetMoveActive(object s, MouseEventArgs evt)
        {
            //if (evt.Button != MouseButtons.Left)
            //    return;
            //if (viewBase.Dock != "None")
            //    return;

            //var control = viewBase?.ToControl();
            //var parent = control?.Parent;
            //if (parent is null)
            //    return;

            //moveActive = true;
            //viewBase.Dock = "None";
            //var pt = parent.PointToClient(new(evt.X,evt.Y));
            //dragOffset = new Point(pt.X - viewBase.X, pt.Y - viewBase.Y);
            //control.Capture = true;
        }

        public void SetMoveDeactive(object s, MouseEventArgs evt)
        {
            moveActive = false;
            var control = viewBase?.ToControl();
            if (control is not null)
                control.Capture = false;
        }
    }
}
