using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
/// <summary>
/// Модель представления для кнопки с действием по нажатию
/// </summary>
[ViewUsageExample("var view = new ButtonView { Text = \"Кнопка\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<button-view Text=\"Кнопка\" Dock=\"Top\" />")]
[Label("Кнопка")]
[Description("Модель представления для кнопки с действием по нажатию")]
public class ButtonView : AppViewBase, IButtonView
{
    [Label("Надпись")]
    [NotNullNotEmpty()]
    public string Text { get => Get<string>("Text"); set => Set<string>("Text", value); }


    [Label("Включена/Выключена")]
    public bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }


    [Label("Иконка")]
    [InputIcon()]
    public string Icon { get => Get<string>("Icon"); set => Set<string>("Icon", value); }


    [Label("Только иконка")]
    public bool IconOrText { get => Get<bool>("IconOrText"); set => Set<bool>("IconOrText", value); }

    [NotInput]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [Label("OnClick")]
    public override System.Action<object, EventArgs> OnClick { get; set; }     

    public ButtonView()
    {        
        this.Enabled = true;
        this.MinHeight = 20;
        this.MinWidth = 80;
        this.Width = MinWidth;
        this.Height = Math.Max(Height, 36);
        
        this.Text = "Отмена";
        this.Icon = "home";
        this.IconOrText = false;
        this.Enabled = true;
        this.OnClick = ( s, e ) =>
        {
        };
        this.AddPropertyChangeListener<bool>(nameof(Enabled), val => {
            //this.Info(val);
        });        
    }

    public void SetText(string text) => this.Text = text;
}