using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel
{
    public class MessageUnit : IDisposable
    {
        private readonly BusinessDbContext _messageDbContext;

        private readonly IEfEntityFasade<MessageAttribute> MessageAttributeFasade;
        private readonly IEfEntityFasade<MessageProtocol> MessageProtocolFasade;
        private readonly IEfEntityFasade<MessageProperty> MessagePropertyFasade;
        private readonly IEfEntityFasade<ValidationModel> ValidationModelFasade;

        public MessageUnit(BusinessDbContext messageDbContext)
        {
            _messageDbContext = messageDbContext;
            MessageAttributeFasade = new EfEntityFasade<MessageAttribute>(_messageDbContext);
            MessagePropertyFasade = new EfEntityFasade<MessageProperty>(_messageDbContext);
            ValidationModelFasade = new EfEntityFasade<ValidationModel>(_messageDbContext);
            MessageProtocolFasade = new EfEntityFasade<MessageProtocol>(_messageDbContext);
        }

        public void Dispose()
        {
            _messageDbContext.Dispose();
        }
    }
}
