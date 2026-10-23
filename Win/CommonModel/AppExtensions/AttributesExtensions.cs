using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Win_BusinessApp.CommonModel;

using WinFormsApp.AppComponents.AppControls;


public static class AttributesExtensions
{
    public static ControlBase CreateWinFormsControl(this SelectDictionaryAttribute selectDictionaryAttribute)
    {
        return new ComboboxControl();
    }
}

