using WinFormsDesignerApp;
using WinFormsApp.AppViews.SideModule;
using WinFormsDesignerApp.Console_InputModule.ViewEvents.List;

namespace WinFormsApp.AppComponents.AppControls
{
    /// <summary>
    /// Панель с автоматически открывающейся верхней панелью инструментов
    /// </summary>
    public class ToolsPanel : ContentPanel
    {
        private readonly ToolStrip tools;
        public ChangingList<object> toolsServices = new();

        private bool _show_top = false;
        public bool show_top { get => _show_top; set { if ((_show_top = value) == true) { ShowTop(); } else { HideTop(); } } }

        public ToolsPanel()
        {
            var p = this;
            SetTop(tools = new ToolStrip());
            bool focused = false;
            View.MouseEnter += (s, e) =>
            {
                show_top = true;
                focused = true;
            };
            View.MouseLeave += (s, e) =>
            {
                Task.Run(() =>
                {
                    Thread.Sleep(2000);
                    if (focused == false)
                    {
                        p.Invoke(() =>
                        {
                            show_top = false;

                        });

                    }
                });
                focused = false;
            };

            toolsServices.OnChanged += OnToolsServicesChanged;
        }




        /// <summary>
        /// Событие изменения набора сервисов состав панели инструментов
        /// </summary>
        private void OnToolsServicesChanged(object sender, IChangingListEvent e)
        {
            var factory = new ToolsFactory();
            var services = new List<object>() { new ControlsFactory() };
            var items = factory.CreateToolsPaneItems(services);
            tools.Items.Clear();
            tools.Items.AddRange(items.ToArray());
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Top
            // 
            Top.Size = new Size(0, 30);
            // 
            // Bottom
            // 
            Bottom.Size = new Size(0, 30);
            // 
            // ToolsPanel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            Name = "ToolsPanel";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}