using Console_CommonModel.CommonLib.LibData.AbstractionModel;

using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{


    [Label("Отзыв оставленный покупателем")]
    public class ProductComment : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int UserId { get; set; }
        public UserContext User { get; set; }


        [MinLength(10)]
        [NotNullNotEmpty]
        [InputRusText]
        public string CommentText { get; set; }


        [Range(0, 10)]
        public int CommentMark { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}