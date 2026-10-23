using System;

namespace Console_CommonModel.CommonLib.Others
{
    /// <summary>
    /// Информация о параметре метода
    /// </summary>
    public class MethodParameterInfo
    {
        /// <summary>
        /// Имя параметра
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Тип параметра
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Значение параметра (если доступно)
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MethodParameterInfo()
        {
            Name = string.Empty;
            Type = string.Empty;
            Value = string.Empty;
        }

        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        public MethodParameterInfo(string name, string type, string value = null)
        {
            Name = name ?? string.Empty;
            Type = type ?? string.Empty;
            Value = value ?? string.Empty;
        }

        /// <summary>
        /// Возвращает строковое представление параметра
        /// </summary>
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                return $"{Name}: {Type} = {Value}";
            }
            return $"{Name}: {Type}";
        }

        /// <summary>
        /// Возвращает краткое представление (только тип)
        /// </summary>
        public string ToShortString()
        {
            return Type;
        }
    }
}

