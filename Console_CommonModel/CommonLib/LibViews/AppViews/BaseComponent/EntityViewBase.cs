using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent
{
    [Label("Базовое представление сущности")]
    public class EntityViewBase<TDbContext, TEntity> : AppViewBase
        where TEntity : BaseEntity
        where TDbContext : DeliveryDbContext
    {
        private readonly TEntity entity;

        public EntityViewBase() : this(default) { }

        public EntityViewBase(TEntity entity)
        {
            this.entity = entity;
        }

        public Action AddContextBusinessActions()
        {
            var businessService = GetService<UserBusinessService<TDbContext>>();


            businessService.GetBusinessLogic<TEntity>();
            var businessFunctions = businessService.GetBusinessFunctions();
            using (var db = typeof(TDbContext).New<DbContextBase>())
            {
                var roleIds = GetService<SigninUser<DeliveryDbContext>>().Verify().UsersResources.Select(r => r.RoleId);

                foreach (var businessFunction in businessFunctions)
                {

                }
            }

            return () => { };

        }

    }
}
