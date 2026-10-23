/// <summary>Выключатель</summary>
[ViewUsageExample("var view = new CheckboxView { Text = \"Выключатель\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<checkbox-view Text=\"Выключатель\" Dock=\"Top\" />")]
[Label("Выключатель")]
public class CheckboxView : ControlView, ICheckboxViewModel
{
    
[Label("Надпись")]
    [NotNullNotEmpty()]
    public virtual string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }
    [InputBool]
[Label("Включена/Выключена")]
    public override bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }
    
    [InputBool]
    [NotInput]
    [Label("Checked")]
    public override bool Checked { get => Get<bool>("Checked"); set => Set<bool>("Checked", value); }

 
   

    public CheckboxView()
    {
        this.SetCheckable(true);
        this.Checked = false;
        this.Enabled = true;
        this.Text = "Текст";
        
        this.OnCheckedChanged = (s, e) =>
        {
            this.Check();
            Changed = true;
            FireEvent(new PropertyChangedMessage()
            {
                Source = this,
                Property = "Enabled",
                Before = Enabled ? false : true,
                After = Enabled
            });
        };
        this.Changed = false;
        this.MinHeight = 50;
    }


    public CheckboxView OnCheckedChangedListener(Action<bool> todo)
    {
        this.AddPropertyChangeListener(nameof(Checked), evt =>
        {
            var enabled = evt.After.ToString().ToBool();
            if (todo is not null)
            {
                todo(enabled);
            }
        });
        return this;
    }

    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [NotInput]
    [Label("On Checked Changed")]
    public Action<object, PropertyChangedMessage<bool>> OnCheckedChanged { get; set; }
}