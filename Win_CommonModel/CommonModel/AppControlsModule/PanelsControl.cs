using Console_CommonModel.CommonLib.LibViews.AppViewModules.LayoutModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using WinFormsApp.AppComponents.AppControls;

public class PanelsControl : ControlBase, ILayoutControl
{
    public Panel ContentView { get; set; }
     
    public PanelsControl()
    {
        this.LayoutFill(this.ContentView = new Panel());
        this.BindMouseEvents(this.ContentView);
    }
    
    public virtual void Add(IControlBase item)
        => this.ContentView.Controls.Add(item.Cast<Control>());

    public virtual void Clear()
        => this.ContentView.Controls.Clear();

    public int Count()
        => this.ContentView.Controls.Count;

    public virtual void Remove(IControlBase item)
        => this.ContentView.Controls.Remove(item.Cast<Control>());

    /*public void SetDirection(string direction)
    {
        switch (direction)
        {
            case "TopDown":
                Flow.FlowDirection = FlowDirection.TopDown;
                break;
            case "DownTop": Flow.FlowDirection = FlowDirection.LeftToRight; break;
            case "BottomUp": Flow.FlowDirection = FlowDirection.BottomUp; break;
            case "RightToLeft": Flow.FlowDirection = FlowDirection.RightToLeft; break;
            default: throw new ArgumentException(nameof(direction));
        }
    }*/

    

   
} 