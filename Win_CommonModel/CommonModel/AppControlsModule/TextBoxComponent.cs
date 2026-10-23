using WinFormsApp.AppComponents.AppControls;
using WinFormsApp.AppViewModules.TextBoxModule;

namespace WinFormsApp.AppComponents.AppControlsModule
{
    public class TextBoxComponent : ComponentBase<TextBoxView, TextBoxControl>
    {
        public TextBoxComponent([NotNullNotEmpty] TextBoxView ViewModel, [NotNullNotEmpty] TextBoxControl Control) : base(ViewModel, Control)
        {
        }

        public override void Bind()
        {
            var textboxControl = GetControl();
            var textboxView = GetViewModel();
            textboxControl.OnEnter += (s, e) =>
            {
                textboxView.OnClick(s, e);
            };
            textboxView.Height = textboxControl.Height;
            var textbox = textboxControl.textbox;
            textboxControl.textbox.Text = textboxView.Text;
            textboxView.AddPropertyChangeListener(nameof(TextBoxView.Text), evt => {
                if($"{evt.After}"!= textboxControl.textbox.Text)
                    textboxControl.textbox.Text = $"{evt.After}";
            });
            textboxControl.textbox.TextChanged += (s,e) =>
            {
                if (textboxView.Text != textbox.Text)
                {
                    //textboxView.OnTextChanged(textboxView.Text = textbox.Text);
                    textboxView.Text = textbox.Text;
                }
            };
            //textbox.Click += (s, e) => { textboxView.OnClick(s,e); };
            textboxView.AddPropertyChangeListener(nameof(textboxView.Text), evt =>
            {
                if(textboxControl.textbox.Text != $"{evt.After}")
                {
                    textboxControl.textbox.Text = $"{evt.After}";
                    textboxControl.textbox.Text = $"{evt.After}1";
                    textboxControl.textbox.Text = $"{evt.After}23";
                }
            });
            textboxView.OnTextChanged += (text) =>
            {
                //if (textboxView.Text != textbox.Text) 
                //    textboxControl.textbox.Text = textboxView.Text; 
            };
        }
    }

}
