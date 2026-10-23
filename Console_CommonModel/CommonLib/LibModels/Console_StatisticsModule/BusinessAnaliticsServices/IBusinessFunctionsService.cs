using Console_CommonModel.CommonLib.LibData.AbstractionModel;

public interface IBusinessFunctionsService
{
    public BusinessFunction CreateBusinessFunctionForBusinessResource<TInput, TOutput>(string businessResourceName, string businessFunctionName)
        where TInput : BaseEntity
        where TOutput : BaseEntity;
}
