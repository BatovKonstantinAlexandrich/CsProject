using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

[Label("Представление элемента управления")]
public class ControlView : AppViewBase, IControlView
{

    [Label("Надпись")]
    [NotNullNotEmpty()]
    public virtual string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }

    [InputBool]
    [Label("Включена/Выключена")]
    public virtual bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }

    public ControlView() : base()
    {
        
        this.Enabled = true;
        this.Text = "Заголовок";
    }
}