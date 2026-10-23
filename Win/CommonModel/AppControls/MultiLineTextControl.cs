using MaterialSkin.Controls;

namespace WinFormsApp.AppComponents.AppControls
{

    /// <summary>
    /// Элемент управления многострочного текстового поля
    /// </summary>
    public class MultiLineTextControl : ControlBase
    {
        private readonly MaterialMultiLineTextBox2 textbox;


        /// <summary>
        /// Связано с элементом управления, при изменении устанавливается значение
        /// свойства Text у MaterialMultiLineTextBox2 textbox.
        /// </summary>
        public string Text { get => Get<string>(nameof(Text)); set => Set(nameof(Text), value); }


        /// <summary>
        /// Вызывается при событии TextChanged MaterialMultiLineTextBox2 textbox
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Action<string> OnTextChanged = test => { };


        public MultiLineTextControl()
        {
            Text = "";
            this.LayoutFill(textbox = new MaterialMultiLineTextBox2());
            textbox.DataBindings.Add(nameof(Text), this, nameof(Text), true);
            textbox.TextChanged += (s, e) => { OnTextChanged(textbox.Text); };
            AutoSize = true;
        }
    }

}
