using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule
{
    [Label("Представление поля ввода")]
    public class TextBoxView : InputView, ITextBoxView
    {

        [Label("Text")]
        public override string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }
        public string Help { get => Get<string>(nameof(Help)); set => Set(nameof(Help), value); }

        public string InputType { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<string> OnTextChanged = test => { };


        public TextBoxView() : base()
        {
            Text = "Текст";
            this.Help = "";
            Height = 25;
            AddPropertyChangeListener(nameof(Text), evt =>
            {
                OnTextChanged($"{evt.After}");
            });
        }

        Action<string> ITextBoxView.OnTextChanged { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
