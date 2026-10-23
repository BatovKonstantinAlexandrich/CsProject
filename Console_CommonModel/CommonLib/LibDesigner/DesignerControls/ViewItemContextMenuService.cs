using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

/// <summary>
/// Используется для объединения контекстного меню 
/// с родительскими представлениями при добавлении в Controls.
/// </summary>
[Label("Управление контекстным меню")]
public class ViewItemContextMenuService : DesignerBase
{
    private readonly ViewItem contextMenuSource;


    public ViewItemContextMenuService(ViewItem parentView) : base()
    {
        contextMenuSource = parentView;
    }


    protected override void Activating(ViewNode view)
    {
         
        var contextMenuItems = contextMenuSource.ContextMenuPublic.Items;
        contextMenuItems.OnAdded((i, p) => {
            ((ViewItem)view).ContextMenu.Items.Add((ContextView)p);
        });
        contextMenuItems.OnRemoved((i, p) => {
            ((ViewItem)view).ContextMenu.Items.Remove((ContextView)p);
        });
        Action<ViewNode> onlayout = pchild => { };
        ((ViewItem)view).OnAppendedChild += onlayout;
        this.rollback.Add(() => { ((ViewItem)view).OnAppendedChild -= onlayout; });

        //((ViewItem)view).ContextMenu.Items.AddRange(contextMenuItems);
        //foreach(ViewItem ctrl in ((ViewItem)view).Controls)
        //{
        //    var s = new ViewItemContextMenuService(ctrl);
        //    ctrl.ViewServices.Add(s);
        //    s.Activates();
        //}
        this.rollback.Add(() => {
            foreach (var item in contextMenuItems)
            {
                ((ViewItem)view).ContextMenu.Items.Remove(item);
            }
        });
         
    }
     
}
