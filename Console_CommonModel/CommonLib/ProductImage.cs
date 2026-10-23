using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using Newtonsoft.Json;


[Label("Фото товара")]
public class ProductImage : BaseEntity
{

    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public string ContentType { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    [InputImage]
    public byte[] ImageData { get; set; }
}
