

using Microsoft.AspNetCore.SignalR;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreConstructorAngular
{
    public class ResourceHub: Hub
    {
        private static string Dir =
            @"A:\Wrk\NetCoreConstructorAngular\NetCoreConstructorAngular\wwwroot";

        public string Request( string RequestText )
        {
            Writing.ToConsole("Requesting: \n" + RequestText);
            DataRequestMessage RequestMessage = Deserrialize(RequestText);
            var ResponseMessage = new DataResponseMessage() {
                ActionStatus = "Success",
                SerialKey = RequestMessage.SerialKey,                
                MessageObject = RequestMessage.MessageObject                
            };
            string ResponseText = JObject.FromObject(ResponseMessage).ToString();
            Clients.Caller.SendAsync("Response", ResponseText);
            return RequestText;
        }


        private static DataRequestMessage Deserrialize(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<DataRequestMessage>(json);
            }
            catch(Exception ex)
            {
                Writing.ToConsole("Не удалось сериализовать сообщение с параметрами запроса: "+ex.Message);
                throw;
            }
        }


        public void Save(byte[] data, string name)
        {
            File.WriteAllBytes($"{Dir}\name" , data);
        }
    }
}
