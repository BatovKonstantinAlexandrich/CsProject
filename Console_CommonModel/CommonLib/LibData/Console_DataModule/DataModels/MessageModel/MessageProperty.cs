using Newtonsoft.Json;

[Icon("message")]
[Label("Поле сообщения")]
public class MessageProperty: NamedObject
{
    [InputOrder(2)]
    [Label("Надпись")]
    [HelpMessage("Надпись располагается рядом над элементом ввода")]
    [NotNullNotEmpty("Введите ")]
    [InputText()]    
    public string Label { get; set; }


    [Label("Имя в наборе данных")]
 
    [HelpMessage("Имя свойства сообщения является идентификатором в наборе данных")]
    [NotNullNotEmpty("Введите имя свойства сообщения")]
    [InputEngText("Используйте латиницу для имени свойства сообщения")]
    [InputOrder(1)]
    public override string Name { get; set; }


    [Label("Порядковый номер")]
    [InputOrder(3)]
    public int ZOrder { get; set; } = 0;


    [Label("Подсказка")]
    [InputRusText("Используйте русскую кирилицу")]
    [InputOrder(4)]
    public string Help { get; set; } = "Подсказка";

    [Label("Обязательное")]
    [InputOrder(3)]
    [InputBool]
    public bool Required { get; set; } = false;

    [Label("Уникальное")]
    [InputOrder(4)]
    [InputBool]
    public bool Unique { get; set; } = false;

    [Label("Создание индекса")]
    [HelpMessage("Индексируемые поля являются ключами для поиска")]
    [InputOrder(5)]
    public bool Index { get; set; } = false;



    [Label("Атрибут")]
    [SelectDictionary(nameof(MessageAttribute) + ",Name")]
    //[NotNullNotEmpty("Необходимо выбрать атрибут")]
    [InputOrder(6)]
    public int? AttributeId { get; set; } 

    [Label("Атрибут")]
    [JsonIgnore()]
    [NotInput("Свойство "+nameof(Attribute) + " не вводится пользователем, оно устанавливается в соответвии с внешним ключом " + nameof(AttributeId))]
    public virtual MessageAttribute Attribute { get; set; }



    [Label("Протокол сообщения")]
    [SelectDictionary(nameof(MessageProtocol) + ",Name")]
    [NotNullNotEmpty("Необходимо выбрать протокол")]
    [InputOrder(7)]
    public int? MessageProtocolId { get; set; } = null;
    
    [Label("Протокол сообщения")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotInput("Свойство " + nameof(MessageProtocol) + " не вводится пользователем, оно устанавливается в соответвии с внешним ключом " + nameof(MessageProtocolId))]
    public virtual MessageProtocol MessageProtocol { get; set; }


     
}
