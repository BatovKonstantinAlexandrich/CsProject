using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.ComboboxViewModule
{
    public interface IComboboxViewModel : IChangeSupport
    {
        [Label("Надпись")]
        [NotNullNotEmpty()]
        public virtual string Text { get => Get<string>("Text"); set => Set("Text", value); }

        [Label("Включена/Выключена")]
        public virtual bool Enabled { get => Get<bool>("Enabled"); set => Set("Enabled", value); }

        public string Selected { get; set; }
        public ChangingList<string> Options { get; set; }
        public Action<object, PropertyChangedMessage<string>> OnSelectionChanged { get; set; }
    }
}
