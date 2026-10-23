using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;

/// <summary>
/// Представление с полосой прокрутки
/// </summary>
[ViewUsageExample("var view = new ScrollView { Text = \"Панель с полосой прокрутки\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<scroll-view Text=\"Панель с полосой прокрутки\" Dock=\"Top\" />")]
[Label("Панель с полосой прокрутки")]
public class ScrollView : AppViewBase, IScrollView
{

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    [Label("On Scroll Changed")]
    public override Action<object, ScrollEventArgs> OnScrollChanged { get; set; } = (s, e) => { };

    public ScrollView()
    {


    }
}
 