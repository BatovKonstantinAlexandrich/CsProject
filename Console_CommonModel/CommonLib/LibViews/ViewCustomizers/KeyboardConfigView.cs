using Console_CommonModel.ActionModels;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.ViewCustomizers
{
    [Label("Представление настройки управления клавиатурой")]
    public class KeyboardConfigView : AppViewBase
    {
        private ChangingList<Keys> Pressed = new();

        public KeyboardConfigView()
        {
            OnKeyDown += (s, e) =>
            {
                Pressed.Add(e.KeyCode);
            };
            OnKeyUp += (s, e) =>
            {
                Pressed.Remove(e.KeyCode);
            };
        }
    }
}
