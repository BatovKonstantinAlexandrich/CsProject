[Label("Чекбокс контекста")]
[Description("Элемент контекстного меню в виде чекбокса с поддержкой состояния выбора и обработчика изменения состояния")]
public class ContextViewCheckbox : ContextView
{
    public bool IsChecked { 
        get => Get<bool>(nameof(IsChecked)); 
        set => Set<bool>(nameof(IsChecked), value); }

    [System.Text.Json.Serialization.JsonIgnore]
    [Newtonsoft.Json.JsonIgnore]
    public Action<bool> OnCheckedChanged { get; set; } = (p) => { };

    public ContextViewCheckbox()
    {
        this.IsChecked = false;
        this.AddPropertyChangeListener(nameof(IsChecked),
             evt =>
             {
                 
                 OnCheckedChanged((bool)evt.After);
             });
        this.OnClick = (s, e) => {
            this.IsChecked = this.IsChecked? false: true;
        };
    }
}
