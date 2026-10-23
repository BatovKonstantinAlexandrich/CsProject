using Console_CommonModel.CommonLib.LibViews.AppViews.FormModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Представление для ввода цвета")]
    public class InputColorView : InputView
    {

        [Label("Color")]
        public string Color { get => Get<string>(nameof(Color)); set => Set(nameof(Color), value); }

        [Label("On Color Changed")]
        public Action<string> OnColorChanged { get; set; } = text => { };
        public InputColorView()
        {
            AddPropertyChangeListener(nameof(Color), evt =>
            {
                var color = evt.After.ToString();
                OnColorChanged(color);
            });
        }
    }
}
