using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Newtonsoft.Json;

/// <summary>
/// Ссылка на метод действия
/// </summary>
[Label("Ссылка на метод действия")]
[Description("По нажатию на ссылку должно быть выполнена операция, определённая по Href")]
public class LinkViewModel: AppViewBase, ILink
{
    [Label("Иконка")]
    [InputIcon()]
    public string Icon { get; set; }

    [Label("Надпись")]
    [InputRusWord()]
    public string Label { get; set; }

    [Label("Ссылка")]
    [NotNullNotEmpty()]
    
    public string Href { get; set; }

    [Label("Активность")]
    [InputBool()]
    public bool IsActive { get; set; } = false;

    [Label("Подсказка")]
    [InputRusText()]
    [InputMultilineText()]
    public string Tooltip { get; set; }

    [Label("Дочерние элементы")]
    [InputStructureCollection(type: nameof(LinkViewModel))]
    public List<ILink> ChildLinks { get; set; } = new();

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInputAttribute]
    [Label("Item")]
    public object Item { get; set; }
}