using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
[ViewUsageExample("var view = new ScrollableView { Text = \"Панель с прокруткой\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<scrollable-view Text=\"Панель с прокруткой\" Dock=\"Top\" />")]
[Label("Панель с прокруткой")]
public class ScrollableView: AppViewBase
{

    ///горизонтальная прокрутка или вертикальная
    [Label("Is Horizontal")]
    [InputBool]
    public bool IsHorizontal { get; set; }

    [Label("Scroll Size")]
    [InputNumber]
    public float ScrollSize { get; set; }


    public ScrollableView(){
        this.ClassList.Add("front-pane");

        Changed = false;
    }

}
