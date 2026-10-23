
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

using static Mysqlx.Crud.Order.Types;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.FlexModule
{

    [Label("Гибкое позиционирование")]
    public class FlexView : ControlView, ILayoutViewNode, IFlexView
    {
        [InputBool]
        [Label("Fill Direction")]
        public bool FillDirection { get => Get<bool>(nameof(WrapContents)); set => Set(nameof(WrapContents), value); }
        [InputBool]
        [Label("Wrap Contents")]
        public bool WrapContents { get => Get<bool>(nameof(WrapContents)); set => Set(nameof(WrapContents), value); }

        [Label("Направление")]
        [Description("Направление размещения элементов в контейнере")]
        [InputSelect("LeftToRight,TopDown,RightToLeft,BottomUp")]
        public string Direction { get => Get<string>(nameof(Direction)); set => Set(nameof(Direction), value); }

        public FlexView()
        {
            FillDirection = true;
            WrapContents = true;
            Direction = "LeftToRight";
            Dock = "Fill";
            AddPropertyChangeListener(nameof(Direction), evt =>
            {
                var direction = evt.After.ToString();
                switch (direction)
                {
                    case "LeftToRight":
                    case "RightToLeft":
                        break;

                    case "TopDown":
                    case "BottomUp":
                        break;
                }
            });
        }

        public void Add(ViewNode item)
        {
            if (FillDirection)
            {
                switch (Direction)
                {
                    case "LeftToRight":
                        ((AppViewBase)item).Dock = "Left";
                        break;
                    case "RightToLeft":
                        ((AppViewBase)item).Dock = "Right";
                        break;

                    case "TopDown":
                        ((AppViewBase)item).Dock = "Top";
                        break;
                    case "BottomUp":
                        ((AppViewBase)item).Dock = "Bottom";
                        break;
                }
            }
            Controls.Add(item);
        }

        public void Clear() => Controls.Clear();
        public int Count() => Controls.Count();
        public void Remove(ViewNode item) => Controls.Remove(item);

        /*public override Control ToControl()
        {
            this.Info(this.GetTypeName());
            FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            foreach(AppViewBase ctrl in Controls)
            {
                flowLayoutPanel.Controls.Add(ctrl.ToControl());
            }
            flowLayoutPanel.FlowDirection = 
                this.Direction=="TopDown"? FlowDirection.TopDown: 
                this.Direction== "RightToLeft" ? FlowDirection.RightToLeft: 
                this.Direction== "BottomUp" ? FlowDirection.BottomUp: 
                this.Direction== "LeftToRight" ? FlowDirection.LeftToRight :
                FlowDirection.LeftToRight;
            this.Controls.OnChanged += (s, e) => { 
                switch(e.Target.Action)
                {
                    case "added":
                        flowLayoutPanel.Controls.Add(((AppViewBase)e.Target.Item).ToControl());
                        break;
                    case "removed":
                        flowLayoutPanel.Controls.RemoveAt(e.Target.Index);
                        break;
                }
            };
            return flowLayoutPanel;
        }*/
    }
}