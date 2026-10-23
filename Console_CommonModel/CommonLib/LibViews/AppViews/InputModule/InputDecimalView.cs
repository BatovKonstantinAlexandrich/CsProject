using Console_CommonModel.CommonLib.LibViews.AppViewModules.TextBoxModule;

using System.Globalization;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.InputModule
{
    [Label("Представление поля ввода десятичного числа (по умолчанию 2 знака после запятой).")]
    public class InputDecimalView : TextBoxView
    {
        [Label("Знаков после запятой")]
        public int DecimalPlaces
        {
            get => Get<int>(nameof(DecimalPlaces));
            set => Set(nameof(DecimalPlaces), value);
        }

        public InputDecimalView()
        {
            DecimalPlaces = 2;
            Text = 0m.ToString("F2", CultureInfo.InvariantCulture);
            Height = 25;
        }
    }
}
