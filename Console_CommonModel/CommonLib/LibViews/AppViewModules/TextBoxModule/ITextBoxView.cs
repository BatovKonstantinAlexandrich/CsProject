namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule
{


    public interface ITextBoxControl : ITextBoxView, IControlBase { }
    public interface ITextBoxView
    {
        public string Text { get; set; }
        public Action<string> OnTextChanged { get; set; }
    }
}