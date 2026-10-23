namespace Console_CommonModel.CommonLib.LibViews.Others
{
    [Label("Кнопка на панели инструментов")]
    public class ToolsItemButtonView : ToolsViewItemBase
    {

        [Label("Text")]
        public string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }

        [Label("Icon")]
        public string Icon { get => Get<string>(nameof(Icon)); set => Set(nameof(Icon), value); }
        [InputBool]
        [Label("Enabled")]
        public bool Enabled { get => Get<bool>(nameof(Enabled)); set => Set(nameof(Enabled), value); }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action OnItemClicked = () => { };
        public ToolsItemButtonView()
        {
            Enabled = true;
            Icon = null;
            Text = "";
            ToolType = ToolsViewItemType.Button;
        }
    }
}
