using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMetadataServices;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataADO.ADODbMigBuilderService;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataApi;
using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.ProductsFilters;
 

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataODBC.API
{
    public interface IWebApi : IDbMigBuilder
    {
        public bool can(string url, Dictionary<string, string> queryParams, Dictionary<string, string> headers);
        public bool match(string url, Dictionary<string, string> queryParams);
        public Dictionary<string, string> GetNavigation(string url);
        public Dictionary<string, IFilter> GetQueryParams<TEntity>(string url);
        public IEntityFasade<TConvertableType> GetFasade<TConvertableType>(Type ptype) where TConvertableType : BaseEntity;
        public Task<Tuple<int, object>> Request(string url, Dictionary<string, string> args = null, Dictionary<string, string> headers = null, string body = null);
        HashSet<IProcedureExecuter> Procedures { get; }
        HashSet<IEntityFasade> Services { get; }

        ITableManager GetFasade(string schema, string table);
        IEntityFasade<T> GetFasade<T>() where T : BaseEntity;
        IDbMetadata GetModel();
        void Init();
        void InitServices();
    }
}
