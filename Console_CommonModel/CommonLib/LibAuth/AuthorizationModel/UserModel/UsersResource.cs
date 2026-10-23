using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.Text.Json.Serialization;

[Label("Роли пользователей")]
public class UsersResource : BaseEntity
{
    public int RoleId { get; set; } = 1;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public BusinessResource Role { get; set; }

    public int UserId { get; set; } = 1;

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public UserContext User { get; set; }
}