[Label("Интерфейс проектироващика бизнес функций пользователя")]
public interface IUserBusinessFunctionsService<TDeliveryDbContext>
    where TDeliveryDbContext : DeliveryDbContext
{


    [Label("Регистрация бизнес-функции над данными")]
    public BusinessFunction CreateUserBusinessFunction< TInputMessageProtocol>(Dictionary<string, BusinessFunction> logic);
    public MessageProtocol CreateMessageProtocol<TInputMessageProtocol>();


}