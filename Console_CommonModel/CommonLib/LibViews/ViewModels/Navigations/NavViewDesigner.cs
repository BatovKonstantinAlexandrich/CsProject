using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using System.Diagnostics;

/// <summary>
/// Настройка навигации
/// </summary>
[Label("Мастер настройки навигации")]
public class NavViewDesigner: DesignerBase
{
    public NavViewDesigner(AppViewBase view) : base(view)
    {
    }

    protected override void Activating(ViewNode view)
    {
        AppViewBase navView = (AppViewBase)view;
        var remove = navView.ContextMenuPublic.AddContextServiceMethods("Навигация", this.view);
        this.rollback.Add(remove);
    }
}
