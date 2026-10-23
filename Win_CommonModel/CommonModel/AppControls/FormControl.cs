using WinFormsApp.AppViewModules.LayoutModule;
using WinFormsApp.AppViews.ColumnsModule;

using WinFormsApp.AppViews.BaseComponent;

namespace WinFormsApp.AppComponents.AppControls
{



    public class InputFormComponent : ComponentBase<InputFormModel, FormControl>
    {
        public InputFormComponent([NotNullNotEmpty] InputFormModel ViewModel, [NotNullNotEmpty] FormControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            this.GetControl().BindInputFormModel(this.GetViewModel());
        }
    }
    public class FormControl : FlexControl, ILayoutControl
    {
        private FlowLayoutPanel flowPanel;
        private readonly ControlsFactory controlsFactory = new();
        private Label titleLabel;

        public FormControl()
        {

            Margin = new(20, 20, 20, 20);
            this.SetVerticalScroll();
            this.LayoutTop(titleLabel = controlsFactory.CreateTextLabel(""));
            this.LayoutFill(flowPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                AutoScroll = false,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            });
            this.SetVerticalScroll();
            AutoSize = true;
        }

        internal void BindInputFormModel(InputFormModel view)
        {
            Controls.Clear();
            Margin = new(0, 0, 0, 0);
            Padding = new(0, 0, 0, 0);

            var panel = new Panel();
            panel.LayoutFill(flowPanel = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false, // Ключевая настройка!
                AutoScroll = true,
                AutoSize = false,
                Size = new Size(300, 200),

                // Явная настройка скролла
                HorizontalScroll = {
                    Maximum = 0,
                    Visible = false
                },

                VerticalScroll = {
                    Maximum = 1000,
                    Visible = true
                }
            });
            flowPanel.Resize += (s, e) => { flowPanel.HorizontalScroll.Visible = false; };
            foreach (var field in view.FormFields)
            {
                field.Parent = view;
                var fieldControl = field.ToControl();
                Add(fieldControl);
                fieldControl.Height = 50;
            }

            this.LayoutTop(titleLabel = controlsFactory.CreateTextLabel(view.Title, "Roboto", 16));
            this.LayoutFill(flowPanel);
        }

        public void Add(Control item)
        {
            item.Margin = new(0, 0, 0, 0);
            item.Padding = new(10, 10, 10, 10);

            //item.Width = this.flowPanel.Width - (this.flowPanel.Padding.Left * 2)-15;
            item.Width = flowPanel.Width;
            flowPanel.Resize += (s, e) => { item.Width = flowPanel.Width - flowPanel.Padding.Left * 2 - 15; item.Location = new(25, item.Location.Y); };
            flowPanel.Controls.Add(item);
            item.Location = new(25, item.Location.Y);

        }

        public void Remove(Control item)
        {
            flowPanel.Controls.Remove(item);

        }
    }
}
