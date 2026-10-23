
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;

[Label("Группа радиокнопок")]
[Description("Элемент контекстного меню в виде группы радиокнопок для выбора одного значения из списка опций с обработчиком изменения выбора")]
public class ContextViewRadioGroup : ContextView
{
    public string Value { get; set; }
    public ChangingList<string> options { get; set; } = new();

    [NotInput]
    public Action<string> OnChanged { get; set; } = p => { };

}
