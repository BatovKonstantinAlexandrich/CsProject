using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi
{
    public interface IProcedureExecuter
    {
        public string GetName();
        public string GetDescription();
        public ProcedureMetadata GetMetaData();
        public Dictionary<string, ParameterMetadata> GetInputMetaData();
        public Dictionary<string, ParameterMetadata> GetOutputMetaData();
        //public DataTable ExecuteProcedure(IDictionary<string, string> input);
        public Func<Dictionary<string, object>, object> GetExecFunc<TResult>() where TResult : class;
    }
}
