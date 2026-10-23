using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static System.Console;
using static Newtonsoft.Json.JsonConvert;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using pickpoint_delivery_service;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;


/// <summary>
/// Постамат/Устройтво хранения
/// </summary>
[Label("Устройство складирования/хранения" +
    " ")]
public class Holder : BaseEntity
{


    /// <summary>
    /// Номер постомата
    /// </summary>
    [SerialNumber("XXXX-XXX")]
    public string HolderSerial { get; set; }

    public bool HolderIsActive { get; set; } = false;
    public string HolderLocation { get; set; } = "";
    public string HolderToken { get; set; } = "";

    public int UserId { get; set; } // ид-пользователя, хозяина склада

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<Order> HolderOrders { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<ProductsInStock> ProductsInStock { get; set; }
}
