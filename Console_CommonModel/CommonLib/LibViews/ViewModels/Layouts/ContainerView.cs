using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

[Label("Контейнер компонента")]
[Description("Используется для соединения представлений")]
public class ContainerView : AppViewBase
{
    
    [Label("Модель представления")]
    public ViewItem Model
    {
        get => Get<ViewItem>(nameof(Model));
        set => Set(nameof(Model), value);
    }
    public object Service { get; internal set; }

    public ContainerView()
    {
        this.MinWidth = 160;
        this.MinHeight = 120;
        this.Width = 240;
        this.Height = 160;
        this.BorderWidth = 1f;
        this.BorderColor = "#D6DBE2";
        this.BackgroundColor = "#F8F9FB";
        this.Model = new ButtonView();
    }
}
