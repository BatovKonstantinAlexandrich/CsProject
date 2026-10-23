using Console_CommonModel.CommonLib.LibData.RepositoryPattern;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibData
{
    public class LibDataUnit : TestingUnit
    {
        public LibDataUnit(IServiceProvider parent) : base(parent)
        {
            this.Append(new RepositoryTest(parent));
            this.Append(new DbContextUserTest(parent));
        }

        
    }

    public class DbContextUserTest : TestingElement
    {
        public DbContextUserTest(IServiceProvider provider) : base(provider)
        {
        }

        public override void OnTest()
        {
            Assert(p => {
                using (var db = new DbContextUser())
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    db.CheckRegistration<BusinessResource>("Driver");
                    var result = db.GetSchemasNames().Contains("Driver");
                    return result;
                }                
            },
            "При регистрации бизнес-ресурса создаётся схема с именем ресурса",
            "При регистрации бизнес-ресурса не создаётся схема с именем ресурса");

            Assert(p => {
                using (var db = new DbContextUser())
                {
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                    db.CheckRegistration<BusinessResource>("Driver");
                    var result = db.GetSqlServerWebApi().GetTableNames().Contains("Driver");
                    return result;
                }
                
            },
            "При регистрации бизнес-ресурса создаётся таблица связанная с пользователей",
            "При регистрации бизнес-ресурса не создаётся таблица связанная с пользователей");
        }
    }
}
