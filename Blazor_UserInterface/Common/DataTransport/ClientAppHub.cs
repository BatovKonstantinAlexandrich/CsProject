using ApplicationCore.Messaging;
using MedOrg.Shared.ViewServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace HospitalConstructor.Server
{
    public class ClientAppHub : Hub, IMessageHandler
    {
        private readonly ILogger<ClientAppHub> _logger;
        private readonly IMessageHandlerService _service;
        private readonly IMessageHandler _handler;



        public ClientAppHub(
                ILogger<ClientAppHub> logger, 
                ClientAppMessageHandler clientAppMessageHandler,
                JsonMessageHandlerService jsonMessageHandlerService) 
        {
            _logger = logger;
            _service = jsonMessageHandlerService;
            _logger.LogInformation("Hub Created");
        }


        public async Task<string> OnMessage(string message)
        {
            try
            {
                _logger.LogInformation("OnMessage()");                
                RequestMessage request = message.Deseriallize<RequestMessage>();
                object result = Execute(request);
                return new ResponseMessage() { request= request,status="success",data= result }.ToJson();
            }
            catch (Exception ex)
            {
                return new ResponseMessage() { status = "failed",message=ex.Message }.ToJson();
            }

            
        }

        private object Execute(RequestMessage request)
        {
            if (request.ActionName == "login")
            {
                return new
                {
                    username = "eckumoc"
                };
            }
            else
            {
                throw new Exception();
            }
            
        }
    }
}