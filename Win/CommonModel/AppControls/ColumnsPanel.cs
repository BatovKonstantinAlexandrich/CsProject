
using MaterialSkin.Controls;

using Org.BouncyCastle.Asn1.Crmf;

using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Панель, которая выполняет размещение элементов управления вертикально вниз
    /// </summary>
    public class ColumnsPanel : MaterialCard
    {
        private readonly ChangingList<Control> lines = new();

        public bool Fixed { get; set; } = false;
        public bool Mode { get; set; } = false;
        public int Count() => lines.Count;
        public ColumnsPanel()
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
            lines.ToList().ForEach(Remove);

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
            Controls.Remove(control);
            lines.Remove(control);
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

        internal void Add(object value)
        {
            throw new NotImplementedException();
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
