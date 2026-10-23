using WinFormsApp.Icons;
using Console_CommonModel.CommonLib.Others;

namespace WinFormsApp.AppComponents.AppControls
{
    public class ButtonComponent : ComponentBase<ButtonView, IButtonControl>
    {
   
        public ButtonComponent([NotNullNotEmpty] ButtonView Model, [NotNullNotEmpty] IButtonControl View) : base(Model, View)
        {
        }

        private void BindEnabled(ButtonView buttonView, IButtonControl buttonViewControl)
        {
            buttonViewControl.Enabled = buttonView.Enabled;
            buttonView.AddPropertyChangeListener(nameof(buttonView.Enabled), evt =>
            {

                //this.Info(evt.After);
                buttonViewControl.Enabled = evt.After.ToString().ToBool();
            });
            buttonView.AddPropertyChangeListener<bool>(nameof(buttonView.Enabled), val => buttonViewControl.Enabled = val);
        }

        public override void Bind()
        {
            var buttonView = GetViewModel();
            var buttonControl = GetControl();
            lock (buttonView)
            {
                if (Binded)
                {
                    return;
                }

                buttonControl.ViewModel = buttonView;
                buttonView.control = buttonControl;
                var iconManager = new IconsManager();

                BindEnabled(buttonView, buttonControl);
                BindIcon(buttonView, buttonControl, iconManager);
                BindText(buttonView, buttonControl);

                buttonView.AddPropertyChangeListener("IconOrText", evt =>
                {
                    var IconOrText = evt.After.ToString().ToBool();
                    if (IconOrText)
                    {
                        ((IButtonControl)Control).Text = "";
                    }
                    else
                    {
                        ((IButtonControl)Control).Text = buttonView.Text;
                    }
                });

                buttonControl.OnClick += (s, e) =>
                {
                    if (GetViewModel().OnClick is not null)
                    {
                        GetViewModel().OnClick(this, new EventArgs());
                    }
                };
            }
            
        }

        private void BindText(ButtonView buttonView, IButtonControl buttonControl)
        {
            buttonControl.Text = buttonView.Text;
            buttonView.AddPropertyChangeListener("Text", evt =>
            {
                ((IButtonControl)Control).Text = evt.After.ToString();
            });
        }

        private static void BindIcon(ButtonView buttonView, IButtonControl buttonControl, IconsManager iconManager)
        {
            buttonControl.Icon = iconManager.GetImageByName("card.png");
            if (!string.IsNullOrWhiteSpace(buttonView.Icon))
            {
                buttonControl.Icon = iconManager.GetImageByName(buttonView.Icon);
            }
            buttonView.AddPropertyChangeListener("Icon", evt =>
            {
                if (!string.IsNullOrWhiteSpace(buttonView.Icon))
                {
                    try
                    {
                        buttonControl.Icon = iconManager.GetImageByName(buttonView.Icon);
                    }
                    catch  
                    {
                        buttonControl.Icon = null;
                    }
                }
                else
                {
                    buttonControl.Icon = null;
                }
            });
        }
    }
}
