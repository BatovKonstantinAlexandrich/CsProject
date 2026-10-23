using System.ComponentModel.DataAnnotations;



/// <summary>
/// Бизнес-функции пользователя
/// </summary>
public interface IUserBusinessService
{

    [Label("Список операций над данными доступные пользователю")]
    List<BusinessFunction> GetBusinessFunctionsForEntity<TEntity>(  )
        where TEntity : class;

    [Label("Список операций над данными доступные пользователю")]
    List<BusinessFunction> DoBusinessFunctionsForEntity<TEntity>(BusinessFunction function)
        where TEntity : class;





    [Label("Контекстное меню для выбора операции")]
    List<ContextView> CreateBusinessContextView<TDbContext, TEntity>(BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class;


    [Label("Выполнение бизнес операции")]
    Action TodoBusinessFunction<TDbContext, TEntity>(TDbContext db, BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class;



    List<BusinessFunction> GetBusinessFunctions(  );
    List<MessageProtocol> GetMessageProtocols( );
    List<BusinessResource> GetBusinessResources( );
}
