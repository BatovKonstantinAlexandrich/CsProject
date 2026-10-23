namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class ProcedureModel : IntegratedObject
    {
        [Label("Модель базы данных")]
        [InputOrder(9)]
        [InputDictionary($"{nameof(DatabaseModel)},Name")]
        [NotNullNotEmpty]
        public int DatabaseModelId { get; set; }

        [NotInput]
        public DatabaseModel DatabaseModel { get; set; }

        [Label("Наименование")]
        [UniqValue]
        [NotNullNotEmpty]
        [InputEngText]
        public override string Name { get; set; }
        public string Schema { get; set; }

        [Label("Параметры")]
        [InputOrder(10)]
        [InputStructureCollection(nameof(ParameterModel))]
        public List<ParameterModel> Parameters { get; set; } = new();
    }
}
