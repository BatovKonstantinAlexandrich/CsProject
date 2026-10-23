namespace Console_CommonModel.CommonLib.LibModels.Console_InputModule.ViewModels.CoreMessaging
{
    public class ResponseMessage
    {

        /// <summary>
        /// true, если ответ был получен
        /// </summary>
        public bool confirmed { get; set; }
        public HttpMessage request { get; set; }
        public object data { get; set; }
        public long time { get; set; }
        public string message { get; set; }
        public string status { get; set; }
        public string stack { get; set; }



        /// <summary>
        /// Перенос данных из другого сообщения
        /// </summary>
        /// <param name="response"></param>
        public void Resolve(ResponseMessage response)
        {
            message = response.message;
            status = response.status;
            confirmed = response.confirmed;
            request = response.request;
            data = response.data;
            time = response.time;
        }
    }
}
