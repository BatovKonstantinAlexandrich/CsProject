[Label("Группа служб")]
public class ServiceGroup: NamedObject
{

    [Label("Сообщения")]
    [InputOrder(2)]    
    public List<ServiceGroupMessage> ServiceGroupMessages { get; set; }
}