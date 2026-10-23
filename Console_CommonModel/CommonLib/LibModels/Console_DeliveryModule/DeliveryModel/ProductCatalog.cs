using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{

    /// <summary>
    /// Группировка продукции ( 1 ко многим )   
    /// </summary>
    [Index(nameof(ProductCatalogName))]
    public class ProductCatalog : HierTable<ProductCatalog>
    {

        [Required]
        public string ProductCatalogName { get; set; }
        public virtual List<Product> Products { get; set; }


        public ProductCatalog()
        {
            Products = new List<Product>();
        }
    }
}
