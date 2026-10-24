namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{


    [Label("Модель базы данных")]
    public class DatabaseModel : IntegratedObject
    {
        [NotNullNotEmpty]
        public int DatabaseConnectionId { get; set; }
        public DatabaseConnectionString DatabaseConnection { get; set; }

    }


}
