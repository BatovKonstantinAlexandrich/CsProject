namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryApi
{
    /// <summary>
    /// Функция публикации событий 
    /// </summary>
    public interface IEventsService
    {
        /// <summary>
        /// Публикация события
        /// </summary>
        public void Publish(string type, object data);
    }

}
