using Console_CommonModel.CommonLib.LibData.Console_DataModule;

using Microsoft.EntityFrameworkCore;

using static BusinessDbBehaviour;

[Label("Функции создания элементов базы данных")]
public class BusinessDbFactory: BusinessDbContext, IMessageModelService
{

    ////////////////////////   IMessageModelService   ////////////////////////////////
    protected readonly IMessageModelService messageModel;
    public BusinessDbFactory()
    {
        this.messageModel = new MessageModelService(this);
    }
    public BusinessDbFactory(DbContextOptions<DbContextBase> options) : base(options)
    {
        this.messageModel = new MessageModelService(this);
    }
    public BusinessDbFactory(string database)
    {
        this._database = database;
        this.messageModel = new MessageModelService(this);
    }
    


    public void CreateBusinessIndicator(BusinessFunction function)
        => this.messageModel.CreateBusinessIndicator(function);

    public void DropBusinessIndicator(BusinessFunction function)
        => this.messageModel.DropBusinessIndicator(function);

    public void DropStoredProcedure(BusinessFunction function)
        => this.messageModel.DropStoredProcedure(function);
    
    public void CreateStoredProcedure(BusinessFunction function)
        => this.messageModel.CreateStoredProcedure(function);
    public string GetProcedureNameFor(BusinessFunction protocol)
      => messageModel.GetProcedureNameFor(protocol);
    public string GetTableNameFor(MessageProtocol protocol)
        => messageModel.GetTableNameFor(protocol);
    public int AddMessageProtocol(MessageProtocol protocol)
    {
        return messageModel.AddMessageProtocol(protocol);
    }
    public string GetBusinessResourceTableNameFor(BusinessFunction function)
    {
        return messageModel.GetBusinessResourceTableNameFor(function);
    }
    public string GetTableNameFor(BusinessResource resource)
    {
        return messageModel.GetTableNameFor(resource);
    }
   
    public bool HasProcedure(string schema, string name)
    {
        var rs = this.ExecuteQuery<CountResultSet>($"select count(*) as [count] from sys.procedures p left join sys.schemas s on s.schema_id=p.schema_id where p.name = '{name.Replace("]", "").Replace("[", "")}' and s.name = '{schema.Replace("]", "").Replace("[", "")}'");
        return rs.Count() > 0 && rs.First().count > 0;
    }

    public MessageProtocol CreateMessageProtocol<TModel>() where TModel : class
    {
        throw new NotImplementedException();
    }

    public BusinessFunction CreateBusinessFunction<TBusinessResource, TInputMessageProtocol>([NotNullNotEmpty] BusinessLogic logic)
    {
        throw new NotImplementedException();
    }

    public Dictionary<string, Dictionary<int, Dictionary<string, List<string>>>> ValidateModel()
    {
        return messageModel.ValidateModel();
    }
}
