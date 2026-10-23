namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public interface IDataModelService
    {
        int AddParameterAttributes(int parameterId, Dictionary<string, string> attributes);
        Dictionary<string, string> GetParameterAttributes(int parameterId);
        Dictionary<string, InputFormField> GetProcedureFormFields(int procedureId);
        ProcedureModel GetProcedureModel(string schema, string name);
    }
}