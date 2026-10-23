[Label("Текстовое поле")]
[Description("Элемент контекстного меню в виде текстового поля для ввода значений с обработчиком изменения текста")]
public class ContextViewTextbox : ContextView
{
    public string Value { get; set; } 
    public Action<string> OnChanged { get; set; }

}
