using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


[Label("Сообщения в группе")]
public class ServiceGroupMessage: BaseEntity
{
    [Label("Группа сервисов")]
    public int ServiceGroupId { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
 
    [NotInput]
    public ServiceGroup ServiceGroup { get; set; }


    [Label("Создано")]
    [InputHidden()]
    public DateTime Created { get; set; } = DateTime.Now;

    [Label("Текст сообщения")]
    [InputMultilineText()]
    [NotNullNotEmpty("Необходимо ввести текст сообщения")]
    public virtual string Text { get; set; } = $"Текст сообщения";
}