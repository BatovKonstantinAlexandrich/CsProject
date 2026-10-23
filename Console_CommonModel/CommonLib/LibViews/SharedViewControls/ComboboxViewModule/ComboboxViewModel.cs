using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.ComboboxViewModule
{
    public class ComboboxViewModel : ChangeSupport, IComboboxViewModel
    {
        [Label("Надпись")]
        [NotNullNotEmpty()]
        public string Text { get => Get<string>("Text"); set => Set("Text", value); }

        [Label("Включена/Выключена")]
        public bool Enabled { get => Get<bool>("Enabled"); set => Set("Enabled", value); }

        public string Selected { get; set; }
        public ChangingList<string> Options { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<object, PropertyChangedMessage<string>> OnSelectionChanged { get; set; }

        public ComboboxViewModel()
        {
            Selected = "";
            Options = new();
            Text = "";
            Enabled = true;
            OnSelectionChanged = (s, e) =>
            {
                this.Info("Selected");
            };
        }
    }
}
