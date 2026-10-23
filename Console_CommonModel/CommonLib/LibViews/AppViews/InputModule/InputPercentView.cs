using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{

    [Label("Ввод процентов")]
    public class InputPercentView : TextBoxView
    {
    }

    [Label("Ввод чисел")]
    public class InputNumberView : TextBoxView
    {
    }


    [Label("Натуральное число")]
    [Description("Поле ввода натурального целого числа (1, 2, 3, …)")]
    public class InputNaturalView : TextBoxView
    {
        public InputNaturalView()
        {
            Text = "1";
            Height = 25;
        }
    }
}
