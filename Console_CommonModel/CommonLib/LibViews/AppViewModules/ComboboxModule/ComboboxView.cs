using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.ComboboxModule
{
    [Label("Выпадающий список")]
    public class ComboboxView : ControlView, IComboboxView
    {

        [Label("Надпись")]
        public override string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }

        [Label("Значение")]

        public string Value { get; set; }

        [InputPrimitiveCollection()]
        [Label("Возможные варианты выбора")]
        public ChangingList<string> Options { get; set; } = new();



        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [Label("On Selected")]
        public Action<string> OnSelected { get; set; }


        public ComboboxView()
        {
            OnSelected = text => { };
            Text = "Наименование";
            Enabled = true;
            Value = null;
            MinHeight = 50;
            Options.Add("Наименование 1");
            Options.Add("Наименование 2");
        }
    }
}
