using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Newtonsoft.Json;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.TextModule
{
    [Label("Текстовое представление")]
    public class TextView : AppViewBase, ITextView
    {
        public TextView()
        {
            FontSize = 14.0f;
            Color = "#000000";
            TextAlign = "left";
            TextIndent = 0;
            MinWidth = 120;
            MinHeight = 25;
            Height = MinHeight;
            HeightUnit = "px";
            Width = MinWidth;
            Text = "надпись";
        }

        [Label("Текст")]
        public string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [InputNumber(null)]
        [Units("{{FontSizeUnit}}")]
        [Label("Размер шрифта")]
        [InputSelect("11,12,13,14,15,16,17,18")]
        public override float FontSize { get => Get<float>("FontSize"); set => Set("FontSize", value); }
        public override string FontSizeUnit { get; set; } = "px";

        [Label("Выравнивание текста")]
        [InputSelect("left,right,center")]
        public override string TextAlign { get => Get<string>("TextAlign"); set => Set("TextAlign", value); }
 

        [Units("px")]
        [Label("Красная строка")]
        public override int TextIndent { get => Get<int>("TextIndent"); set => Set("TextIndent", value); }

        [Label("Цвет текста")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [InputColor(null)]
        public string Color { get => Get<string>("Color"); set => Set("Color", value); }


    }
}
