using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations.Schema;

[Label("Роль пользователя в системе")]
public class BusinessResource : IntegratedObject
{
    [NotInput]
    [Label("Родитель")]
    [InputDbContextDictionary<DeliveryDbContext>($"{nameof(BusinessResource)},Name")]
    public int? ParentId { get; set; }

    [Label("Наименование")]
    [InputOrder(1)]
    [InputEngWord]
    //[UniqValue]
    [NotNullNotEmpty(ErrorMessage = "Необходимо указать наименование")]
    public override string Name { get; set; }

    [Label("Код")]
    [NotNullNotEmpty()]
    //[UniqValue()]
    [InputEngWord()]
    [InputOrder(4)]
    public override string Code { get; set; }

    [Label("Родитель")]
    [NotInput]
    [InputHidden("true")]
    public virtual BusinessResource Parent { get; set; }

    [NotInput]
    [Label("Функции")]
    [NotMapped]
    public List<BusinessFunction> Functions { get; set; }

    [NotInput]
    [Label("Сервисы")]
    [NotMapped]
    public List<ServiceContext> Services { get; set; }

    public BusinessResource() : base()
    {

    }
}
