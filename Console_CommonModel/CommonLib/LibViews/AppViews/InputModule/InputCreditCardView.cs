using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    /// <summary>
    /// Представление ввода номера кредитной карты (формат XXXX-XXXX-XXXX-XXXX).
    /// </summary>
    [Label("Номер кредитной карты")]
    public class InputCreditCardView : InputView
    {
        [Label("Номер")]
        public string Number
        {
            get => Get<string>(nameof(Number));
            set => Set(nameof(Number), value);
        }

        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [NotInput]
        public Action<string> OnNumberChanged { get; set; } = _ => { };

        public InputCreditCardView()
        {
            Number = "";
            AddPropertyChangeListener(nameof(Number), evt =>
            {
                OnNumberChanged(evt.After?.ToString() ?? "");
            });
        }

        /// <summary>Приводит строку к виду XXXX-XXXX-XXXX-XXXX (до 16 цифр).</summary>
        public static string FormatCardNumber(string text)
            => InputCreditCardAttribute.FormatCardNumber(text);

        /// <summary>Проверка формата XXXX-XXXX-XXXX-XXXX.</summary>
        public static bool TryValidate(string text, out string error)
            => InputCreditCardAttribute.TryValidate(text, out error);
    }
}
