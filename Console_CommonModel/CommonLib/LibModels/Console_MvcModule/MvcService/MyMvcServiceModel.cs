using Console_CommonModel.CommonLib.LibModels.Console_LocationModule;

using Microsoft.EntityFrameworkCore;

using Mysqlx.Crud;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Console_CommonModel.CommonLib.LibModels.Console_MvcModule.MvcService
{
    public class MyMvcServiceModel
    {    

        public MyMvcServiceModel()
        {            
        }

        public List<string> GetMvcApplications()
        {
            using (var db = new MyMvcDbContext())
            {
                db.Database.SetCommandTimeout(15);
                return db.ApplicationModels.Select(app => app.Name).ToList();
            }
        }


        public List<string> GetMvcControllers(string appName)
        {
            using (var db = new MyMvcDbContext())
            {
                db.Database.SetCommandTimeout(15);
                return db.ControllerModels.Include(p => p.Parent).Where(ctrl => ctrl.Parent.Name == appName).Select(app => app.Name).ToList();
            }
        }

        public List<string> GetMvcControllerActions(string appName, string ctrlName)
        {
            using (var db = new MyMvcDbContext())
            {
                db.Database.SetCommandTimeout(15);
                var controller = db.ControllerModels
                    .Include(p => p.Parent)
                    .Include(p => p.ActionModels)
                    .Where(ctrl => ctrl.Parent.Name == appName)
                    .FirstOrDefault(ctrl => ctrl.Name == ctrlName);
                if (controller?.ActionModels == null)
                    return new List<string>();
                return controller.ActionModels.Select(a => a.Name).ToList();
            }
        }

        private void GetMvcApplication(string appName)
        {
            var result = GetOrCreateByName<MyApplicationModel>(appName);
            if (result.Succeeded)
            {
                var appModel = result.Result;
                var router = new MyMvcApplicationRouter();
            }
            else
            {
                this.Error(result.Exception);
                throw new Exception(result.Exception);
            }
        }


        public void AddOperation(string appName, string userRole, string actionName, Dictionary<string, MyParameterDeclarationModel> input)
        {
            var app = GetOrCreateByName<MyActionModel>(appName);
        }



        public dynamic GetOrCreateByNameForType(Type TEntity, [InputEngWord][NotNullNotEmpty] string name, List<string> path = null)
        {
            return GetType().GetMethod(nameof(GetOrCreateByName)).MakeGenericMethod(TEntity).Invoke(this, new object[] { name, path });
        }


        public MethodResult<TEntity> GetOrCreateByName<TEntity>([InputEngWord][NotNullNotEmpty] string name, List<string> path = null)
            where TEntity : HierTable
        {
            MethodBase.GetCurrentMethod().EnsureAgumentsIsValid(name);
            try
            {
                using (var db = new MyMvcDbContext())
                {

                    var p = db.Set<TEntity>().FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
                    Action setParent = () =>
                    {
                        var parentType = typeof(TEntity).GetProperty("Parent")?.PropertyType;
                        var parentName = path.Last();
                        path.RemoveAt(path.Count() - 1);
                        var result = GetOrCreateByNameForType(parentType, parentName, path);
                        if (result.Succeeded)
                        {
                            p.ParentId = result.Result.GetValue<int>("Id");
                            ObjectValidationExtensions.SetValue(p, "Parent", result.Result);
                            db.Set<TEntity>().Add(p);
                            db.SaveChanges();

                        }
                        else
                        {
                            throw new Exception(result.Exception);
                        }
                    };
                    if (p is null)
                    {
                        p = typeof(TEntity).New<TEntity>();
                        p.Name = name;

                        if (path is null || path.Count() == 0)
                        {
                            setParent();
                            db.Set<TEntity>().Add(p);
                            db.SaveChanges();
                            return MethodResult<TEntity>.OnResult(p);
                        }
                        else
                        {

                            return MethodResult<TEntity>.OnResult(p);
                        }
                    }
                    setParent();
                    return MethodResult<TEntity>.OnResult(p);
                }
            }
            catch (Exception ex)
            {
                this.Error(ex);
                return MethodResult<TEntity>.OnError(ex);
            }
        }

        public MyActionModel GetMvcControllerActionModel(string appName, string ctrlName, string actionName)
        {
            using (var db = new MyMvcDbContext())
            {
                var model = db.ControllerModels
                    .Include(p => p.Parent)
                    .Include(p => p.ActionModels)                         
                    .Where(ctrl => ctrl.Parent.Name == appName)
                    .FirstOrDefault(ctrl => ctrl.Name == ctrlName)
                    .ActionModels.First(a => a.Name== actionName);
                model.Parameters = new Dictionary<string,MyParameterDeclarationModel> (db.ParameterModels.Where(p => p.ParentId == model.Id).Select(p => new KeyValuePair<string,MyParameterDeclarationModel>(p.Name, p)).ToList());
                return model;
            }
        }
    }
}
