using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.ComponentModel.DataAnnotations;

[Label("Проверка достоверности данных")]
public class ValidationModel: BaseEntity
{
    [Label("Наименование")]
    [HelpMessage("Используется в дальнейшем для доп. валидации")]
    [UniqValue]
    [NotNullNotEmpty]
    public string ValidationName { get; set; } = "ValidationName";

    [Label("Код JavaScript")]
    [InputMultilineText]
    [Required]
    public string JavaScript { get; set; } = "function validate(valuue){ return null; }";

    public ValidationModel()
    {
    }
}
