public interface IMessageModelService 
{
    public string GetProcedureNameFor(BusinessFunction protocol);
    public string GetTableNameFor(MessageProtocol protocol);

    public MessageProtocol CreateMessageProtocol<TModel>( )
        where TModel : class;
    public BusinessFunction CreateBusinessFunction<TBusinessResource, TInputMessageProtocol>
    (
        [NotNullNotEmpty]
        BusinessLogic logic
    );
    public Dictionary<string, Dictionary<int, Dictionary<string, List<string>>>> ValidateModel();


    public int AddMessageProtocol(MessageProtocol protocol);
    public void DropStoredProcedure(BusinessFunction function);
    public void CreateStoredProcedure(BusinessFunction function);
    public void CreateBusinessIndicator(BusinessFunction function);
    public void DropBusinessIndicator(BusinessFunction function);
    public string GetTableNameFor(BusinessResource resource);
    public string GetBusinessResourceTableNameFor(BusinessFunction function);
}
