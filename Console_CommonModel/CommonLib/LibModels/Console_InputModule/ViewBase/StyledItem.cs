using Newtonsoft.Json;

using System.Collections.Generic;

using WinFormsApp.API;

public interface IViewPosition
{
    [InputSelect("абсолютное,относительное,гибкое")]
    [NotNullNotEmpty]
    public string PositionType { get; set; }
}



public class Flex : ChangeSupport, IViewPosition
{
    [InputSelect("абсолютное,относительное,гибкое")]
    [NotNullNotEmpty]
    public string PositionType { get; set; } = "гибкое";
}



public class Relative : ChangeSupport, IViewPosition
{

    [InputSelect("абсолютное,относительное,гибкое")]
    [NotNullNotEmpty]
    public string PositionType { get; set; } = "относительное";

    [Label("Относительная ширина (%)")]
    [NotNullNotEmpty("Не задана относительная ширина")]
    //[InputPercent("Относительная ширина задаётся как процент ( т.е. значение внутри интервала [0,100])")]
    public int Width { get => Get<int>("Width"); set => Set<int>("Width", value); }

    [Label("Относительная высота (%)")]
    [NotNullNotEmpty("Не задана относительная ширина")]
    //[InputPercent("Относительная высота задаётся как процент ( т.е. значение внутри интервала [0,100])")]
    public int Height { get => Get<int>("Height"); set => Set<int>("Height", value); }
}



[Label("Компонент представления со свойствами стилизации")]
public class StyledItem : ViewDataBuilder, IStyledItem
{

    //public bool AnchorTop { get => Get<bool>(nameof(AnchorTop)); set => Set<bool>(nameof(AnchorTop), value); }
    //public bool AnchorBottom { get => Get<bool>(nameof(AnchorBottom)); set => Set<bool>(nameof(AnchorBottom), value); }
    //public bool AnchorRight { get => Get<bool>(nameof(AnchorRight)); set => Set<bool>(nameof(AnchorRight), value); }
    //public bool AnchorLeft { get => Get<bool>(nameof(AnchorLeft)); set => Set<bool>(nameof(AnchorLeft), value); }

    public bool AnchorTop { get; set; }
    public bool AnchorBottom { get; set; }
    public bool AnchorRight { get; set; }
    public bool AnchorLeft { get; set; }

    [Label("Курсор")]
    [NotNullNotEmpty("Не задано свойство стилизации курсора")]
    [InputSelect("wait,w-resize,col-resize,grabbing,sw-resize,nw-resize,ne-resize,se-resize,e-resize,s-resize,n-resize,inherit")]
    public string Cursor { get => Get<string>(nameof(Cursor)); set => Set<string>(nameof(Cursor), value); }

    //public IViewPosition Position { get; set; } = new RelativeViewPosition();
    public bool AutoSize { get => Get<bool>(nameof(AutoSize)); set => Set<bool>(nameof(AutoSize), value); }

    [Label("Прикрепить")]
    [InputSelect("Fill,None,Top,Bottom,Left,Right")]
    //"Fill,Top,Bottom,Left,Right,None")]
    public string Dock { get => Get<string>("Dock"); set => Set<string>("Dock", value); }

    [Label("Относительная ширина (%)")]
    [NotNullNotEmpty("Не задана относительная ширина")]
    [Units("{{WidthUnit}}")]
    //[InputPercent("Относительная ширина задаётся как процент ( т.е. значение внутри интервала [0,100])")]
    public int Width { get { if (Dock == "Top" || Dock == "Bottom" || Dock == "Fill") return 100; else return Get<int>("Width"); } set => Set<int>("Width", value); }
    public string WidthUnit { get; set; } = "px";

    [Units("px")]
    public int MinWidth { get => Get<int>("MinWidth"); set => Set<int>("MinWidth", value); }
    public int MaxWidth { get => Get<int>("MaxWidth"); set => Set<int>("MaxWidth", value); }

    [Label("Относительная высота (%)")]
    [NotNullNotEmpty("Не задана относительная ширина")]
    [Units("{{HeightUnit}}")]
    //[InputPercent("Относительная высота задаётся как процент ( т.е. значение внутри интервала [0,100])")]
    public int Height { get => Get<int>("Height"); set => Set<int>("Height", value); }
    public string HeightUnit { get; set; } = "px";
    [Units("px")]
    public int MinHeight { get => Get<int>("MinHeight"); set => Set<int>("MinHeight", value); }

    [Label("Толщина контура границы")]
    [InputNumber("Нужно использовать численный тип данных")]
    [InputSelect("1,2,3,4,5")]
    [Units("px")]
    public virtual float BorderWidth { get => Get<float>("BorderWidth"); set => Set<float>("BorderWidth", value); }

    [Label("Цвет контура границы")]
    [InputColor(null)]
    public virtual string BorderColor { get => Get<string>("BorderColor"); set => Set<string>("BorderColor", value); }

    [InputNumber(null)]
    [Units("{{FontSizeUnit}}")]
    [Label("Размер шрифта")]
    [InputSelect("11,12,13,14,15,16,17,18")]
    public virtual float FontSize { get => Get<float>("FontSize"); set => Set<float>("FontSize", value); }
    public virtual string FontSizeUnit { get; set; } = "px";

    [Label("Цвет текста")]
    [InputColor(null)]
    public virtual string Color { get => Get<string>("Color"); set => Set<string>("Color", value); }

    [Label("Цвет фона")]
    [InputColor("Введите значечние цвета")]
    public virtual string BackgroundColor { get => Get<string>("BackgroundColor"); set => Set<string>("BackgroundColor", value); }

    [InputSelect("left,right,center")]
    public virtual string TextAlign { get => Get<string>("TextAlign"); set => Set<string>("TextAlign", value); }

    [Units("px")]
    public virtual int TextIndent { get => Get<int>("TextIndent"); set => Set<int>("TextIndent", value); }

    [Units("px")]
    public virtual int Margin { get => Get<int>("Margin"); set => Set<int>("Margin", value); }

    [Units("px")]
    public virtual int Padding { get => Get<int>("Padding"); set => Set<int>("Padding", value); }

    public virtual int X { get => Get<int>("X"); set => Set<int>("X", value); }
    public virtual int Y { get => Get<int>("Y"); set => Set<int>("Y", value); }


    public StyledItem() : base()
    {
        MaxWidth = 100000;
        AnchorBottom = AnchorTop = AnchorLeft = AnchorRight = false;
        FontSize = 11.0f;
        Color = "#000000";
        BackgroundColor = "#ffffff";
        BorderWidth = 0f;
        BorderColor = "#ffffff";
        Width = 100;
        Height = 25;
        this.MinHeight = this.MinWidth = 20;
        TextAlign = "left";
        Cursor = "inherit";
        Changed = false;
        Dock = "None";
        Padding = Margin = TextIndent = 0;
        AutoSize = false;
        this.X = this.Y = 0;
        //Size = new(0,0);
    }

    public StyledItem GetStyle() => (StyledItem)this;

    public string GetCssText()
    {        
        var seriallizer = new CssSerializer();
        string[] options = ReflectionService.GetOwnPropertyNames(typeof(StyledItem)).ToArray();
        Dictionary<string, object> valuesMap = new Dictionary<string, object>();
        foreach (var option in options)
            valuesMap[option] = this.GetValue(option);
        string cssText = seriallizer.Seriallize(this, valuesMap, CommonUtils.ForAllPropertiesInType(typeof(StyledItem)));
        return cssText;
    }

    public StyledItem MaxSize()
    {
        this.Width = 100;
        this.Height = 100;
        return this;
    }

    public void SetBaseTextStyle()
    {
        this.FontSize = 12;
        this.Color = "#000000";
    }

    DesignerResizeTool.Cursors IStyledItem.Cursor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}