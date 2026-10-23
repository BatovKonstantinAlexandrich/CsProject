using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryModel;

using System.Reflection;

namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters
{
    public class Filter : IFilter
    {
        public string name { get; set; }
        public string action { get; set; }
        public string param { get; set; }

        public string GetExpression()
        {
            switch (action)
            {
                case "in": return $"{name.ToTSQLStyle()} in @{name}";
                case "not": return $"{name.ToTSQLStyle()} not @{name}";
                case "equals": return $"{name.ToTSQLStyle()} = '{name}'";
                case "contains": return $"{name.ToTSQLStyle()} like '%{name}%'";
                case "min_range": return $"{name.ToTSQLStyle()} > @{name}'";
                default: return "1 = 1"; break;
            }
        }
    }

    public interface IFilter
    {
        public string GetExpression();

    }

    public class PriceFilter : IFilter
    {

        [NotNullNotEmpty]
        public string FilterName { get; } = nameof(PriceFilter);

        [NotNullNotEmpty]
        public float? Min { get; set; }

        [NotNullNotEmpty]
        public float? Max { get; set; }

        public string GetExpression()
        {
            return $"Price > {Min} && Price < {Max}";
        }
    }


}
