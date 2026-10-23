using Microsoft.EntityFrameworkCore;

using Console_CommonModel.CommonLib.LibModels.Console_ModelsModule;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;

namespace Console_CommonModel.CommonLib.LibModels.Console_ServiceModule
{

    /// <summary>
    /// Сервис интеграции на уровне данных. 
    /// Создаёт новые записи в другую таблицу, при изменении интегрируемого объекта.
    /// </summary>
    /// <typeparam name="TDbContext">Тип контекста</typeparam>
    /// <typeparam name="TIntegratedObject">Тип сущности изменения которого приводят к изменению данных</typeparam>
    /// <typeparam name="TDataObject">Тип сущности который создаётся автоматически</typeparam>
    public abstract class DataIntegration<TDbContext, TIntegratedObject, TDataObject> : 
        IntegrationService<TDbContext, TIntegratedObject>
        where TIntegratedObject : class
        where TDbContext : DbContext
        where TDataObject : BaseEntity
    {

        public DataIntegration() : base()
        {
        }

        public DataIntegration(TDbContext dbContext) : base(dbContext)
        {

        }

        protected override void OnUpdated(
            Dictionary<string, PropertyChangedMessage> changes,
            TIntegratedObject beforeChanges,
            TIntegratedObject afterChanges)
        {
            var b = FindDataObject(beforeChanges);
            var a = CreateDataObject(afterChanges);
            a.Id = b.Id;
            dbContext.Update(a);
            dbContext.SaveChanges();
        }

        protected override void OnDeleted(TIntegratedObject afterChanges)
        {
            var target = FindDataObject(afterChanges);
            using (var db = new ModelsDbContext())
            {
                db.Remove(target);
                db.SaveChanges();
            }

        }

        protected override void OnAdded(TIntegratedObject afterChanges)
        {
            var target = CreateDataObject(afterChanges);
            using (var db = new ModelsDbContext())
            {
                
                db.Add(target);
                db.SaveChanges();
            }
        }

        protected abstract TDataObject FindDataObject(TIntegratedObject integrated);
        protected abstract TDataObject CreateDataObject(TIntegratedObject integrated);
    }
}
