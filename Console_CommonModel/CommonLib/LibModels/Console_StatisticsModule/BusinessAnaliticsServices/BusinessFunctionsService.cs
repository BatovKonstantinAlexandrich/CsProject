using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibAttributes.AttributesInput.InputCustom;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

/// <summary>
/// 
/// </summary>
public class BusinessFunctionsService: IBusinessFunctionsService
{
    private readonly BusinessDbContext businessDbContext;

    public BusinessFunctionsService(BusinessDbContext businessDbContext)
    {
        this.businessDbContext = businessDbContext;
    }

    public BusinessFunction CreateBusinessFunctionForBusinessResource<TInput, TOutput>(string businessResourceName, string businessFunctionName)
        where TInput : BaseEntity
        where TOutput : BaseEntity
    {
        
        var inproto = GetOrCreateMessageProtocol<TInput>();
        var outproto = GetOrCreateMessageProtocol<TOutput>();
        var businessFunction = new BusinessFunction()
        {
            InputId = inproto.Id,
            OutputId = outproto.Id,
        };
        businessDbContext.Add(businessFunction);
        businessDbContext.SaveChanges();
        return businessFunction;
    }

    private MessageProtocol GetOrCreateMessageProtocol<TInput>()
    {
        var inputTypeName = typeof(TInput).GetTypeName();
        var protocol = this.businessDbContext.MessageProtocols.Include(p => p.Properties).FirstOrDefault(p => p.Name == inputTypeName);
        if(protocol is not null)
        {
            return protocol;
        }
        else
        {
            protocol = ToMessageProtocol(typeof(TInput));            
            AddOrUpdate(protocol);
            return protocol;
        }        
    }

    public void AddOrUpdate(MessageProtocol protocol)
    {
        var properties = protocol.Properties?.ToList() ?? new List<MessageProperty>();
        protocol.Properties = new List<MessageProperty>();
        var context = businessDbContext;
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

    public MessageProtocol ToMessageProtocol(Type type)
    {
        var protocol = new global::MessageProtocol();
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
                prop.Attribute = businessDbContext.MessageAttributes.FirstOrDefault(a => a.Name == typeName);
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
                        prop.Attribute = businessDbContext.GetMessageAttribute<InputObjectAttribute>();
                        prop.AttributeId = prop.Attribute?.Id;
                    }
                    else
                    {
                        prop.Attribute = businessDbContext.GetMessageAttribute(property.PropertyType);
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
}
