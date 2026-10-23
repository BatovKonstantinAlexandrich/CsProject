using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.DatabaseMetadata;
using Console_CommonModel.CommonLib.LibData.Console_DataModule.DataCommon.Metadata;

/// <summary>
/// Модель процесса разработки информационной системы 
/// </summary>
public class BusinessDatabaseGenerator
{
    BusinessDbContext _context;
    public BusinessDatabaseGenerator()
    {
        _context = new();
    }
    public BusinessDatabaseGenerator(BusinessDbContext context)
    {
        _context = context;
    }



    public TableInfo CreateDataModel(BusinessResource resource)
    {
        if (resource == null)
        {
            throw new Exception("CreateDataModel(BusinessResource resource)");
        }

        var validationResult = resource.Validate();
        if (validationResult.Count > 0)
        {
            var e = new Exception(resource + " Ссылка на BusinessResporce не пригодна " + validationResult.ToJson()) { };

            //e.Data
            throw e;
        }

        var metadata = new TableInfo()
        {
            name = RusEngTranslite.TransliteToLatine(resource.Name).ToUpper(),
            caption = resource.Name,
            multicount_name = resource.Name.ToMultiCount(),
            description = resource.Description,
            schema = "BusinessResource",
            singlecount_name = resource.Name.ToSingleCount(),
            pk = "ID"
        };
        metadata.columns["ID"] = new ColumnMetaData()
        {
            description = "Первичный ключ",
            nullable = false,
            type = "int",
            incremental = true
        };
        metadata.columns["UserID"] = new ColumnMetaData()
        {
            description = "Внешний ключ пользователя",
            nullable = false,
            type = "int",
            incremental = true
        };

        return metadata;
    }


    public TableInfo CreateTableModel(MessageProtocol protocol)
    {
        protocol.EnsureIsValide();
        var validationResult = protocol.Validate();
        if (validationResult.Count > 0)
        {
            throw new Exception("Ссылка на экземляр BusinessResporce не пригодна " + validationResult.ToJson()) { };
        }
        var metadata = new TableInfo();
        metadata.caption = protocol.Name;
        metadata.name = protocol.Name;  //"[" + RusEngTranslite.TransliteToLatine(protocol.Name).ToUpper() + "]";
        metadata.multicount_name = protocol.Name.ToMultiCount();
        metadata.description = protocol.Description;
        
        metadata.schema = protocol.FromBusinessFunctionId + "_" + protocol.ToBusinessFunctionId;
        metadata.singlecount_name = protocol.Name;
        metadata.pk = "ID";

        metadata.columns["[ID]"] = new ColumnMetaData()
        {
            description = "Первичный ключ",
            nullable = false,
            type = "int",
            incremental = true
        };
        if (protocol.Properties == null)
        {

            protocol.Properties = (from p in _context.MessageProperties where p.MessageProtocolId == protocol.Id select p).ToList();

        }
        foreach (var p in protocol.Properties)
        {
            
            if (p.AttributeId is not null && p.Attribute is null)
            {
                using (var pdb = new BusinessDbContext())
                {
                    p.Attribute = pdb.MessageAttributes.FirstOrDefault(ma => ma.Id == p.AttributeId);
                }
            }

            var columnName = "[" + RusEngTranslite.TransliteToLatine(p.Name).ToUpper() + "]";
            metadata.columns[columnName] = new ColumnMetaData()
            {
                type = p.Attribute.SqlType,
                nullable = !p.Required && p.Name.ToLower() != "id",
                caption = p.Name,
                name = "[" + RusEngTranslite.TransliteToLatine(p.Name).ToUpper() + "]",
                description = "p.GetDescription()",
                unique = p.Unique,
                primary = p.Name.ToLower() == "id" ? true : false,
                incremental = p.Name.ToLower() == "id"
                //!((p.Join("Attribute")).IsSystem) //системные а
            };
        }
        
        return metadata;
    }

    public TableMetadata CreateDataModel(MessageProtocol protocol)
    {
        var validationResult = protocol.Validate();
        if (validationResult.Count > 0)
        {
            throw new Exception("Ссылка на экземляр BusinessResporce не пригодна " + validationResult.ToJson()) { };
        }
        var metadata = new TableMetadata();
        metadata.TableSchema = "_";
        //metadata.TableCaption = protocol.Name;
        metadata.TableName = protocol.Name;
        //metadata.multicount_name = protocol.Name.ToMultiCount();
        //metadata.description = protocol.Description;
        metadata.TableCatalog = protocol.FromBusinessFunctionId + "_" + protocol.ToBusinessFunctionId;
        // metadata.singlecount_name = protocol.Name;
        //metadata.pk = "ID";
        metadata.ColumnsMetadata = new Dictionary<string, ColumnMetadata>();
        metadata.ColumnsMetadata["[ID]"] = new ColumnMetadata()
        {
            //description = "Первичный ключ",
            IsNullable = false,
            DataType = "int",
            IsAutoIncrement = true
        };
        if (protocol.Properties == null)
        {

            protocol.Properties = (from p in _context.MessageProperties where p.MessageProtocolId == protocol.Id select p).ToList();

        }
        foreach (var p in protocol.Properties)
        {
            if (p.Attribute is null && p.AttributeId is not null)
                using (var pdb = new BusinessDbContext())
                {
                    p.Attribute = pdb.MessageAttributes.FirstOrDefault(m => m.Id == p.AttributeId);
                }

            metadata.ColumnsMetadata[p.Name] = new ColumnMetadata()
            {
                DataType = p.Attribute.SqlType,
                IsNullable = !p.Required && p.Name.ToLower() != "id",
                //ColumnCaption = p.Name,
                ColumnName = p.Name, //"[" + RusEngTranslite.TransliteToLatine(p.Name).ToUpper() + "]",
                //ColumnDescription = "p.GetDescription()",
                //IsUnique = p.Unique,
                //IsPrimary = p.Name.ToLower() == "id" ? true: false,
                IsAutoIncrement = p.Name.ToLower() == "id"
                //!((p.Join("Attribute")).IsSystem) //системные а
            };
        }
        return metadata;
    }

    public TableInfo CreateDataModel(BusinessDataset dataset)
    {
        throw new NotImplementedException();
    }

    public TableInfo CreateDataModel(BusinessLogic logic)
    {
        throw new NotImplementedException();
    }



    public TableInfo CreateStatisticsModel(BusinessResource resource)
    {
        throw new NotImplementedException();
    }

    public TableInfo CreateStatisticsModel(MessageProtocol protocol)
    {
        throw new NotImplementedException();
    }

    public TableInfo CreateStatisticsModel(BusinessDataset dataset)
    {
        throw new NotImplementedException();
    }

    public TableInfo CreateStatisticsModel(BusinessLogic logic)
    {
        throw new NotImplementedException();
    }

    internal string DropProcedure(string name)
    {
        return $"DROP PROCEDURE {name}";
    }
}
