using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO
{
    public class ChangingDataListTest : TestingElement
    {
        public ChangingDataListTest(IServiceProvider sp) : base(sp)
        {
        }

        public override void OnTest()
        {

            using (var db = new DeliveryDbContext())
            {
                var api = db.GetSqlServerWebApi();
                api.AddEntityType<StatisticsDataset>();
                api.Init();
                api.UpdateDatabase();

                var fasade = api.GetFasade<StatisticsDataset>();
                var datalist = new ChangingDataList<StatisticsDataset>(fasade);
                var data = new StatisticsDataset() { DatasetName = "input" };
                datalist.Add(data);
                if (data.Id == 0)
                    throw new Exception("После создания сущности не назначен идентификатор");

                if (fasade.Get(data.Id).FirstOrDefault() is null)
                {
                    throw new Exception("Данные не сохранились");
                }
                data.DatasetName = "123";
                if (fasade.Get(data.Id).FirstOrDefault().DatasetName != data.DatasetName)
                {
                    throw new Exception("Данные не обновились");
                }
                datalist.Remove(data);
                if (fasade.Get(data.Id).FirstOrDefault() is not null)
                {
                    throw new Exception("Данные удалились");
                }
            }
        }
    }
}
