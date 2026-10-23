namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.TextboxViewModule
{
    public class TextboxViewModel : ChangeSupport, ITextboxViewModel
    {
        public bool Enabled { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<object, PropertyChangedMessage<string>> OnTextChanged { get; set; }

        public TextboxViewModel()
        {
            Enabled = true;
            Text = "";
            Value = "";
            OnTextChanged = (s, e) =>
            {
                this.Info("Changed");
            };
        }
    }
}
