namespace Console_CommonModel.CommonLib.LibViews.ViewEvents.List
{
    /// <summary>
    /// Событие выполнение операций над коллекции
    /// </summary>
    public class ChangingListMessage
    {
        [Label("Элемент")]
        public object Item { get; set; }

        [Label("Индекс")]
        [InputPositiveInt()]
        public int Index { get; set; }

        [Label("Операция")]
        [NotNullNotEmpty()]
        [InputSelect("added,removed")]
        public string Action { get; set; }
    }
}
