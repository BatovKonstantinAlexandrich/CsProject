using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule
{
    /// <summary>
    /// Элемент цепочки навигации (хлебная крошка).
    /// </summary>
    [Label("Элемент хлебных крошек")]
    public class BreadcrumbItemView : AppViewBase
    {
        [Label("Надпись")]
        [InputRusWord()]
        public string Label
        {
            get => Get<string>(nameof(Label));
            set => Set(nameof(Label), value);
        }

        [Label("Ссылка")]

        public string Href
        {
            get => Get<string>(nameof(Href));
            set => Set(nameof(Href), value);
        }

        [Label("Текущий элемент")]
        [InputBool()]
        public bool IsActive
        {
            get => Get<bool>(nameof(IsActive));
            set => Set(nameof(IsActive), value);
        }

        public BreadcrumbItemView()
        {
            Label = "Раздел";
            Href = "/";
            IsActive = false;
        }

        public BreadcrumbItemView(string label, string href = null, bool isActive = false)
        {
            Label = label;
            Href = href ?? label;
            IsActive = isActive;

        }
    }
}
