using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

using Newtonsoft.Json;

namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.BreadcrumbModule
{
    /// <summary>
    /// Представление цепочки навигации (breadcrumbs), аналог веб-компонента.
    /// </summary>
    [ViewUsageExample(
        "var crumbs = new BreadcrumbView();\ncrumbs.Items.Add(new BreadcrumbItemView(\"Главная\", \"/\"));\ncrumbs.Items.Add(new BreadcrumbItemView(\"Каталог\", \"/catalog\"));\ncrumbs.Items.Add(new BreadcrumbItemView(\"Товар\", isActive: true));\ncrumbs.Dock = \"Top\";\nparent.Add(crumbs);",
        "<breadcrumb-view Dock=\"Top\" Separator=\"/\" />")]
    [Label("Хлебные крошки")]
    [Description("Горизонтальная цепочка навигации с кликабельными ссылками и текущим разделом")]
    public class BreadcrumbView : AppViewBase
    {
        [Label("Элементы")]
        [InputOrder(1)]
        [InputStructureCollection(nameof(BreadcrumbItemView))]
        public ChangingList<BreadcrumbItemView> Items { get; set; } = new();

        [Label("Разделитель")]
        [InputSelect("/,>,›,→")]
        public string Separator
        {
            get => Get<string>(nameof(Separator));
            set => Set(nameof(Separator), value);
        }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotInput]
        [Label("При клике на элемент")]
        public Action<BreadcrumbItemView, int> OnItemClick { get; set; } = (_, _) => { };

        public BreadcrumbView()
        {
            Separator = "›";
            Height = 40;
            MinHeight = 40;
            Dock = "Top";
            BackgroundColor = "#F8F9FB";
            BorderWidth = 1f;
            BorderColor = "#D6DBE2";
        }

        /// <summary>
        /// Задаёт путь из пар (надпись, ссылка). Последний элемент помечается как текущий.
        /// </summary>
        public void SetPath(params (string label, string href)[] segments)
        {
            Items.Clear();
            if (segments is null || segments.Length == 0)
                return;

            for (var i = 0; i < segments.Length; i++)
            {
                var (label, href) = segments[i];
                Items.Add(new BreadcrumbItemView(label, href, i == segments.Length - 1));
            }
        }

        /// <summary>
        /// Задаёт путь только из надписей (href = label).
        /// </summary>
        public void SetPath(params string[] labels)
        {
            Items.Clear();
            if (labels is null || labels.Length == 0)
                return;

            for (var i = 0; i < labels.Length; i++)
                Items.Add(new BreadcrumbItemView(labels[i], labels[i], i == labels.Length - 1));
        }
    }
}
