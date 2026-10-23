
namespace Console_CommonModel.CommonLib.LibAuth.AppAuth.Authorization
{
    public interface IBusinessProcessValidator
    {
        string GetBusinessIndicatorName(BusinessFunction function);
        string GetStoredProcedureName(BusinessFunction function);
        string GetTableName(MessageProtocol protocol);
        bool HasBusinessIndicator(BusinessFunction function);
        void HasStoredProcedure(BusinessFunction function);
        bool HasTableFor(MessageProtocol protocol);
        Dictionary<string, List<string>> Validate(BusinessProcess process);
    }
}