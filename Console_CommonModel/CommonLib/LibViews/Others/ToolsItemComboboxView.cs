using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.Others
{

    /// <summary>
    /// Выпадающий список на панелиь инструментов
    /// </summary>
    [Label("Выпадающий список панели инструментов")]
    public class ToolsItemComboboxView : ToolsViewItemBase
    {
        [Label("Value")]

        public string Value { get; set; }

        [Label("Options")]
        public ChangingList<string> Options { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<string> OnSelect = (text) =>
        {
        };


        public ToolsItemComboboxView()
        {
            ToolType = ToolsViewItemType.Combobox;
        }
    }


    [Label("Переключатель на панели инструментов")]
    public class ToolsItemRadioView : ToolsItemComboboxView
    {
        public ToolsItemRadioView()
        {
            ToolType = ToolsViewItemType.Radio;
        }
    }
}
