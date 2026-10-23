using MaterialSkin.Controls;

using WinFormsApp.AppComponents.AppControls;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    /// <summary>
    /// Управление текстовым полем
    /// </summary>
    public class TextBoxControl : ControlBase
    {
        public readonly MaterialTextBox textbox;
        public EventHandler OnEnter { get; set; }
        public TextBoxControl()
        {
            this.LayoutFill(textbox = new MaterialTextBox());
            OnEnter = (s, e) =>
            {
                this.Info(e);
            };
            textbox.Enter += OnEnter;
            textbox.Click += OnEnter;
            textbox.GotFocus += OnEnter;
            AutoSize = true;
            Height = textbox.Height;
            MinimumSize = new(20, 20);
        }
    }

}
