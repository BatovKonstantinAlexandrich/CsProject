namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.CheckboxViewModule
{
    public class CheckboxViewModel : ChangeSupport, ICheckboxViewModel
    {
        [Label("Надпись")]
        [NotNullNotEmpty()]
        public string Text { get => Get<string>("Text"); set => Set("Text", value); }

        [Label("Включена/Выключена")]
        public bool Enabled { get => Get<bool>("Enabled"); set => Set("Enabled", value); }

        [NotInput]
        public bool Checked { get => Get<bool>("Checked"); set => Set("Checked", value); }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<object, PropertyChangedMessage<bool>> OnCheckedChanged { get; set; }

        public CheckboxViewModel()
        {
            Text = "Включить";
            Enabled = true;
            Checked = false;
            OnCheckedChanged = (s, e) => { };
        }
    }
}
