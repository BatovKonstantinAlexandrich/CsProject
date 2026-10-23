using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibViews.ViewEvents.List;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{
    public class ClassDbBuilder : ClassDbCompiler
    {
        public ChangingList<ClassModel> EntityClassModels { get; set; }

        public ClassDbBuilder()
        {
            EntityClassModels = new();
        }

        public void AddDbContextEntities<TDbContext>()
            where TDbContext : DbContextBase
        {
            var entityTypes = typeof(TDbContext).GetEntitiesTypes();
            foreach(var entityType in entityTypes)
            {
                var classModel = CreateClassModel(entityType);
                EntityClassModels.Add(classModel);
            }
        }

        private ClassModel CreateClassModel(Type entityType)
        {
             using (var db = typeof(ClassDbContext).New<ClassDbContext>()) 
                return db.CreateClassModel(entityType);
        }

        public Assembly BuildDbContext(string DbContextName)
        {
            while (true)
            {
                try
                {
                    //this.Info(DbContextName);
                    //this.Info(EntityClassModels.Count);
                    var entityTypes = DbContextName.ToType().New<DbContext>().GetEntitiesTypes();

                    EntityClassModels.AddRange(entityTypes.Select(CreateClassModel).ToList());
                    var dll = CompileDll(DbContextName, EntityClassModels);
                    var build = Assembly.Load(dll);
                    return build;
                }
                catch (Exception ex)
                {
                    this.Error(ex.ToDocument());
                    InputConsole.ConfirmContinue();
                }
            }
        }

    }
}
