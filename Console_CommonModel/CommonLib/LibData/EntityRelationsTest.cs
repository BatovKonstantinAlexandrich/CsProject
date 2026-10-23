using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;

using pickpoint_delivery_service;

namespace Console_CommonModel.CommonLib.LibData
{
    [Label("Проверка функции получения отношений между сущностями")]

    public class EntityRelationsTest : TestingElement
    {
        public EntityRelationsTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            Assert(p =>
            {
                using (var db = new DeliveryDbContext())
                {
                    var user = typeof(UserContext).New<BaseEntity>();
                    db.Info(user.GetEntityRelations());
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    foreach (string migration in db.Database.GetMigrations())
                    {
                        db.Info(migration);
                    }
                    foreach (var ptype in db.GetEntitiesTypes())
                    {
                        try
                        {
                            InputConsole.Clear();
                            InputConsole.WriteLine(ptype.GetTypeName());
                            db.Info(ptype.New<BaseEntity>().GetEntityRelations());
                            db.Info(db.GetEntityRelationsForType(ptype));
                        }
                        finally
                        {
                            InputConsole.ConfirmContinue();
                        }
                    }
                }
                return true;
            }
            , "Функция получения отношений между сущностями работает корректно"
            , "Функция получения отношений между сущностями работает с ошибками");
        }
    }


}

