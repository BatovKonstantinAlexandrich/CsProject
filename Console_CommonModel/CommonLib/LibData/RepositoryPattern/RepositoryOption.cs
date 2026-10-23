namespace Console_CommonModel.CommonLib.LibData.RepositoryPattern
{

    /// <summary>
    /// Вариант автоподстановки с информацией о релевантности
    /// </summary>
    public class RepositoryOption
    {
        /// <summary>
        /// Текст варианта продолжения
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Полный текст, из которого извлечен вариант
        /// </summary>
        public string FullText { get; set; }

        /// <summary>
        /// Релевантность варианта (чем больше, тем лучше)
        /// </summary>
        public int Relevance { get; set; }

        /// <summary>
        /// Частота встречаемости варианта
        /// </summary>
        public int Frequency { get; set; } = 1;

        /// <summary>
        /// Свойство, из которого извлечен вариант
        /// </summary>
        public string SourceProperty { get; set; }

        /// <summary>
        /// Все свойства, из которых был извлечен этот вариант
        /// </summary>
        public HashSet<string> SourceProperties { get; set; } = new HashSet<string>();
    }

}

