using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

using System.ComponentModel.DataAnnotations;

using static Azure.Core.HttpHeader;

/// <summary>
/// Управление спец. функциями системы относительно пользователя
/// </summary>
public class UserBusinessService<TDeliveryDbContext> : IUserBusinessService 
    where TDeliveryDbContext : DeliveryDbContext
{
    
    private SigninUser<TDeliveryDbContext> _signin;

    public UserBusinessService( SigninUser<TDeliveryDbContext> signin)
    {
         
        _signin = signin;
    }

    /// <summary>
    /// Получение ролей
    /// </summary>
    public List<BusinessResource> GetBusinessResources( )
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            int userId = this._signin.Verify().Id;
            var userRoleIds = _context.UsersResources.Where(sr => sr.UserId == userId).Select(sr => sr.RoleId).ToList();
            return _context.BusinessResources.Where(r => userRoleIds.Contains(r.Id)).ToList();
        }
    }

    public List<MessageProtocol> GetMessageProtocols( )
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {

            try
            {
                int userId = this._signin.Verify().Id;
                var ids = _context.BusinessFunctions.Where(rf => this.GetBusinessResources().Select(r => r.Id).Contains((int)rf.BusinessResourceId)).Select(rf => rf.InputId).ToList();
                return _context.MessageProtocols.Include(p => p.Properties).Where(p => ids.Contains(p.Id)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Попытка получения списка потоколов потоков сообщений завершилась исключением: " + ex.Message);
            }
        }
    }
   

    public List<BusinessFunction> GetBusinessFunctions()
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {
            int userId = this._signin.Verify().Id;
            return _context.BusinessFunctions.Where(rf => this.GetBusinessResources().Select(r => r.Id).Contains((int)rf.BusinessResourceId)).ToList();
        }

    }

    public BusinessLogic GetBusinessLogic<T>()
    {
        var messageProtocol = this.GetMessageProcolInput<T>();
        var businessFucnction = this.GetBusinessFunctions().FirstOrDefault(bf => bf.InputId == messageProtocol.Id);
        //TODO: get business logic from business function
        throw new NotImplementedException();
    }

    private MessageProtocol GetMessageProcolInput<T>()
    {
        var messageProtocol = this.GetMessageProtocols().FirstOrDefault(mp => mp.GetInputTableName() == typeof(T).GetTypeName() || mp.GetInputTableName() == typeof(T).GetTypeName().ToSingleCount() || mp.GetInputTableName() == typeof(T).GetTypeName().ToMultiCount() );
        return messageProtocol;
    }

    public List<BusinessFunction> GetBusinessFunctionsForEntity<TEntity>() where TEntity : class
    {
        var functions = this.GetBusinessFunctions();
        return functions.Where(f => f.Input.Name == typeof(TEntity).GetTypeName()).ToList();
    }

    public List<BusinessFunction> DoBusinessFunctionsForEntity<TEntity>(BusinessFunction function) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public void SetDbContext(TDeliveryDbContext bdc)
    {
     
    }

    public void CheckBusinessResourcesHasRegistered(IEnumerable<string> names)
    {
        using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
        {

            _context.CheckRegistration<BusinessResource>(names);
        }
    }

    public List<ContextView> CreateBusinessContextView<TDbContext, TEntity>(BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Action TodoBusinessFunction<TDbContext, TEntity>(TDbContext db, BusinessFunction function)
        where TDbContext : DeliveryDbContext
        where TEntity : class
    {
        throw new NotImplementedException();
    }
}