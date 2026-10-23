using WinFormsApp.AppViews.BaseComponent;
using WinFormsApp.AppViews.SplitModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class SplitComponent : ComponentBase<SplitView, SplitControl>
    {
        public SplitComponent([NotNullNotEmpty] SplitView viewModel, [NotNullNotEmpty] SplitControl control)
            : base(viewModel, control)
        {
        }

        public override void Bind()
        {
            var view = GetViewModel();
            var ctrl = GetControl();
            var factory = new ComponentsFactory();

            void ApplyLeft() =>
                ctrl.SetPanel1(view.Left is null ? null : factory.CreateControl(view.Left));

            void ApplyRight() =>
                ctrl.SetPanel2(view.Right is null ? null : factory.CreateControl(view.Right));

            ApplyLeft();
            ApplyRight();

            view.AddPropertyChangeListener(nameof(SplitView.Left), _ => ApplyLeft());
            view.AddPropertyChangeListener(nameof(SplitView.Right), _ => ApplyRight());
            view.AddPropertyChangeListener(nameof(SplitView.Orientation), evt =>
            {
                ctrl.SetOrientation(IsHorizontal(evt.After?.ToString()));
            });
            view.AddPropertyChangeListener(nameof(SplitView.SplitterDistance), evt =>
            {
                if (evt.After is int distance)
                    ctrl.SetSplitterDistance(distance);
            });
            view.AddPropertyChangeListener(nameof(SplitView.SplitterWidth), evt =>
            {
                if (evt.After is int width)
                    ctrl.SetSplitterWidth(width);
            });

            ctrl.SetOrientation(IsHorizontal(view.Orientation));
            ctrl.SetSplitterWidth(view.SplitterWidth > 0 ? view.SplitterWidth : 6);
            if (view.SplitterDistance > 0)
                ctrl.SetSplitterDistance(view.SplitterDistance);

            factory.Bind(view, ctrl);
            factory.BindEvents(view, ctrl);
            factory.BindControlEvents(view, ctrl);
            factory.BindContextMenu(view, ctrl);
        }

        static bool IsHorizontal(string? orientation) =>
            string.Equals(orientation, "Horizontal", StringComparison.OrdinalIgnoreCase);
    }
}
