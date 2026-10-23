using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FormModule
{
    [Label("Форма редактирования (ввода)")]
    public class InputView : ControlView, ILayoutViewNode
    {
        private readonly ColumnsView columnsView;

        private TextView LabelControl;
        private ColumnsView ErrorsControl;



        [Label("Label")]
        public string Label { get => Get<string>(nameof(Label)); set => Set(nameof(Label), value); }

        [Label("Errors")]
        public ChangingList<string> Errors { get => Get<ChangingList<string>>(nameof(Errors)); set => Set(nameof(Errors), value); }

        [Label("Control Pane")]
        public AppViewBase ControlPane { get; private set; }

        public InputView() : base()
        {
            Label = "home";
            Errors = new();
            LayoutFill(columnsView = new ColumnsView());
            LabelControl = CreateText(Label);
            ErrorsControl = new ColumnsView();
            AddPropertyChangeListener(nameof(Label), evt => LabelControl.Text = evt.After is not null ? evt.After.ToString() : null);
            SetView(new AppViewBase());
        }

        public void SetView(AppViewBase control)
        {

            Add(LabelControl);
            Add(ControlPane = control);
            Add(ErrorsControl);
        }

        public void Add(ViewNode item)
        {
            ((ILayoutViewNode)columnsView).Add(item);
        }


        public void Remove(ViewNode item)
        {
            ((ILayoutViewNode)columnsView).Remove(item);
        }
    }
}
