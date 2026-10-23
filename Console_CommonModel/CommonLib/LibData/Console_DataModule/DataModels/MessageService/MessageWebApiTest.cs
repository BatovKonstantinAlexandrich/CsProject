using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService
{
    public class MessageWebApiTest : TestingService<MessageWebApi>
    {
        public override void OnTest()
        {
            var api = new MessageWebApi();
            api.DropAndCreate();

            api.InitMessageAttributes();
            var protocolFasade = api.GetMessageProtocol();
            this.Info(protocolFasade.GetAll().ToJsonOnScreen());
            int protocolId = 0;
            this.Info(protocolId = protocolFasade.Create(new MessageProtocol()
            {
                Name = "Logs"
            }));

            var properetyFasade = api.GetMessageProperty();
            //Clear();
            int id = properetyFasade.Create(new MessageProperty()
            {
                MessageProtocolId = protocolId,
            });
            this.Info(properetyFasade.GetAll().ToJsonOnScreen());

            id = properetyFasade.Create(new MessageProperty()
            {
                MessageProtocolId = protocolId,
            });
            this.Info(properetyFasade.GetAll().ToJsonOnScreen());
            properetyFasade.Remove(id);
            this.Info(properetyFasade.GetAll().ToJsonOnScreen());
            properetyFasade.Update(new MessageProperty()
            {
                Id = properetyFasade.GetAll().First().Id,
                Label = "ТЕст"
            });
            this.Info(properetyFasade.GetAll().ToJsonOnScreen());
        }

        public override bool OnServiceTest(MessageWebApi service)
        {
            throw new NotImplementedException();
        }
    }
}
