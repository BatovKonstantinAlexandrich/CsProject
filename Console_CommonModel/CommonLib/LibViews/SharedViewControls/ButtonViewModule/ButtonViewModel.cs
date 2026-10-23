using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.SharedViewControls.ButtonViewModule
{
    public class ButtonViewModel : ChangeSupport, IButtonViewModel
    {
        [Label("Надпись")]
        [NotNullNotEmpty()]
        public virtual string Text { get => Get<string>("Text"); set => Set("Text", value); }

        [Label("Включена/Выключена")]
        public virtual bool Enabled { get => Get<bool>("Enabled"); set => Set("Enabled", value); }

        [Label("Иконка")]
        [InputIcon()]
        public virtual string Icon { get => Get<string>("Icon"); set => Set("Icon", value); }

        [Label("Только иконка")]
        public virtual bool IconOrText { get => Get<bool>("IconOrText"); set => Set("IconOrText", value); }

        [NotInput]
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public override Action<object, EventArgs> OnClick { get; set; } = (s, e) => { };

        public ButtonViewModel()
        {
            Text = "Подтвердить";
            Enabled = true;
            Icon = null;
            IconOrText = false;
            OnClick = (s, e) =>
            {
                this.Info("Clicked");
            };
        }
    }
}
