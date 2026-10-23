using Microsoft.EntityFrameworkCore;

using Org.BouncyCastle.Asn1.Cms;

using pickpoint_delivery_service;

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

using Console_CommonModel.CommonLib.LibViews.DataComponents;
using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;

namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel
{

    [Label("Инициализация потоков данных")]
    public class MessagesDbContextInitializer : DbContextInitiliazer<DeliveryDbContext>
    {
        public MessagesDbContextInitializer()
        {
        }

        public override Dictionary<string, int> Init(DeliveryDbContext context)
        {
            var result = new Dictionary<string, int>();
            try
            {
                result[nameof(MessageAttribute)] = InitMessageAttributes(context);
                result[nameof(MessageProtocol)] = InitMessageProtocols(context);
                //result[nameof(ValidationModel)] = InitValidationModels(context);
                //result[nameof(BusinessFunction)] = InitBusinessFunctions(context);

                return result;
            }
            catch (Exception ex)
            {
                this.Error(ex);
                throw;
            }
        }


        [Label("Инициаллизация бизнес-функций")]
        public int InitBusinessFunctions(DeliveryDbContext context)
        {
            //context.CreateBusinessFunctionForBusinessResource<UserAccount, UserContext>("Users", "Authorization");
            return 1;
        }

        public int InitValidationModels(DeliveryDbContext context)
        {


            return 0;
        }

        public int InitMessageAttributes(DeliveryDbContext context)
        {

            var records = context.MessageAttributes.ToList();
            int ctn = 0;
            List<Type> attributes = GetBaseInputAttributes().ToList();
            List<Type> generated = new();
            this.Info(attributes.Select(t => t.GetTypeName()).ToJsonOnScreen());
            foreach (var attribute in attributes.ToList())
            {

                if (attribute.GetGenericArguments().Count() > 0)
                {
                    attributes.Remove(attribute);
                    //generated.AddRange(attribute.Generate());
                }
            }

            this.Info(attributes.Select(t => t.GetTypeName()).ToJsonOnScreen());

            this.Warn(attributes.Select(a => a.GetLabel()).Where(a => a.IsEng()).ToJsonOnScreen());
            foreach (var attribute in attributes)
            {
                MessageAttribute messageAttribute = ToMessageAttribute(attribute);
                if (!context.MessageAttributes.ToList().Any(ma => ma.Name == messageAttribute.Name))
                {
                    this.Debug(messageAttribute.ToJsonOnScreen());
                    this.Debug(messageAttribute.Validate().ToJsonOnScreen());
                    context.MessageAttributes.Add(messageAttribute);
                    context.SaveChanges();
                    ctn++;
                }
            }

            foreach (var attribute in generated)
            {
                MessageAttribute messageAttribute = ToMessageAttribute(attribute);
                messageAttribute.Name = messageAttribute.Name.Replace("<", "").Replace(">", "").Replace(",", "");
                if (!context.MessageAttributes.ToList().Any(ma => ma.Name == messageAttribute.Name))
                {
                    this.Debug(messageAttribute.ToJsonOnScreen());
                    this.Debug(messageAttribute.Validate().ToJsonOnScreen());
                    context.MessageAttributes.Add(messageAttribute);
                    context.SaveChanges();
                    ctn++;
                }
            }
            context.SaveChanges();
            return ctn;
        }

        [Label("Среда выполнения")]
        public class Environment
        {
            public int Id { get; set; }
            [InputRusWord()]
            public string Name { get; set; }

        }

        [Label("Компания")]
        public class Company
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string CompanyAddress { get; set; }
            public string CompanyInn { get; set; }
        }

        [Label("Сделка")]
        public class Deal
        {
            public int Id { get; set; }
            public int CustomerId { get; set; }
            public int CompanyId { get; set; }
        }



        public int InitMessageProtocols(DeliveryDbContext context)
        {
            int counter = 0;

            if (true)
            {
                this.Info(context.Database.GetConnectionString());

                var env = ToMessageProtocol(context, typeof(Environment));
                this.Info(env.ToJsonOnScreen());
                AddOrUpdate(context, env);
                var input = ToMessageProtocol(context, typeof(Company));
                AddOrUpdate(context, input);
                BusinessResource holder = context.BusinessResources.FirstOrDefault(r => r.Name == "Holder");
                if (holder is null)
                {
                    this.Info(context.Database.GetConnectionString());
                    holder = new BusinessResource()
                    {
                        Name = "Holder",
                        Code = "Holder",
                        Description = "Holder",
                        Label = "Склад"
                    };
                    context.Add(holder);
                    context.SaveChanges();
                }
                var registration = new BusinessFunction()
                {
                    Name = "CompanyRegistration",
                    Label = "Регистрация компании партнёра.",
                    Description = "Регистрация компании партнёра.",
                    InputId = env.Id,
                    OutputId = input.Id,
                    BusinessResourceId = holder.Id
                };
                context.Add(registration);
                context.SaveChanges();

                env.ToBusinessFunctionId = registration.Id;
                input.FromBusinessFunctionId = registration.Id;
                context.SaveChanges();

                var output = ToMessageProtocol(context, typeof(ServiceSertificate));
                AddOrUpdate(context, output);
                context.Add(new MessageProperty()
                {
                    Name = "Created",
                    Label = "Создано",
                    Help = "Дата создания",
                    AttributeId = context.MessageAttributes.First(a => a.Name == nameof(InputDateAttribute)).Id,
                    MessageProtocolId = output.Id
                });
                context.SaveChanges();
                counter++;
            }

            return counter;
        }

        public void AddOrUpdate(DeliveryDbContext context, MessageProtocol protocol)
        {
            var properties = protocol.Properties?.ToList() ?? new List<MessageProperty>();
            protocol.Properties = new List<MessageProperty>();

            context.AddOrUpdate(protocol);

            if (protocol.Id > 0)
            {
                context.RemoveRange(context.MessageProperties.Where(mp => mp.MessageProtocolId == protocol.Id));
                context.SaveChanges();
            }

            foreach (var property in properties)
            {
                property.Id = 0;
                property.MessageProtocolId = protocol.Id;
                property.MessageProtocol = null;
                property.Attribute = null;
            }

            if (properties.Count > 0)
            {
                context.MessageProperties.AddRange(properties);
                context.SaveChanges();
            }
        }

        public MessageProtocol ToMessageProtocol(DeliveryDbContext context, Type type)
        {
            var protocol = new MessageProtocol();
            protocol.Name = type.GetTypeName();
            protocol.Icon = type.GetIcon();
            protocol.Help = type.GetHelp();
            protocol.Label = type.GetLabel();
            if (!protocol.Icon.IsEng())
                throw new Exception($"{type.GetTypeName()} содержит не корректные данные атрибутов");
            protocol.Description = type.GetDescription();

            var inputProperties = type.GetInputPropertiesSorted();
            foreach (var propertyName in inputProperties)
            {
                try
                {
                    var property = type.GetProperty(propertyName);
                    var attrs = type.GetPropertyAttributes(propertyName);
                    this.Info(property + " " + attrs.ToJsonOnScreen());

                    string typeName = attrs.Keys.FirstOrDefault(kv => kv.ToType().IsExtends(typeof(BaseInputAttribute)));

                    var prop = new MessageProperty();
                    string TypeName = property.PropertyType.Name;
                    prop.Attribute = context.MessageAttributes.FirstOrDefault(a => a.Name == typeName);
                    prop.AttributeId = prop.Attribute?.Id;
                    prop.Name = property.Name;
                    prop.Label = type.GetPropertyLabel(property.Name);
                    prop.Help = type.GetPropertyHelp(property.Name);
                    prop.Unique = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(UniqValueAttribute));
                    prop.Required = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(NotNullNotEmptyAttribute));
                    prop.Index = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(Index));
                    //prop.MessageProtocolId = protocol.Id;
                    protocol.Properties.Add(prop);
                    if (prop.AttributeId is null)
                    {
                        if (!Typing.IsPrimitive(property.PropertyType))
                        {

                            prop.Attribute = context.GetMessageAttribute<InputObjectAttribute>();
                            prop.AttributeId = prop.Attribute?.Id;
                        }
                        else
                        {
                            prop.Attribute = context.GetMessageAttribute(property.PropertyType);
                            prop.AttributeId = prop.Attribute?.Id;
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при регистрации свойства {propertyName}: {ex.Message}");
                }
            }

            return protocol;
        }

        public IEnumerable<Type> GetBaseInputAttributes()
            => ServiceFactory.Get().GetTypesExtendeds<BaseInputAttribute>().Where(t => t.IsAbstract == false && t.GetTypeName().IndexOf("ControlAttribute") == -1 && t.GetTypeName().IndexOf("InputAttribute") == -1 && t.GetTypeName().IndexOf("BaseInput") == -1);


        public MessageAttribute ToMessageAttribute(Type attribute)
        {
            string atrName = (attribute.GetType().Name.StartsWith("Input") ? attribute.GetType().Name.Substring("Input".Length) : attribute.GetType().Name).Replace("Attribute", "");

            BaseInputAttribute attr = attribute.New<BaseInputAttribute>();
            MessageAttribute res = new MessageAttribute()
            {
                Label = attribute.GetLabel(),
                SqlType = attr.GetSqlServerDataType(),
                CsType = attr.GetCSTypeName(),
                SqlServerDataType = attr.GetSqlServerDataType(),
                MySqlDataType = attr.GetMySQLDataType(),
                PostgreDataType = attr.GetPostgreDataType(),
                OracleDataType = attr.GetOracleDataType(),
                Description = attribute.GetDescription(),
                Icon = attribute.GetIcon(),
                Name = attribute.GetTypeName().Replace("<", "").Replace(">", "").Replace(",", "").Replace(".", ""),
                Code = attribute.GetTypeName(),
                InputType = (attribute.Name.StartsWith("Input") ? attribute.Name.Substring("Input".Length) : attribute.Name).Replace("Attribute", "")
            };
            return res;
        }

    }

}
