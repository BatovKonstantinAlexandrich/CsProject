using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ColumnsControl : ControlBase
    {
        private readonly ChangingList<Control> lines = new();

        public bool Fixed { get; set; } = false;
        public bool Mode { get; set; } = false;
        public int Count() => lines.Count;
        public ColumnsControl()
        {
            BorderStyle = BorderStyle.Fixed3D;
            AutoScroll = true;
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.Fixed3D;
            Resize += (s, e) =>
            {
                //this.Debug($"[{this.Size.Width},{this.Size.Height}]");
                if (lines is not null)
                    foreach (Control c in lines)
                    {
                        //this.Debug($"[{c.Size.Width},{c.Size.Height}]");
                    }
            };

        }

        public void AddChangeListener(EventHandler<IChangingListEvent> onchanged)
        {
            if (onchanged is not null)
                lines.OnChanged += onchanged;
        }

        public void Clear()
        {
            lines.ToList().ForEach(Controls.Remove);

        }
        public void Set(Control control)
        {
            foreach (var item in lines.ToList())
                Remove(item);
            lines.Add(control);
            UpdateControlsPanel();
            control.ControlRemoved += (s, e) =>
            {
                lines.Remove(control);
            };
        }
        public void Add(Control control)
        {
            lines.Add(control);

            UpdateControlsPanel();
            control.ControlRemoved += (s, e) =>
            {
                lines.Remove(control);
            };
        }
        public void RemoveAt(int i)
        {
            Controls.Remove(lines[i]);

            lines.RemoveAt(i);

            UpdateControlsPanel();
        }
        public void Remove(Control control)
        {
            lines.Remove(control);
            Controls.Remove(control);
            UpdateControlsPanel();
        }

        public void UpdateControlsPanel()
        {
            lines.ToList().ForEach(Controls.Remove);

            var items = new List<Control>(lines);
            items.Reverse();
            foreach (var item in items)
            {
                if (!Mode) this.LayoutTop(item);
                else this.LayoutLeft(item);
            }
            if (!Fixed)
            {
                AutoScroll = false;
                if (Parent is not null && Parent.IsVerticalResizable())
                {
                    if (!Mode)
                    {
                        if (items.Count() > 0)
                            Parent.Height = items.Select(item => item.Location.Y + item.Height).Max() + 46;
                    }
                    else
                    {
                        if (items.Count() > 0)
                            Parent.Width = items.Select(item => item.Location.X + item.Width).Max() + 5;
                    }
                }
                else
                {
                    //throw new Exception($"{this.GetTypeName()} установите свойство Fixed = true для работе в этом контейнере"); 
                }
            }
            else
            {
                AutoScroll = true;
            }
        }



        internal object GetByIndex(int index)
        {
            return lines[index];
        }

        public void AddRange(IEnumerable<Control> ctrls)
        {
            lines.AddRange(ctrls);
            UpdateControlsPanel();
            foreach (var control in ctrls)
                control.ControlRemoved += (s, e) =>
                {
                    lines.Remove(control);
                };
        }
    }
}
