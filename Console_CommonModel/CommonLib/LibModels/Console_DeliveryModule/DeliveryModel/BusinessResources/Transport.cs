
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

/// <summary>
/// Тот кто осуществляет транспортировку,
/// напр. сотрудник курьерской службы
/// </summary>
[Label("Трансопртная компания")]
public class Transport : BaseEntity
{
    [NotNullNotEmpty]
    [Label("Широта")]
    public float Latitude { get; set; }

    [NotNullNotEmpty]
    [Label("Долгота")]
    public float Longitude { get; set; }

    [NotNullNotEmpty]
    [Label("Пользователь")]
    public int? UserId { get; set; }
    public UserContext User { get; set; }
}

