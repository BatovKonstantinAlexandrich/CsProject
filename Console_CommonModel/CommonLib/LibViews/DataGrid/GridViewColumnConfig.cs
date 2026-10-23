namespace Console_CommonModel.CommonLib.LibViews.DataGrid
{
    /// <summary>
    /// Конфигурация колонки GridView
    /// </summary>
    public class GridViewColumnConfig
    {
        /// <summary>
        /// Имя колонки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Заголовок колонки
        /// </summary>
        public string HeaderText { get; set; }

        /// <summary>
        /// Тип ячейки
        /// </summary>
        public GridViewCellType CellType { get; set; } = GridViewCellType.TextBox;

        /// <summary>
        /// Ширина колонки
        /// </summary>
        public int Width { get; set; } = 100;

        /// <summary>
        /// Только для чтения
        /// </summary>
        public bool ReadOnly { get; set; } = false;

        /// <summary>
        /// Видимость колонки
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Для ComboBox: список элементов
        /// </summary>
        public List<string> ComboBoxItems { get; set; } = new List<string>();

        /// <summary>
        /// Для Button: текст кнопки
        /// </summary>
        public string ButtonText { get; set; } = "Кнопка";

        /// <summary>
        /// Для Numeric: минимальное значение
        /// </summary>
        public decimal NumericMin { get; set; } = 0;

        /// <summary>
        /// Для Numeric: максимальное значение
        /// </summary>
        public decimal NumericMax { get; set; } = 100;

        /// <summary>
        /// Для Numeric: шаг изменения
        /// </summary>
        public decimal NumericIncrement { get; set; } = 1;
    }
}



