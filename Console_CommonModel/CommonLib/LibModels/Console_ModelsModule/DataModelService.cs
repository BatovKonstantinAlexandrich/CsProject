using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibData.Console_DataModule.Data.ADOWebApiService;
using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;

namespace Console_CommonModel.CommonLib.LibModels.Console_ModelsModule
{
    public class DataModelService : IDataModelService
    {
        private readonly ModelsDbContext modelsDbContext;

        public DataModelService() { }
        public DataModelService(ModelsDbContext modelsDbContext)
        {
            this.modelsDbContext = modelsDbContext;
        }
        public ProcedureModel GetProcedureModel(string schema, string name)
        {
            var procedure = modelsDbContext.Procedures.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
            if (procedure is not null)
            {
                procedure.Parameters = modelsDbContext.Parameters.Include(p => p.Attributes).Where(par => par.ProcedureModelId == procedure.Id).ToList();
            }
            procedure.EnsureIsValide();
            return procedure;
        }


        public int AddParameterAttributes(int parameterId, Dictionary<string, string> attributes)
        {
            var arr = attributes.Select(kv => new ParameterAttributeModel()
            {
                ParameterModelId = parameterId,
                AttributeType = kv.Key,
                AttributeValue = kv.Value
            }).ToList();

            arr.ToList().ForEach(p => p.EnsureIsValide());
            modelsDbContext.ParameterAttributes.AddRange(arr);
            return modelsDbContext.SaveChanges();
        }

        public Dictionary<string, string> GetParameterAttributes(int parameterId)
        {
            Dictionary<string, string> attributes = new();
            var attrs = modelsDbContext.ParameterAttributes.Where(a => a.ParameterModelId == parameterId).Select(a => new KeyValuePair<string, string>(a.AttributeType, a.AttributeValue));
            return new Dictionary<string, string>(attrs);
        }

        public Dictionary<string, InputFormField> GetProcedureFormFields(int procedureId)
        {
            var dbConnections = new DbConnectionsService();
            Dictionary<string, InputFormField> fields = new();
            var procedure = modelsDbContext.Procedures.Include(p => p.DatabaseModel).Include(p => p.Parameters).First(p => p.Id == procedureId);
            var database = modelsDbContext.Databases.Include(d => d.DatabaseConnection).FirstOrDefault(d => d.Id == procedure.DatabaseModelId);
            if (database != null)
            {
                throw new InvalidOperationException("Не удалось получить сведения по DatabaseModel");
            }
            var connectionString = database.DatabaseConnection.ConnectionString;
            var api = new SqlServerWebApi();
            api.Configure(connectionString);
            this.Info($"Schema={procedure.Schema}, procedure={procedure.Name}");
            ProcedureMetadata metadata = api.GetProcedureMetadata(procedure.Schema, procedure.Name);

            var key = procedure.Name.Substring(procedure.Name.LastIndexOf("[") + 1);
            key = key.Substring(0, key.Length - 1);
            foreach (var par in procedure.Parameters)
            {
                var parameterId = par.Id;
                var attrs = modelsDbContext.ParameterAttributes.Where(a => a.ParameterModelId == parameterId).Select(a => new KeyValuePair<string, string>(a.AttributeType, a.AttributeValue));
                Dictionary<string, string> parameterAttributes = new Dictionary<string, string>(attrs);
                if (metadata.ParametersMetadata.ContainsKey(par.Name))
                {
                    throw new Exception($"Параметр не найден {par.Name}");
                }
                var parmeta = metadata.ParametersMetadata[par.Name];

                fields[par.Name] = new InputFormField()
                {
                    Name = par.Name,
                    Attributes = parameterAttributes,
                    Description = par.Description,
                    Icon = par.Icon,
                    IsCollection = false,
                    IsPrimitive = true,
                    Label = par.Label,
                    Help = par.Help,
                    Type = GetInputType(parmeta.DataType, parameterAttributes),
                    Order = GetOrder(parameterAttributes)
                };
            }
            return fields;



        }

        private int GetOrder(Dictionary<string, string> attrs)
        {
            var order = attrs.ContainsKey(nameof(InputOrderAttribute)) ?
                typeof(InputOrderAttribute).New<InputOrderAttribute>(new object[] { attrs[nameof(InputOrderAttribute)] }) : null;
            if (order is not null)
                return order._Order;
            return 0;
        }

        private string GetInputType(string dataType, Dictionary<string, string> attrs)
        {
            var key = attrs.Keys.FirstOrDefault(key => key.ToType().IsExtends(typeof(BaseInputAttribute)));
            if (key != null)
            {
                var attrValue = attrs[key];
                if (string.IsNullOrWhiteSpace(attrValue))
                    key.ToType().New<BaseInputAttribute>(Array.Empty<object>());
                else
                    key.New<BaseInputAttribute>(new object[] { attrValue });
                return key.Replace("Input", "").Replace("Attribute", "").ToLower();
            }
            else
            {
                switch (dataType)
                {
                    case "int": return nameof(InputNumberAttribute).Replace("Input", "").Replace("Attribute", "").ToLower();
                    case "varchar": return nameof(InputTextAttribute).Replace("Input", "").Replace("Attribute", "").ToLower();
                    default: throw new NotImplementedException($"Тип данных не поддерживается {dataType}");
                }
            }
        }


    }
}
