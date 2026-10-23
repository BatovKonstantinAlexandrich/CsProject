namespace Console_CommonModel.CommonLib.LibData
{
    /// <summary>
    /// Базовый класс для фильтров
    /// </summary>
    public class FilterBase
    {
        public virtual string Label { get; set; }
        public virtual string Type { get; set; }
        public virtual string Property { get; set; }

        /// <summary>
        /// Label свойства
        /// </summary>
        public virtual string Indicator { get; set; }

    }


    public class FilterEquals : FilterBase
    {
        public override string Label { get; set; } = "Значение";
        public override string Type { get; set; } = nameof(FilterEquals);
        public override string Property { get; set; }
        public override string Indicator { get; set; }

        public string Value { get; set; }
    }


    public class FilterRange : FilterBase
    {
        public override string Label { get; set; } = "Диапазон";
        public override string Type { get; set; } = nameof(FilterRange);
        public override string Property { get; set; }
        public override string Indicator { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }
    }

    public class FilterSet : FilterBase
    {
        public override string Label { get; set; } = "Множество";
        public override string Type { get; set; } = nameof(FilterSet);
        public override string Property { get; set; }
        public List<string> Values { get; set; }
    }


    public class FilterNot : FilterBase
    {
        public override string Label { get; set; } = "Исключение";
        public override string Type { get; set; } = nameof(FilterNot);
        public override string Property { get; set; }
        public string Value { get; set; }
    }
}

