using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.ColumnsModule;

[Label("Боковые панели")]
public class PanelsView : ViewItem, ILayoutViewNode, IPanelsView
{
    [Label("Левая панель")]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [InputConstructor<AppViewBase>()]
    [XmlChildNode]
    public virtual ViewItem Left { get => Get<ViewItem>("Left"); set => Set<ViewItem>("Left", value); }


    [Label("Правая панель")]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [InputConstructor<AppViewBase>()]
    [XmlChildNode]
    public virtual ViewItem Right { get => Get<ViewItem>("Right"); set => Set<ViewItem>("Right", value); }


    [Label("Верхняя панель")]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [InputConstructor<AppViewBase>()]
    [XmlChildNode]
    public virtual ViewItem Top { get => Get<ViewItem>("Top"); set => Set<ViewItem>("Top", value); }

    [Label("Нижняя панель")]
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [InputConstructor<AppViewBase>()]
    [XmlChildNode]
    public virtual ViewItem Bottom { get => Get<ViewItem>("Bottom"); set => Set<ViewItem>("Bottom", value); }


    public PanelsView() : base()
    {
        this.Left = this.Right = this.Top = this.Bottom = null;
        this.AddPropertyChangeListener(ListenChanges);
    }

    /// <summary>
    /// Необходимо для корректной работы ComponentsFactory
    /// </summary>    
    private void ListenChanges(PropertyChangedMessage message)
    {
        if (message is null)
            return;
        switch (message.Property)
        {
            case nameof(Top):
            case nameof(Left):
            case nameof(Right):
            case nameof(Bottom):
                AppViewBase view = message.After is null || !(message.After is AppViewBase) ? null : (AppViewBase)message.After;
                AppViewBase before = message.Before is null || !(message.Before is AppViewBase) ? null : (AppViewBase)message.Before;
                if (before is not null)
                {
                    before.Parent = null;
                    before.OnLayoutLeft(this);
                }
                if (view is not null)
                {
                    view.Parent = this;
                    view.OnLayout(this);
                }
                break;
        }
    }

    public virtual void Add(ViewNode item)
    {
        this.Controls.Add(item);
    }

    public virtual void Remove(ViewNode item)
    {
        this.Controls.Remove(item);
    }
}

