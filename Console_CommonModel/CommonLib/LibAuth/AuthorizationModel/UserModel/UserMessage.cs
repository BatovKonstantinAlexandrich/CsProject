

using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.ComponentModel.DataAnnotations.Schema;

[Label("Прикрепление к сообщению")]
public class UserMessageFile: BaseEntity
{
    [Label("Сообщение")]
    public int UserMessageId { get; set; } = 1;

     
    public UserMessage UserMessage { get;set; }

    [Label("Тип контента")]
    public string ContentType { get; set; } = "applciation/text";

    [Label("Имя файла")]
    public string FileName { get; set; } = "readme.txt";

    [Label("Бинарные данные")]
    public byte[] FileData { get; set; } = new byte[] { 0 };

    [Label("Готовность")]
    public bool? IsReaded { get; set; } = false;

    [Label("Зарегистрировано")]
    public DateTime Created { get; set; } = DateTime.Now;
}

[Label("Сообщение")]
[Icon("drafts")]
[SearchTerms(nameof(Subject) + ","+ nameof(Text))]
public class UserMessage : BaseEntity
{
    public UserMessage(): base()
    {           
    }

    [Label("Источник")]
    [NotNullNotEmpty("Свойство " + nameof(FromUserId) + " дожно иметь действительное значение" )]
    [NotInput("Свойство " + nameof(FromUserId) + " не вводится пользователем, оно устанавливается системой перед созданием сообщения на эл. почту пользорвателя с инструкциями по активации")]    
    [InputDictionary($"{nameof(UserContext)},{nameof(UserContext.AccountId)}")]
    public int? FromUserId { get; set; }

    [Label("Источник")]        
    [NotInput("Свойство " + nameof(FromUser) + " не вводится пользователем, оно устанавливается системой перед созданием сообщения на эл. почту пользорвателя с инструкциями по активации")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotMapped]
    public virtual UserContext  FromUser { get; set; }

    [Label("Назначение")]
    [NotNullNotEmpty]
    [InputDictionary(nameof(UserContext) + ",Person.GetFullName()")]   
    public int? ToUserId { get; set; }

        
    [Label("Создано")]
    [InputHidden()]
    public DateTime Created { get; set; } = DateTime.Now;


    [Label("Тема")]
    [NotNullNotEmpty("Необходимо указать тему сообщения")]
    public virtual string Subject { get; set; } = "Личное";


    [Label("Текст сообщения")]
    [InputMultilineText()]
    [NotNullNotEmpty("Необходимо ввести текст сообщения")]
    public virtual string Text { get; set; } = "Текст";

    public bool Readed { get; set; } = false;

 
}
 
