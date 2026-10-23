using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
namespace Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule
{

    [Label("Представление панели позиционирования")]
    [Description("Размещает элементы управления сверху вниз во всю ширину")]
    public class ColumnsView : AppViewBase, ILayoutViewNode
    {
        [Label("Дочерние представления")]
        [InputInterfaceCollection<AppViewBase>("GetLabel()")]
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public override ChangingList<ViewNode> Controls { get; set; } = new();

        [Label("true-с прокруткой,false-без")]
        public bool Fixed { get => Get<bool>(nameof(Fixed)); set => Set(nameof(Fixed), value); }

        [Label("true-горизонтально,false-вертикально")]
        public bool Mode { get => Get<bool>(nameof(Mode)); set => Set(nameof(Mode), value); }

        public int Count() => Controls.Count();

        public ColumnsView()
        {
            MinHeight = 320;
            MinWidth = 320;
            Fixed = false;
            Mode = false;
            DoDrop = (text, x, y) =>
            {
                if (text is not null)
                {
                    /*var message = text.FromJson<Dictionary<string, object>>();
                    var type = message["type"].ToString();
                    var dataType = type.ToType();
                    var data = message["data"].ToString();
                    var target = data.FromJson(dataType);
                    if (target is ViewItem)
                    {
                        this.Add((ViewItem)target);
                    }*/
                }
            };
        }

        public void Add(ViewNode item)
        {
            Controls.Add(item);
        }

        public void Remove(ViewNode item)
        {
            Controls.Remove(item);
        }

        public void Clear()
        {
            Controls.Clear();
        }
    }
}
