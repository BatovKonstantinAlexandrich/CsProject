using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    /// <summary>
    /// 
    [Label("Размещение представлений")]
    public abstract class ViewLayout : AppViewBase, ILayoutViewNode
    {

        [Label("Flow Direction")]
        public LayoutDirection FlowDirection { get => Get<LayoutDirection>(nameof(FlowDirection)); set => Set(nameof(FlowDirection), value); }
        [Label("Scroll Pane")]
        public bool ScrollPane { get => Get<bool>(nameof(ScrollPane)); set => Set(nameof(ScrollPane), value); }
        [Label("Wrap Contents")]
        public bool WrapContents { get => Get<bool>(nameof(WrapContents)); set => Set(nameof(WrapContents), value); }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [Label("Controls")]
        public override ChangingList<ViewNode> Controls { get => base.Controls; set => base.Controls = value; }

        public void Add(ViewNode item) => base.Controls.Add(item);
        public void Clear() => base.Controls.Clear();
        public int Count() => base.Controls.Count();
        public void Remove(ViewNode item) => base.Controls.Remove(item);

        public ViewLayout()
        {
            FlowDirection = LayoutDirection.LeftRight;
            WrapContents = true;
            ScrollPane = true;
        }

        //public override Control ToControl()
        //{
        //    var ctrl = new ControlLayout();
        //    foreach(AppViewBase childView in this.Controls)
        //    {
        //        var childControl = childView.ToControl();
        //        ctrl.Add(childControl);
        //    }
        //    this.Controls.OnAdded((index, item) => {
        //        var childControl = ((AppViewBase)item).ToControl();
        //        ctrl.Add(childControl);
        //    });
        //    this.Controls.OnRemoved((index, item) => {                
        //        ctrl.Remove(((AppViewBase)item).control);
        //    });
        //    return ctrl;
        //}
    }
}
