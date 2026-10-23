namespace Console_CommonModel.CommonLib.LibViews.SharedViewCollections.DataTable
{
    /// <summary>
    /// Интерфейс для настройки ячеек GridView
    /// Типы: 
    /// -text
    /// -number
    /// -checkbox
    /// -combobox
    /// -mage
    /// -link
    /// -button
    /// 
    /// </summary>
    public interface IDataTableCustomizer
    {
        /// <summary>
        /// Устанавливает ComboBox в ячейку по номеру столбца
        /// </summary>
        void SetComboBoxColumn(int columnIndex, IEnumerable<string> items, bool readOnly = false);

        /// <summary>
        /// Устанавливает ComboBox в ячейку по имени столбца
        /// </summary>
        void SetComboBoxColumn(string columnName, IEnumerable<string> items, bool readOnly = false);

        /// <summary>
        /// Устанавливает CheckBox в ячейку по номеру столбца
        /// </summary>
        void SetCheckBoxColumn(int columnIndex, bool readOnly = false);

        /// <summary>
        /// Устанавливает CheckBox в ячейку по имени столбца
        /// </summary>
        void SetCheckBoxColumn(string columnName, bool readOnly = false);

        /// <summary>
        /// Устанавливает Button в ячейку по номеру столбца
        /// </summary>
        void SetButtonColumn(int columnIndex, string buttonText, Action<int, int> onClick = null, bool readOnly = true);

        /// <summary>
        /// Устанавливает Button в ячейку по имени столбца
        /// </summary>
        void SetButtonColumn(string columnName, string buttonText, Action<int, int> onClick = null, bool readOnly = true);

        /// <summary>
        /// Устанавливает DateTimePicker в ячейку по номеру столбца
        /// </summary>
        void SetDateTimePickerColumn(int columnIndex, bool readOnly = false);

        /// <summary>
        /// Устанавливает DateTimePicker в ячейку по имени столбца
        /// </summary>
        void SetDateTimePickerColumn(string columnName, bool readOnly = false);

        /// <summary>
        /// Устанавливает NumericUpDown в ячейку по номеру столбца
        /// </summary>
        void SetNumericColumn(int columnIndex, decimal min = 0, decimal max = 100, decimal increment = 1, bool readOnly = false);

        /// <summary>
        /// Устанавливает NumericUpDown в ячейку по имени столбца
        /// </summary>
        void SetNumericColumn(string columnName, decimal min = 0, decimal max = 100, decimal increment = 1, bool readOnly = false);

        /// <summary>
        /// Устанавливает LinkLabel в ячейку по номеру столбца
        /// </summary>
        void SetLinkColumn(int columnIndex, Action<int, int, string> onClick = null, bool readOnly = true);

        /// <summary>
        /// Устанавливает LinkLabel в ячейку по имени столбца
        /// </summary>
        void SetLinkColumn(string columnName, Action<int, int, string> onClick = null, bool readOnly = true);

        /// <summary>
        /// Устанавливает Image в ячейку по номеру столбца
        /// </summary>
        void SetImageColumn(int columnIndex, bool readOnly = false);

        /// <summary>
        /// Устанавливает Image в ячейку по имени столбца
        /// </summary>
        void SetImageColumn(string columnName, bool readOnly = false);

        /// <summary>
        /// Устанавливает кастомный тип ячейки в конкретную ячейку по координатам
        /// </summary>
        void SetCustomCell(int rowIndex, int columnIndex, Type cellType, object cellValue = null);

        /// <summary>
        /// Устанавливает кастомный тип ячейки в конкретную ячейку по координатам
        /// </summary>
        void SetCustomCell(int rowIndex, string columnName, Type cellType, object cellValue = null);

        /// <summary>
        /// Устанавливает обработчик изменения значения ячейки
        /// </summary>
        void SetCellValueChangedHandler(int columnIndex, Action<int, int, object> onValueChanged);

        /// <summary>
        /// Устанавливает обработчик изменения значения ячейки
        /// </summary>
        void SetCellValueChangedHandler(string columnName, Action<int, int, object> onValueChanged);
    }
}



