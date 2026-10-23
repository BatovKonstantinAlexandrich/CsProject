namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    public class StatisticsDataset : ChangeSupport
    {
        public string DatasetName { get => Get<string>(nameof(DatasetName)); set => Set(nameof(DatasetName), value); }

        public StatisticsDataset()
        {
            DatasetName = "Базовые показатели";
        }
    }
}
