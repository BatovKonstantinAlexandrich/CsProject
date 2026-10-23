namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.InputFormsAPI
{

    [Label("Проверка возможности формирования форм ввода данных в базу")]
    public class SqlServerInputFormsApiTest : TestingElement
    {
        public SqlServerInputFormsApiTest(IServiceProvider sp) : base(sp)
        {
        }

        public override void OnTest()
        {
            Assert(
                (test) =>
                {
                    //var api = new SqlServerInputFormsApi();
                    ////api.Init();
                    //api.InitServices();
                    //var protocolsFasade = api.GetFasade<MessageProtocol>();
                    //api.Info("protocols: \n" + protocolsFasade.GetAll().ToJson());
                    //protocolsFasade.Create(new MessageProtocol() { Name = "Protocol 1" });
                    //api.Info("protocols: \n" + protocolsFasade.GetAll().ToJson());
                    using (var db = new DeliveryDbContext())
                    {
                        var procedure = db.GetProceduresMetadata().First();
                        db.CreateExecuterForm(procedure.Value.Schema.Name, procedure.Value.ProcedureName);
                    }
                    return true;
                },
                "CRUD операции работают корректно",
                "CRUD операции не работают корректно");
        }

    }
}
