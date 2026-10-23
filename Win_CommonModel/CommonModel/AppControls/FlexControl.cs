using System.Windows.Forms;

using WinFormsApp.AppComponents.AppControls;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    public class FlexControl : ControlBase
    {
        public readonly FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0)
        };

        public bool FillSize = true;

        public FlexControl()
        {
            AutoSize = true;
            this.LayoutFill(flowLayoutPanel);
        }


        public void SetDefaultLayout()
        {

        }

        public void SetDirection(object direction)
        {
            switch (direction)
            {
                case "TopDown":
                    flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
                    break;
                case "DownTop": flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight; break;
                case "BottomUp": flowLayoutPanel.FlowDirection = FlowDirection.BottomUp; break;
                case "RightToLeft": flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft; break;
            }
        }

        public void Add(Control item)
        {
            flowLayoutPanel.Controls.Add(item);

            if (FillSize)
            {
                if (flowLayoutPanel.FlowDirection == FlowDirection.LeftToRight)
                {
                    item.BindHeightToParent();
                }
                if (flowLayoutPanel.FlowDirection == FlowDirection.RightToLeft)
                {
                    item.BindHeightToParent();
                }
                if (flowLayoutPanel.FlowDirection == FlowDirection.TopDown)
                {
                    item.BindWidthToParent();
                }
                if (flowLayoutPanel.FlowDirection == FlowDirection.BottomUp)
                {
                    item.BindWidthToParent();
                }
            }
        }

        public void Remove(Control item)
        {
            flowLayoutPanel.Controls.Remove(item);
        }

    }

    /*/// <summary>
    /// Гибкое позиционирование элементов управления
    /// </summary>
    public class FlexControl : ControlBase, ILayoutControl
    {

        /// <summary>
        /// Видна ли полоса прокрутки
        /// </summary>
        public bool Scrollable { get => Get<bool>(nameof(Scrollable)); set => Set(nameof(Scrollable), value); }

        /// <summary>
        /// Выполняется ли перенос при переполнении
        /// </summary>
        public bool WrapContents { get => Get<bool>(nameof(WrapContents)); set => Set(nameof(WrapContents), value); }


        /// <summary>
        /// Панель размещения
        /// </summary>
        public readonly FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel
        {
          
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoScroll = false,
            AutoSize = false,
            Padding = new Padding(0)
        };


        public FlexControl()
        {
            this.flowLayoutPanel.BorderStyle = BorderStyle.Fixed3D;            
            this.flowLayoutPanel.DataBindings.Add(nameof(flowLayoutPanel.WrapContents), this, nameof(flowLayoutPanel.WrapContents), true);
            this.Controls.Add(flowLayoutPanel);
            flowLayoutPanel.Height = 320;
            flowLayoutPanel.Width = 320;
        }

        public void Add(Control item)
        {

            flowLayoutPanel.Controls.Add(item);
        }
        public void Clear() => flowLayoutPanel.Controls.Clear();
        public int Count() => flowLayoutPanel.Controls.Count;
        public void Remove(Control item) => flowLayoutPanel.Controls.Add(item);

        


        public ChangingList<Control> GetChangingList(ControlCollection controls)
        {
            var list = new ChangingList<Control>();
            list.OnChanged += (s, evt) =>
            {
                switch (evt.Target.Action)
                {
                    case "added":
                        controls.Add((Control)evt.Target.Item);
                        break;
                    case "removed":
                        controls.Remove((Control)evt.Target.Item);
                        break;
                }
            };
            return list;
        }


        

        public void UpdateFlowDirection(string direction)
        {
            if (string.IsNullOrWhiteSpace(direction))
            {
                direction = "LeftToRight";
            }

            switch (direction)
            {
                case "LeftToRight":
                    flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;
                case "TopDown":
                    flowLayoutPanel.FlowDirection = FlowDirection.TopDown;
                    break;
                case "RightToLeft":
                    flowLayoutPanel.FlowDirection = FlowDirection.RightToLeft;
                    break;
                case "BottomUp":
                    flowLayoutPanel.FlowDirection = FlowDirection.BottomUp;
                    break;
                default:
                    flowLayoutPanel.FlowDirection = FlowDirection.LeftToRight;
                    break;
            }
        }
    }*/
}
