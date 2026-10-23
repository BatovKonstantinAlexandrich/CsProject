namespace Console_CommonModel.CommonLib.LibViews.AppViews.TextModule
{
    public interface ITextView
    {

        [Parameter]
        [System.Text.Json.Serialization.JsonIgnore]
        [InputNumber(null)]
        [Units("px")]
        [Label("Размер шрифта")]
        [InputSelect("11,12,13,14,15,16,17,18")]
        public float FontSize { get; set; }

        [Parameter]
        [Label("Выравнивание текста")]
        [InputSelect("left,right,center")]
        public string TextAlign { get; set; }

        [Parameter]
        [Units("px")]
        [Label("Красная строка")]
        public int TextIndent { get; set; }

        [Parameter]
        [Label("Цвет текста")]
        [System.Text.Json.Serialization.JsonIgnore]
        [InputColor(null)]
        public string Color { get; set; }
    }
}