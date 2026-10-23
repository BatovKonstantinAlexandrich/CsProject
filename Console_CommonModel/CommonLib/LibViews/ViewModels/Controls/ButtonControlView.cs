using Newtonsoft.Json;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
[Icon("home")]
[ViewUsageExample("var view = new ButtonControlView { Text = \"Кнопка\" };\nview.Dock = \"Top\";\nparent.Add(view);", "<button-control-view Text=\"Кнопка\" Dock=\"Top\" />")]
[Label("Кнопка")]
[Description(
    "По нажатию выполняет функцию заданную в переменной OnClick. " +
    "Может отображаться в 2 вариантах: надпись/иконка")]
public class ButtonControlView: ViewItem
{
[Label("Текст")]     
    [NotNullNotEmpty("Необходимо задать текст надписи на кнопки")]
    public string Label { get => Get<string>("Label"); set => Set<string>("Label", value); }
[Label("Иконка")]
    [InputIcon()]
    public string Icon { get => Get<string>("Icon"); set => Set<string>("Icon", value); }


    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public Action<ButtonControlView> OnClick { get => Get<Action<ButtonControlView>>("OnClick"); set => Set<Action<ButtonControlView>>("OnClick", value); }
[Label("Вкл/Выкл")]
    public bool Enabled { get => Get<bool>("Enabled"); set => Set<bool>("Enabled", value); }
[Label("Иконка/текст")]
    public bool IconOrText { get => Get<bool>("IconOrText"); set => Set<bool>("IconOrText", value); }
[Label("Семантическая раскраска")]
    [InputSelect("primary,info,danger,warning,secondary,success,debug")]
    
    public string BootstrapColor { get => Get<string>("BootstrapColor"); set => Set<string>("BootstrapColor", value); }




    public ButtonControlView() : base()
    {
        Tag = "button";
        BootstrapColor = "primary";
 
        UpdateClassList();
        IconOrText = false;
        Enabled = true;
        Label = "Готово";
        Icon = "home";
        this.OnClick = (button) => {
            this.Label = "Clicked";
            this.Changed = true;
        };
       
        OnEvent += (message) =>
        {
            if( message is PropertyChangedMessage)
            {
                Changed = true;
            }
        };
    }

    [OnChange("BootstrapColor")]
    public void UpdateClassList(PropertyChangedMessage evt=null){
        ClassList.Add("btn");
        ClassList.Add("btn-"+BootstrapColor);
    }


    public void Click()
    {
        if(OnClick != null)
        {
            OnClick(this);
        }
    }

    
}


