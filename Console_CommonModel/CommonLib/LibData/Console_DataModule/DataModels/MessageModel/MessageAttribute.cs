 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

[Label("Атрибут сообщения")]
public class MessageAttribute: IntegratedObject
{

    [Label("Системный")]
    [HelpMessage("С системными данными работают информационные ресурсы, пользователи не учавствуют в обмене")]
    [InputOrder(3)]
    public bool IsSystem { get; set; } = false;

    [Label("Иконка")]
    [InputIcon()]
    [InputOrder(3)]
    public string Icon { get; set; } = "help";

    [Label("Тип данных")]
    [NotNullNotEmpty("Обязательно укажите тип данных")]
    [SelectCsDataType()]
    [InputOrder(2)]
    public string CsType { get; set; } = "string";


    [Label("Тип данных")]
    [NotNullNotEmpty("Обязательно укажите тип данных")]
    [InputOrder(2)]
    public string SqlType { get; set; } = "nvarchar(max)";


    [Label("Тип ввода")]
    [NotNullNotEmpty("Обязательно укажите тип ввода")]
    [SelectInputType()]
    [InputOrder(2)]
    public string InputType { get; set; } = "text";


    [Label("Краткое описание")]
    //[NotNullNotEmpty("Кратко опишите атрибут")]
    [InputMultilineText]
    [InputOrder(3)]
    public string Description { get; set; }

 

    [Label("Методы проверки")]
    [NotMapped()]
    [NotInput]
    [InputOrder(4)]
    public List<ValidationModel> Validations { get; set; }

    [Label("Тип данных SQL Server")]
    [InputOrder(2)]
    public string SqlServerDataType { get; set; }

    [Label("Тип данных MySQL")]
    [InputOrder(2)]
    public string MySqlDataType { get; set; }

    [Label("Тип данных Postgre")]
    [InputOrder(2)]
    public string PostgreDataType { get; set; }

    [Label("Тип данных Oracle")]
    [InputOrder(2)]
    public string OracleDataType { get; set; }


    [Label("Код")]
    [NotNullNotEmpty()]
    [UniqValue()]
    [InputEngText()]
    [InputOrder(4)]
    public override string Code { get; set; }

    public MessageAttribute()
    {
    }

    public MessageAttribute(Dictionary<string, string> attributes)
    {

    }


  
}