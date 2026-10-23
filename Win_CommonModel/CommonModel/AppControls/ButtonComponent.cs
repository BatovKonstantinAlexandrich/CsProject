using WinFormsApp.Icons;
using WinFormsApp.SharedViewControls;

using WinFormsApp.AppViewModules.ButtonModule;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ButtonComponent : ComponentBase<ButtonView, ButtonControl>
    {
        public ButtonComponent() : base(new(), new())
        {
        }

        public ButtonComponent([NotNullNotEmpty] ButtonView Model, [NotNullNotEmpty] ButtonControl View) : base(Model, View)
        {
        }

        private void BindEnabled(ButtonView buttonView, ButtonControl buttonViewControl)
        {
            buttonViewControl.Enabled = buttonView.Enabled;
            buttonView.AddPropertyChangeListener(nameof(buttonView.Enabled), evt =>
            {

                this.Info(evt.After);
                buttonViewControl.button.Enabled = evt.After.ToString().ToBool();
            });
            buttonView.AddPropertyChangeListener<bool>(nameof(buttonView.Enabled), val => buttonViewControl.button.Enabled = val);
        }

        public override void Bind()
        {
            var buttonView = GetViewModel();
            var buttonControl = GetControl();
            var iconManager = new IconsManager();



            BindEnabled(buttonView, buttonControl);
            BindIcon(buttonView, buttonControl, iconManager);
            BindText(buttonView, buttonControl);

            buttonView.AddPropertyChangeListener("IconOrText", evt =>
            {
                var IconOrText = evt.After.ToString().ToBool();
                if (IconOrText)
                {
                    ((ButtonControl)Control).button.Text = "";
                }
                else
                {
                    ((ButtonControl)Control).button.Text = buttonView.Text;
                }
            });

            buttonControl.button.Click += (s, e) =>
            {
                if (GetViewModel().OnClick is not null)
                {
                    GetViewModel().OnClick(this, new EventArgs());
                }
            };
        }

        private void BindText(ButtonView buttonView, ButtonControl buttonControl)
        {
            buttonControl.button.Text = buttonView.Text;
            buttonView.AddPropertyChangeListener("Text", evt =>
            {
                ((ButtonControl)Control).button.Text = evt.After.ToString();
            });
        }

        private static void BindIcon(ButtonView buttonView, ButtonControl buttonControl, IconsManager iconManager)
        {
            buttonControl.button.Icon = iconManager.GetImageByName("card.png");
            if (!string.IsNullOrWhiteSpace(buttonView.Icon))
            {
                buttonControl.button.Icon = iconManager.GetImageByName(buttonView.Icon);
            }
            buttonView.AddPropertyChangeListener("Icon", evt =>
            {
                if (!string.IsNullOrWhiteSpace(buttonView.Icon))
                {
                    try
                    {
                        buttonControl.button.Icon = iconManager.GetImageByName(buttonView.Icon);
                    }
                    catch  
                    {
                        buttonControl.button.Icon = null;
                    }
                }
                else
                {
                    buttonControl.button.Icon = null;
                }
            });
        }
    }
}
