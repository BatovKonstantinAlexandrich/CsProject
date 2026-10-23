using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Логическая бизнес функция
/// </summary>
[Label("Логическая бизнес функция")]
[Description("Функция обработки определяет протокол входящей, исходящей информации и сообщение при исключении")]
public class BusinessLogic : BusinessEntity<BusinessLogic>
{

    public int BusinessProcessId { get; set; }
    public BusinessProcess BusinessProcess { get; set; }

    public string OperationName { get; set; }


    [Label("Вход")]
    [SelectDictionary(nameof(MessageProtocol)+",Name")]
    public MessageProtocol Input { get; set; }

    [Label("Выход")]
    [NotMapped]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    public MessageProtocol Output { get; set; }

    [Label("Ошибка")]
    [NotMapped]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    public MessageProtocol Error { get; set; }
}