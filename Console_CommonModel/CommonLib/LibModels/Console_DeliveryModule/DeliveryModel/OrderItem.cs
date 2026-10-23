using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Позиция в заказе")]
    public class OrderItem : BaseEntity
    {
        [NotInput]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [InputDictionary($"{nameof(Product)},ProductName")]
        public int ProductId { get; set; }


        public Product Product { get; set; }
        public int ProductCount { get; set; }

        [NotNullNotEmpty]
        public string ProductSize { get; set; } = "100,100,100";

        [NotNullNotEmpty]
        public int ProductWeight { get; set; } = 1000;
    }

}
