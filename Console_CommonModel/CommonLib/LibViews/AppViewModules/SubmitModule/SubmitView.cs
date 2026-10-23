using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
[ViewUsageExample("var view = new SubmitView { Text = \"Панель подтверждения\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<submit-view Text=\"Панель подтверждения\" Dock=\"Top\" />")]
[Label("Панель подтверждения")]
public class SubmitView : ControlView
{

    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    [Label("oncompleted")]
    public Action<bool, AppViewBase> oncompleted { get; set; }
    [InputBool]
    [Label("Show Buttons")]
    public bool ShowButtons { get => Get<bool>(nameof(ShowButtons)); set => Set<bool>(nameof(ShowButtons), value); }

    [InputBool]
    [Label("Validation Success")]
    public override bool ValidationSuccess { get => Get<bool>(nameof(ValidationSuccess)); set => Set<bool>(nameof(ValidationSuccess), value); }

    public SubmitView() : this(new AppViewBase(), null) { }
    public SubmitView(AppViewBase control, Action<bool, AppViewBase> oncompleted = null)
    { 
        if(control is null) 
            throw new ArgumentNullException(nameof(control));
        this.oncompleted = oncompleted is null ? (s, p) => { } : oncompleted;
        this.ShowButtons = true;
        this.AutoSize = false;
        this.SetView(control);          
   }

    public void SetView(AppViewBase ctrl)
    {
        this.LayoutFill(ctrl); 
    }     
}