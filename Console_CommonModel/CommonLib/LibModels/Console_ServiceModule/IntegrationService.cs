using Microsoft.EntityFrameworkCore;

using System.Reflection;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{
    public abstract class IntegrationService<TDbContext, TIntegratedObject> : Attribute,
        
        IIntegrationService<TDbContext, TIntegratedObject>
        
        where TIntegratedObject : class
        where TDbContext : DbContext
    {
        protected readonly TDbContext dbContext;

        public IntegrationService()
        {
        }

        public IntegrationService(TDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnDbSetChanged([NotNullNotEmpty] EntityState state, Dictionary<string, object> before, Dictionary<string, object> after)
        {
            try
            {
                //MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(state, before, after);
                TIntegratedObject beforeChanges = null;
                TIntegratedObject afterChanges = null;
                if (before is not null)
                {
                    beforeChanges = typeof(TIntegratedObject).New<TIntegratedObject>();

                    foreach (var item in before)
                        beforeChanges.SetValue(item.Key, item.Value);
                }
                if (after is not null)
                {
                    afterChanges = typeof(TIntegratedObject).New<TIntegratedObject>();
                    foreach (var item in after)
                        afterChanges.SetValue(item.Key, item.Value);
                }
                switch (state)
                {
                    case EntityState.Added: OnAdded(afterChanges); break;
                    case EntityState.Deleted: OnDeleted(beforeChanges); break;
                    case EntityState.Modified:
                        var changes = beforeChanges.Compare(afterChanges);
                        OnUpdated(changes, beforeChanges, afterChanges);
                        break;
                        throw new NotSupportedException($"Не поддерживается событие {state}");
                }
            }
            catch (Exception ex)
            {
                this.Error($"Ошибка при обработки события изменения объекта интеграции: {typeof(TIntegratedObject).GetTypeName()} => \n {ex}");
            }

        }

        protected abstract void OnUpdated(Dictionary<string, PropertyChangedMessage> changes, TIntegratedObject beforeChanges, TIntegratedObject afterChanges);
        protected abstract void OnDeleted(TIntegratedObject afterChanges);
        protected abstract void OnAdded(TIntegratedObject afterChanges);
    }
}
