using System.ComponentModel.DataAnnotations;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class ParameterModel : IntegratedObject
    {

        [NotNullNotEmpty]
        public int ProcedureModelId { get; set; }
        public ProcedureModel ProcedureModel { get; set; }


        [Label("Наименование")]
        [InputOrder(1)]
        [UniqValue]
        [NotNullNotEmpty(ErrorMessage = "Необходимо указать наименование")]
        public override string Name { get; set; } = "";

        [Label("Наименование")]
        [InputOrder(2)]
        [UniqValue]
        [NotNullNotEmpty(ErrorMessage = "Необходимо указать код")]
        public override string Code { get; set; } = "";

        public List<ParameterAttributeModel> Attributes { get; set; }
    }
}
