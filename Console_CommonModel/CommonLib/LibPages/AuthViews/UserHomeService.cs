using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibPages.AuthViews
{
    public interface IUserHomeService
    {
        public NavView GetBusinessNavView();
    }


    public class UserHomeService<TDeliveryDbContext> : UserMessagesService<TDeliveryDbContext>, IUserHomeService
        where TDeliveryDbContext : DeliveryDbContext
    {
        private readonly IMessageModelService messageModel;

        public UserHomeService(IMessageModelService messageModel, TDeliveryDbContext context, SigninUser<TDeliveryDbContext> signin) : base(signin)
        {
            this.messageModel = messageModel;

        }

        public NavView GetBusinessNavView()
        {
            using (var _context = typeof(TDeliveryDbContext).New<TDeliveryDbContext>())
            {
                var nav = new NavView();
                foreach (var messageProtocol in GetMessageProtocols())
                {
                    var tableName = messageModel.GetTableNameFor(messageProtocol);
                    this.Info(tableName);
                    this.Info(_context.Model.GetEntityTypes().Select(et => et.GetTypeName()));
                    var entityType = _context.Model.GetEntityTypes().FirstOrDefault(et => et.GetTypeName() == tableName);
                    var repository = _context.GetRepositoryForType(entityType.ClrType);
                    nav.AddNavOption(
                        entityType.ClrType.GetLabel(),
                        repository
                    );
                }
                return nav;
            }
        }
    }
}
