namespace Console_CommonModel.CommonLib.LibModels.Console_StatisticsModule.BusinessActionServices
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBusinessUserService
    {
        /// <summary>
        /// Получение списка доступных операций (магинное наименование-текстовое обозначение)
        /// </summary>        
        public Dictionary<string, string> GetActions();

  
    }
}
