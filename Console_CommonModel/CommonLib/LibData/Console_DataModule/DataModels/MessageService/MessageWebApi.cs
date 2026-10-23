using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageService
{
    [Label("API протоколы связи")]
    public class MessageWebApi : SqlServerWebApi
    {
        public MessageWebApi()
        {
            AddEntityType(typeof(MessageAttribute));
            AddEntityType(typeof(MessageProperty));
            AddEntityType(typeof(MessageProtocol));
            AddEntityType(typeof(ValidationModel));
            GetConnection();
            DropAndCreate();
            InitServices();
        }


        public int CreateMessageProtocol<TModel>()
        {
            var protocol = new MessageProtocol();
            protocol.Name = typeof(TModel).GetTypeName();
            int result = GetFasade<MessageProtocol>().Create(protocol);
            foreach (var name in typeof(TModel).GetInputPropertiesSorted())
            {
                var messageProperty = new MessageProperty()
                {
                    MessageProtocolId = result,
                    Label = typeof(TModel).GetPropertyLabel(name),
                    Help = typeof(TModel).GetPropertyHelp(name)
                };
                GetFasade<MessageProperty>().Create(messageProperty);
            }
            return result;
        }

        public void InitMessageAttributes()
        {
            IEnumerable<Type> attributes = GetBaseInputAttributes();
            if (GetMessageAttribute().Count() != attributes.Count())
            {
                foreach (var attribute in attributes)
                {
                    /*foreach (var m in GetMessageAttribute().GetAll())
                    {
                        GetMessageAttribute().Remove(m.Id);
                    }*/
                    var at = ConvertToMessageAttribute(attribute);
                    Services.FirstOrDefault(s => s.GetEntityName() == nameof(MessageAttribute)).Create(at);
                    //GetMessageAttribute().Create(at);
                }
            }
        }

        private IEnumerable<Type> GetBaseInputAttributes()
        {
            return ServiceFactory.Get().GetTypesExtendeds<BaseInputAttribute>().Where(t => t.IsAbstract == false && t.GetTypeName().IndexOf("ControlAttribute") == -1 && t.GetTypeName().IndexOf("InputAttribute") == -1 && t.GetTypeName().IndexOf("BaseInput") == -1);
        }

        private static MessageAttribute ConvertToMessageAttribute(Type attribute)
        {
            string atrName = (attribute.GetType().Name.StartsWith("Input") ? attribute.GetType().Name.Substring("Input".Length) : attribute.GetType().Name).Replace("Attribute", "");
            //var icons = new MaterialIconsService();

            BaseInputAttribute attr = ReflectionService.CreateWithDefaultConstructor<BaseInputAttribute>(attribute);
            MessageAttribute res = new MessageAttribute()
            {

                SqlType = attr.GetSqlServerDataType(),
                CsType = attr.GetCSTypeName(),
                SqlServerDataType = attr.GetSqlServerDataType(),
                MySqlDataType = attr.GetMySQLDataType(),
                PostgreDataType = attr.GetPostgreDataType(),
                OracleDataType = attr.GetOracleDataType(),
                Description = attribute.GetLabel() + ":\n" + attribute.GetDescription(),
                //Icon = attribute.GetIcon(),
                Name = attribute.GetTypeName(),
                InputType = (attribute.Name.StartsWith("Input") ? attribute.Name.Substring("Input".Length) : attribute.Name).Replace("Attribute", "")
            };
            res.ToJsonOnScreen().WriteToConsole();
            if (res.Name == null)
            {
                int x = 0;
                string label = attribute.GetLabel();
            }
            return res;
        }

        public IEntityFasade<MessageProtocol> GetMessageProtocol() => GetFasade<MessageProtocol>();
        public IEntityFasade<MessageProperty> GetMessageProperty() => GetFasade<MessageProperty>();
        public IEntityFasade<MessageAttribute> GetMessageAttribute() => GetFasade<MessageAttribute>();
        public IEntityFasade<ValidationModel> GetValidationModel() => GetFasade<ValidationModel>();
    }
}
