using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

[Label("Представление Grid")]
public class GridView : AppViewBase
{

    public GridView()
    {
        this.MinHeight = this.MinWidth = 120;
        this.BorderWidth = 1;
    }
}