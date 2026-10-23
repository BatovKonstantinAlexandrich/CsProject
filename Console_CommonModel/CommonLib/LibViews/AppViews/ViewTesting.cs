using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViews
{
    public class ViewTesting<TView> : TestingService<TView>
        where TView : AppViewBase
    {
        public ViewTesting() : base()
        {
            this.Name = this.GetLabel();
        }

        public ViewTesting(IServiceProvider provider) : base(provider)
        {
            this.Name = this.GetLabel();
        }

        public string GetLabel()
        {
            return $"Тестирование представления {typeof(TView).GetTypeName()}. {typeof(TView).GetLabel()}. {typeof(TView).GetDescription()}";
        }

        public override bool OnServiceTest(TView view)
        {
            var control = new ViewTest();
            try
            {
                control.SetViewModel(view);
                control.ConvertTo<IControlBase>().ShowDialog($"{GetType().GetTypeName()}. {GetType().GetLabel()}. {GetType().GetDescription()}");
            }
            catch (Exception ex)
            {
                this.Error($"Исключение при тестировании представления {typeof(TView).GetTypeName()} {ex.Message}");
            }
            return true;
        }
    }
}
