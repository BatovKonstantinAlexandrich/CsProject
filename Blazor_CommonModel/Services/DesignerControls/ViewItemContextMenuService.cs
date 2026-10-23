using WinFormsApp.AppViews.ApiServices;

/// <summary>
/// Используется для объединения контекстного меню 
/// с родительскими представлениями при добавлении в Controls.
/// </summary>
[Label("Управление контекстным меню")]
public class ViewItemContextMenuService : DesignerBase
{
    private readonly ViewItem contextMenuSource;


    public ViewItemContextMenuService() : this(new ButtonView()) { }

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
         
        ((ViewItem)view).ContextMenu.Items.AddRange(contextMenuItems);
        this.rollback.Add(() => {
            foreach (var item in contextMenuItems)
            {
                ((ViewItem)view).ContextMenu.Items.Remove(item);
            }
        });
         
    }
     
}
