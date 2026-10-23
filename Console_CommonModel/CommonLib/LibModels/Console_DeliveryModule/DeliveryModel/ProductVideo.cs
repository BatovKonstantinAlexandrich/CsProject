using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Видео Запись")]
    public class ProductVideo : BaseEntity
    {

        [Label("Тип данных")]
        public string ContentType { get; set; }

        [Label("Данные")]
        [NotNullNotEmpty]
        [InputFileData]
        public byte[] VideoData { get; set; }

        [StringLength(40)]
        [Label("Наименование")]
        [InputRusWord]
        [NotNullNotEmpty]
        public string VideoName { get; set; }

        [InputHidden]
        public int ProductId { get; set; }

        [NotInput]
        public Product Product { get; set; }
    }
}
