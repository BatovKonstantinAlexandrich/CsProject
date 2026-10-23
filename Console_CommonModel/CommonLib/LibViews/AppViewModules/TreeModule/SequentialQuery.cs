namespace Console_CommonModel.CommonLib.LibViews.AppViewModules.TreeModule
{
    /// <summary>
    /// Класс для описания последовательного SQL запроса
    /// </summary>
    [Label("Последовательный SQL запрос")]
    [Description("Настройка SQL запроса для загрузки данных в TreeView")]
    public class SequentialQuery
    {
        /// <summary>
        /// SQL запрос
        /// </summary>
        [Label("SQL запрос")]
        [NotNullNotEmpty("Необходимо указать SQL запрос")]
        [InputMultilineText]
        [InputOrder(1)]
        public string SqlQuery { get; set; }

        /// <summary>
        /// Имя колонки с ключом
        /// </summary>
        [Label("Колонка ключа")]
        [NotNullNotEmpty("Необходимо указать колонку с ключом")]
        [InputOrder(2)]
        public string KeyColumn { get; set; }

        /// <summary>
        /// Имя колонки с текстом
        /// </summary>
        [Label("Колонка текста")]
        [NotNullNotEmpty("Необходимо указать колонку с текстом")]
        [InputOrder(3)]
        public string TextColumn { get; set; }

        /// <summary>
        /// Имя колонки с ключом родителя (опционально)
        /// </summary>
        [Label("Колонка ключа родителя")]
        [InputOrder(4)]
        public string ParentKeyColumn { get; set; }



        /// <summary>
        /// Добавлять узлы к корню TreeView
        /// </summary>
        [Label("Добавлять к корню")]
        [InputOrder(6)]
        public bool AddToRoot { get; set; } = true;

        public SequentialQuery()
        {

        }

        public SequentialQuery(string sqlQuery, string keyColumn, string textColumn, string parentKeyColumn = null)
        {
            SqlQuery = sqlQuery;
            KeyColumn = keyColumn;
            TextColumn = textColumn;
            ParentKeyColumn = parentKeyColumn;

            AddToRoot = true;
        }
    }
}

