

using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;

using System.ComponentModel.DataAnnotations.Schema;

using WinFormsApp.AppComponents.AppControls;


namespace WinFormsApp.AppComponents.AppControlsModule
{


    /// <summary>
    /// Представление для управления способами размещения и позиционирования
    /// элементов.
    /// -горизонтально/вертикально
    /// -поведение при переполнении
    /// -заполнение пространства поперечной оси
    /// </summary>    
    public class LayoutControl : PanelsControl, ILayoutControl
    {
        [NotInput]
        [NotMapped]
        private List<Action> RollbackLayout = new();

        [Label("горизонтально/вертикально")]
        [InputBool]
        public bool IsVertical { get => Get<bool>(nameof(IsVertical)); set => Set(nameof(IsVertical), value); }

        [Label("перенос при переполнении")]
        [InputBool]
        public bool WrapContent { get => Get<bool>(nameof(WrapContent)); set => Set(nameof(WrapContent), value); }

        [Label("заполнить пространство")]
        [InputBool]
        public bool FillSize { get => Get<bool>(nameof(FillSize)); set => Set(nameof(FillSize), value); }


        public Panel contentPane;

        public LayoutControl()
        {
            base.Add(contentPane = new Panel() { AutoSize = false });
            contentPane.Dock = DockStyle.Fill;
            WrapContent = false;
            IsVertical = true;
            Initialized = true;
            FillSize = true;
            AutoSize = false;
            this.SetVerticalScroll();
            contentPane.SetVerticalScroll();
            //UpdateLayout();

            //AddPropertyChangeListener(nameof(IsVertical), evt => UpdateLayout());
        }


        public Action SetAutoFill(List<Control> controls)
        {
            foreach (ControlBase ctrl in controls.Where(ctrl => ctrl is ControlBase).Select(ctrl => (ControlBase)ctrl).ToList())
            {
                ctrl.FillContent = true;
            }

            if (controls.Any(ctrl => !contentPane.Contains(ctrl)))
            {
                throw new Exception("Не все элементы размещены на панели");
            }
            List<Control> c = new();
            foreach(Control p in contentPane.Controls)
            {
                c.Add(p);
            }
            var otherControls = c.Where(ctrl => !controls.Contains(ctrl)).ToList();
            foreach (ControlBase ctrl in otherControls.Where(ctrl => ctrl is ControlBase).Select(ctrl => (ControlBase)ctrl).ToList())
            {
                ctrl.FillContent = false;
            }
            Action updateSize = () =>
            {
                if (IsVertical)
                {
                    var fillingHeight = contentPane.Height - otherControls.Sum(ctrl => ctrl.Height);
                    var summFill = controls.Sum(ctrl => ctrl.Height);
                    foreach (Control control in controls)
                    {
                        control.Height = fillingHeight * (control.Height / summFill);
                    }
                    Height = fillingHeight;
                    contentPane.Height = fillingHeight;
                }
                else
                {
                    var fillingWidth = contentPane.Width - otherControls.Sum(ctrl => ctrl.Width);
                    var summFill = controls.Sum(ctrl => ctrl.Width);
                    foreach (Control control in controls)
                    {
                        control.Width = fillingWidth * (control.Width / summFill);
                    }
                    Width = summFill;
                    contentPane.Width = summFill;
                }
            };
            EventHandler evt = (s, e) =>
            {
                updateSize();
            };
            contentPane.Resize += evt;

            updateSize();
            foreach (Control control in otherControls)
            {
                contentPane.Resize += evt;
                contentPane.LocationChanged += evt;
            }
            return () =>
            {
                contentPane.Resize -= (s, e) =>
                {
                    updateSize();
                };


                foreach (Control control in otherControls)
                {
                    contentPane.Resize -= evt;
                    contentPane.LocationChanged -= evt;
                }
            };
        }


        [NotMapped]
        [NotInput]
        private bool Initialized = false;
        [OnChange(nameof(IsVertical))] public void OnIsVerticalChanged() => UpdateLayout();
        [OnChange(nameof(WrapContent))] public void OnWrapContentChanged() => UpdateLayout();
        [OnChange(nameof(FillSize))] public void OnFillSizeChanged() => UpdateLayout();


        public void UpdateLayout()
        {
            if (!Initialized)
            {
                return;
            }
            List<Control> ctrls = new();
            foreach (Control p in contentPane.Controls)
            {
                ctrls.Add(p);
            }
             
            foreach (var todo in RollbackLayout)
            {
                todo();
            }
            RollbackLayout.Clear();
            contentPane.Controls.Clear();

            if (Controls.Contains(contentPane))
            {
                Controls.Remove(contentPane);
            }
            Controls.Add(
               contentPane = WrapContent ?
                    new FlowLayoutPanel()
                    {
                        Dock = DockStyle.Fill,
                        AutoScroll = true,
                        FlowDirection = IsVertical ?
                            FlowDirection.TopDown :
                            FlowDirection.LeftToRight
                    } :
                    new Panel() { Dock = DockStyle.Fill }
                    );
            if (!WrapContent)
            {
                if (IsVertical)
                {
                    ((Panel)contentPane).SetVerticalScroll();
                }
                else
                {
                    ((Panel)contentPane).SetHorizontalScroll();
                }
            }
            foreach (var ctrl in ctrls)
            {
                AddControl(ctrl);
            }
            var fillingControls = ctrls.Where(ctrl => ctrl is ControlBase).Select(ctrl => (ControlBase)ctrl).Where(ctrl => ctrl.FillContent).Select(ctrl => (Control)ctrl).ToList();
            SetAutoFill(fillingControls);
        }

        private void AddControl(Control ctrl)
        {
            if (IsVertical)
            {
                AddVertical(ctrl);
            }
            else
            {
                AddHorizontal(ctrl);
            }
        }

        private void RemoveControl(Control ctrl)
        {
            contentPane.Controls.Remove(ctrl);
            UpdateLayout();
        }

        private void AddHorizontal(Control ctrl)
        {
            if (contentPane.Controls.Count != 0)
            {
                RollbackLayout.Add(ctrl.FixLeftRight(contentPane.Controls[contentPane.Controls.Count - 1]));
            }

            ctrl.Top = 0;

            contentPane.Controls.Add(ctrl);
            if (FillSize)
            {
                RollbackLayout.Add(ctrl.BindHeightToParent());
            }

        }

        private void AddVertical(Control ctrl)
        {
            if (contentPane.Controls.Count != 0)
            {
                RollbackLayout.Add(ctrl.FixTopBottom(contentPane.Controls[contentPane.Controls.Count - 1]));
            }
            ctrl.Left = 0;
            contentPane.Controls.Add(ctrl);
            if (FillSize)
            {
                RollbackLayout.Add(ctrl.BindWidthToParent());
            }
        }

        public void Add(Control item)
        {
            var layoutHeight = Height;
            var contentHeight = contentPane.Height;
            AddControl(item);

            // Keep height stable after adding controls.
            if (Height < layoutHeight)
            {
                Height = layoutHeight;
            }
            if (contentPane.Height < contentHeight)
            {
                contentPane.Height = contentHeight;
            }
        }

        public void Remove(Control item)
        {
            RemoveControl(item);
        }
    }




}
