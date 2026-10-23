namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.TextboxViewModule
{
    public interface ITextboxViewModel : IChangeSupport
    {
        public bool Enabled { get; set; }

        public string Value { get; set; }

        public Action<object, PropertyChangedMessage<string>> OnTextChanged { get; set; }
    }
}
