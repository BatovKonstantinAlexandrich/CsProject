using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;


/// <summary>
/// Кнопка переключатель
/// </summary>
[ViewUsageExample("var view = new ToggleView { Text = \"Кнопка - переключатель\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<toggle-view Text=\"Кнопка - переключатель\" Dock=\"Top\" />")]
[Label("Кнопка - переключатель")]
public class ToggleView: CheckboxView
{
    
    [Label("Text")]
    public string Text { get => Get<string>(nameof(Text)); set => Set<string>(nameof(Text),value); }


    
    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    [Label("On Checked")]
    public Action<bool> OnChecked { get; set; } = val => { };

    [InputBool]
    [Label("Checkable")]
    public override bool Checkable { get => base.Checkable; set => base.Checkable = value; }
    [InputBool]
    [Label("Checked")]
    public override bool Checked { get => base.Checked; set => base.Checked = value; }

    public ToggleView()
    {
        this.Text = "Надпись";
        this.Checked = false;
        this.Checkable = true;
    }
}