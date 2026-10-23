using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

using System.Text;
using System.Threading.Tasks;


namespace Console_CommonModel.CommonLib.LibData.Console_DataModule.DataModels.MessageModel
{

    [Label("База потоков сообщений")]
    public class MessageDbContext : DbContextBase
    {
        public MessageDbContext()
        {

        }

        public MessageDbContext(DbContextOptions options) : base(options)
        {

        }

        public override string GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = @"DESKTOP-IHJM9RD";
            var databaseName = typeof(DeliveryDbContext).GetTypeName();

            //this.Info(databaseName);
            builder.InitialCatalog = databaseName;
            builder.TrustServerCertificate = true;
            builder.IntegratedSecurity = true;
            return builder.ToString();
        }


        public virtual DbSet<MessageAttribute> MessageAttributes { get; set; }
        public virtual DbSet<MessageProperty> MessageProperties { get; set; }
        public virtual DbSet<MessageProtocol> MessageProtocols { get; set; }
        public virtual DbSet<ValidationModel> ValidationModels { get; set; }



        public void Save()
        {
            using (var db = typeof(DeliveryDbContext).New<DeliveryDbContext>())
            {
                db.GetEntitiesTypes().Select(e => ToMessageProtocol(e));
            }
        }

        public MessageProtocol ToMessageProtocol(Type type)
        {
            var protocol = new MessageProtocol();
            protocol.Name = type.GetTypeName();
            protocol.Icon = type.GetIcon();
            protocol.Help = type.GetHelp();
            protocol.Label = type.GetLabel();
            if (!protocol.Icon.IsEng())
                throw new Exception($"{type.GetTypeName()} содержит не корректные данные атрибутов");
            protocol.Description = type.GetDescription();

            foreach (var property in type.GetProperties())
            {
                try
                {
                    var attrs = type.GetPropertyAttributes(property.Name);
                    this.Info(attrs.ToJsonOnScreen());

                    string typeName = attrs.Keys.FirstOrDefault(kv => kv.ToType().IsExtends(typeof(BaseInputAttribute)));

                    var prop = new MessageProperty();
                    string TypeName = property.PropertyType.Name;
                    prop.Attribute = MessageAttributes.FirstOrDefault(a => a.Name == typeName);
                    prop.AttributeId = prop.Attribute?.Id;
                    prop.Name = property.Name;
                    prop.Label = type.GetPropertyLabel(property.Name);
                    prop.Help = type.GetPropertyHelp(property.Name);
                    prop.Unique = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(UniqValueAttribute));
                    prop.Required = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(NotNullNotEmptyAttribute));
                    prop.Index = type.GetPropertyAttributes(property.Name).Keys.Contains(nameof(Index));
                    prop.MessageProtocolId = protocol.Id;
                    protocol.Properties.Add(prop);
                    if (prop.AttributeId is null)
                    {
                        //TODO: предложить настроить
                        if (!Typing.IsPrimitive(property.PropertyType))
                        {

                            //prop.Attribute = new MessageDbContext<DeliveryDbContext>().GetMessageAttribute<NewInputObjectAttribute>();
                            //prop.AttributeId = prop.Attribute?.Id;
                        }
                        else
                        {
                            //prop.Attribute = context.GetMessageAttribute(property.PropertyType);
                            //prop.AttributeId = prop.Attribute?.Id;
                        }
                    }

                }
                catch (Exception ex)
                {
                    this.Error($"Исключение при регистрации свойства {property.Name}: {ex.Message}");
                }
            }

            return protocol;
        }

    }
}
