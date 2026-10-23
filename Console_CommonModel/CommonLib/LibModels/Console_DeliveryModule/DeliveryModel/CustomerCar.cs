using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel.BusinessResources;

using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel
{
    [Label("Сведения по автомобилю клиента")]
    public class CustomerCar : BaseEntity
    {

        [Label("Использовать по-умолчанию")]
        public bool Active { get; set; } = false;

        [Label("Произодитель")]
        [NotNullNotEmpty()]
        public string Manufacturer { get; set; } = "";

        [Label("Модель")]
        [NotNullNotEmpty()]
        public string Model { get; set; }

        [Label("Год выпуска")]
        public string Year { get; set; }

        [Label("ВИН")]
        [StringLength(16)]
        [MinLength(16)]
        [NotNullNotEmpty()]
        public string Vin { get; set; }

        [Label("Регистрационный номер")]
        [NotNullNotEmpty()]
        public string RegNumber { get; set; }

        [Label("Клиент")]
        [InputDictionary(nameof(BusinessResources.Customer) + ",FirstName")]
        public int CustomerId { get; set; }

        [Label("Клиент")]
        public Customer Customer { get; set; }

    }
}
