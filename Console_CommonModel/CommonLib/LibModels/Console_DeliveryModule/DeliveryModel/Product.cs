using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Сведения по товарой позиции")]
    public class Product : BaseEntity, IOnModelCreating
    {

        public int? ProductCatalogId { get; set; }

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public ProductCatalog ProductCatalog { get; set; }

        [Label("Новый/БУ")]
        public bool IsNew { get; set; } = true;

        [Label("Цена")]
        [NotNullNotEmpty]
        public float? ProductCost { get; set; } = 0;

        [Label("Рейтинг")]
        public float ProductRate { get; set; } = 0;

        [Label("Торговое наименование")]
        [NotNullNotEmpty]
        [InputText()]
        public string ProductName { get; set; }


        [Label("Описане от производителя")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ProductDesc { get; set; }

        [Label("Характеристики товара")]
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public string ProductIndicatorsJson { get; set; } = "{}";


        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotMapped]
        public int ImageIndex { get; set; } = 0;
        public int IncImageIndex()
        {
            ImageIndex++;
            if (ProductImages is not null && ProductImages.Count() == ImageIndex)
                ImageIndex = 0;
            return ImageIndex;
        }





        public long NumberOfSales { get; set; } = 0;

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ProductVideo> ProductVideos { get; set; } = new List<ProductVideo>();

        public void OnModelCreating(ModelBuilder modelBuilder)
        {

        }

        public override string ToString()
        {
            return ProductName;
        }
    }

}
