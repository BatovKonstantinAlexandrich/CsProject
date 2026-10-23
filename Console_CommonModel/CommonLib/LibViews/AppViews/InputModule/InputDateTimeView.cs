namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Ввод даты/времени")]
    public class InputDateTimeView : ControlView
    {

        [Label("Value")]
        [InputDateTime]
        public DateTime? Value { get; set; }



        [Label("On Value Changed")]
        public Action<DateTime?> OnValueChanged { get; set; } = dt => { };


        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<string> OnInput = (text) => { };



        [Label("Mode")]
        public InputDateTimeViewMode Mode { get; set; } = InputDateTimeViewMode.Day;


    }
}

public enum InputDateTimeViewMode { Year, Month, Day }
