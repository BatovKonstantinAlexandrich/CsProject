using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Сведения о продукции имеющейся в наличии на складе")]
    public class ProductsInStock : BaseEntity
    {

        [InputDictionary($"{nameof(Holder)},{nameof(Holder.HolderLocation)}")]
        public int HolderId { get; set; }
        public Holder Holder { get; set; }

        [InputDictionary($"{nameof(Product)},{nameof(Product.ProductName)}")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [InputPositiveInt()]
        public int ProductCount { get; set; }

        /// <summary>
        /// Допустимое количество
        /// </summary>
        [InputPositiveInt()]
        public int StoreSize { get; set; } = 0;

        [InputPositiveInt()]
        public int ProductsInReserve { get; set; } = 0;
    }





}
