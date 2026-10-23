using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

using Console_CommonModel.CommonLib.LibViews.AppViews.SplitModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.SplitModule
{
    /// <summary>
    /// Две панели с перетаскиваемым разделителем (<see cref="Windows.Forms.SplitContainer"/>).
    /// </summary>
    [Label("Панель с разделителем")]
    [Description("Две области (слева/справа или сверху/снизу) с изменяемой границей.")]
    public class SplitView : AppViewBase, ILayoutViewNode
    {
        [Label("Левая / верхняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public ViewItem Left { get => Get<ViewItem>(nameof(Left)); set => Set(nameof(Left), value); }

        [Label("Правая / нижняя панель")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [InputConstructor<AppViewBase>()]
        [XmlChildNode]
        public ViewItem Right { get => Get<ViewItem>(nameof(Right)); set => Set(nameof(Right), value); }

        [Label("Ориентация")]
        [Description("Vertical — панели слева и справа; Horizontal — сверху и снизу.")]
        [InputSelect("Vertical,Horizontal")]
        public string Orientation { get => Get<string>(nameof(Orientation)); set => Set(nameof(Orientation), value); }

        [Label("Позиция разделителя (px)")]
        public int SplitterDistance { get => Get<int>(nameof(SplitterDistance)); set => Set(nameof(SplitterDistance), value); }

        [Label("Толщина разделителя (px)")]
        public int SplitterWidth { get => Get<int>(nameof(SplitterWidth)); set => Set(nameof(SplitterWidth), value); }

        public SplitView()
        {
            Orientation = "Vertical";
            SplitterDistance = 200;
            SplitterWidth = 6;
            Dock = "Fill";
            ViewDesigner = new SplitViewDesigner(this);
            AddPropertyChangeListener(OnPanelPropertyChanged);
        }

        private void OnPanelPropertyChanged(PropertyChangedMessage message)
        {
            if (message is null)
                return;

            switch (message.Property)
            {
                case nameof(Left):
                case nameof(Right):
                    var view = message.After is AppViewBase after ? after : null;
                    var before = message.Before is AppViewBase prev ? prev : null;
                    if (before is not null)
                    {
                        before.Parent = null;
                        before.OnLayoutLeft(this);
                    }
                    if (view is not null)
                    {
                        view.Parent = this;
                        view.OnLayout(this);
                    }
                    break;
            }
        }

        public void Add(ViewNode item) => Controls.Add(item);

        public void Remove(ViewNode item) => Controls.Remove(item);
    }
}
