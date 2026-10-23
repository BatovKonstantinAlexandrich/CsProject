using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


[Label("Сообщения в группе")]
public class UserGroupMessage: BaseEntity
{
    
    public int? GroupId { get; set; }
    public virtual UserGroup Group { get; set; }


    [Label("Источник")]
    [NotNullNotEmpty("Свойство " + nameof(FromUserId) + " дожно иметь действительное значение" )]
    [NotInput("Свойство " + nameof(FromUserId) + " не вводится пользователем, оно устанавливается системой перед созданием сообщения на эл. почту пользорвателя с инструкциями по активации")]
    [NotMapped]
    public int? FromUserId { get; set; }

    [Label("Источник")]        
    [NotInput("Свойство " + nameof(FromUser) + " не вводится пользователем, оно устанавливается системой перед созданием сообщения на эл. почту пользорвателя с инструкциями по активации")]
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [NotMapped]
    public virtual UserContext  FromUser { get; set; }

   


    [Label("Создано")]
    [InputHidden()]
    public DateTime Created { get; set; } = DateTime.Now;


    [Label("Тема")]
    [NotNullNotEmpty("Необходимо указать тему сообщения")]
    public virtual string Subject { get; set; } = "Рассылка";


    [Label("Текст сообщения")]
    [InputMultilineText()]
    [NotNullNotEmpty("Необходимо ввести текст сообщения")]
    public virtual string Text { get; set; } = "Всем привет";
}