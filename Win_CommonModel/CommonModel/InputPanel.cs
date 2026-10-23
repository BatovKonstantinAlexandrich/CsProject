
using Win_BusinessApp.CommonModel;

namespace MaterialApp.Win_MaterialModule.DialogModule
{
    public class InputPanel : ControlBase
    {
        public InputPanel(Type type, Action<object, object> value)
        {
            Type = type;
            Value = value;
        }

        public Type Type { get; }
        public Action<object, object> Value { get; }
    }
}