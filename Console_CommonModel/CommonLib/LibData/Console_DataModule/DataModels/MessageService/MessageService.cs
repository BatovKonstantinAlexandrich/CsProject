using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService
{
    public interface IMessageService
    {
        public int UpdateMessageProperty(MessageProperty property);
        public int DeleteMessageProperty(int MessageProtocolId, MessageProperty property);
        public int AddMessageProperty(int MessageProtocolId, MessageProperty property, List<MessageAttribute> attrs);
        public IEnumerable<MessageProperty> GetMessageProperties(int MessageProtocolId);
        public IEnumerable<MessageProtocol> GetMessageProtocols();
        public int AddMessageProtocol(MessageProtocol target);
    }

    public class MessageService : IMessageService
    {
        private readonly MessageWebApi api;

        public MessageService(MessageWebApi api)
        {
            this.api = api;

        }
        public MessageProtocol CreateMessageProtocol<TModel>()
        {
            var protocol = new MessageProtocol();
            protocol.Name = typeof(TModel).GetTypeName();

            foreach (var name in typeof(TModel).GetProperties().Select(p => p.Name))
            {
                var messageProperty = new MessageProperty()
                {
                    MessageProtocol = protocol,
                    Label = typeof(TModel).GetPropertyLabel(name),
                    Help = typeof(TModel).GetPropertyHelp(name)
                };
                protocol.Properties.Add(messageProperty);


            }
            return protocol;
        }
        public int UpdateMessageProperty(MessageProperty property)
        {
            return api.GetMessageProperty().Update(property);
        }

        public int DeleteMessageProperty(int MessageProtocolId, MessageProperty property)
        {
            property.Id = MessageProtocolId;
            return api.GetMessageProperty().Create(property);
        }

        public int AddMessageProperty(int MessageProtocolId, MessageProperty property, List<MessageAttribute> attrs)
        {
            property.Id = MessageProtocolId;
            foreach (var attr in attrs)
            {
                property.Attribute = attr;
            }
            return api.GetMessageProperty().Create(property);
        }

        public IEnumerable<MessageProperty> GetMessageProperties(int MessageProtocolId)
        {
            return api.GetMessageProperty().GetAll().Where(property => property.MessageProtocolId == MessageProtocolId);
        }

        public IEnumerable<MessageProtocol> GetMessageProtocols()
        {
            return api.GetMessageProtocol().GetAll();
        }

        public int AddMessageProtocol(MessageProtocol target)
        {
            return api.GetMessageProtocol().Create(target);
        }

        public int RemoveMessageProtocol(int id)
        {
            return api.GetMessageProtocol().Remove(id);
        }

        public int UpdateMessageProtocol(MessageProtocol target)
        {
            return api.GetMessageProtocol().Update(target);
        }
    }
}

public static class MessageServiceExtension
{
    public static void AddMessageService(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BusinessDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });
        services.AddTransient<MessageWebApi>();
        services.AddTransient<IMessageService, MessageService>();
    }
}