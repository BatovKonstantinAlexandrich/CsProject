using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Microsoft.EntityFrameworkCore;



namespace Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.AppBusiness
{

    /// <summary>
    /// Выполняет бизнес функции пользователя
    /// </summary>
    public class UserBusinessFunctionsService<TDeliveryDbContext> : IUserBusinessFunctionsService<DeliveryDbContext>
        where TDeliveryDbContext : DeliveryDbContext
    {
        private readonly TDeliveryDbContext businessDbContext;

        public UserBusinessFunctionsService()
        {
            businessDbContext = typeof(TDeliveryDbContext).New<TDeliveryDbContext>();
        }

        public List<(BusinessFunction, ProcedureMetadata)> GetBusinessFunctions(string resource)
        {
            List<(BusinessFunction, ProcedureMetadata)> results = new();
            var procedures = businessDbContext.GetProceduresMetadata(resource).Values.ToList();
            var functions = businessDbContext.BusinessFunctions.Include(f => f.Input)
                .Where(f => f.BusinessResourceId == businessDbContext.BusinessResources.First(r => r.Name.ToLower() == resource.ToLower()).Id).ToList();
            foreach (var func in functions)
            {
                func.Output = func.OutputId is not null ? businessDbContext.MessageProtocols.Find(func.OutputId) : null;
                var procedure = procedures.FirstOrDefault(p => p.ProcedureName == func.Name);
                results.Add((func, procedure));
            }
            return results;
        }

        public BusinessFunction CreateUserBusinessFunction<TInputMessageProtocol>(Dictionary<string, BusinessFunction> logic)
        {
            var input = CreateMessageProtocol<TInputMessageProtocol>();
            var businessFunction = new BusinessFunction()
            {
                InputId = input.Id,
                Input = input
            };
            businessFunction.Name = "";
            foreach (var kv in logic)
            {
                businessFunction.Name += kv.Key;
                var cf = kv.Value;
                cf.Name = kv.Key;
                kv.Value.Parent = businessFunction;
                kv.Value.ParentId = businessFunction.Id;
            }
            //businessDbContext.Add(businessFunction);
            //businessDbContext.SaveChanges();
            return businessFunction;



        }

        public MessageProtocol CreateMessageProtocol<TInputMessageProtocol>()
        {
            var context = businessDbContext;
            Type type = typeof(TInputMessageProtocol);
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
    }

}
