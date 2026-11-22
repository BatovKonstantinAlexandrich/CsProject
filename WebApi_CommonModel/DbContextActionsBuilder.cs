using ClassModelLib.ActionsClient;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using System.Reflection;
using WebApi_CommonModel.MiddlewareComponents;


namespace WebApi_CommonModel.WebApiModule
{

    public class DbContextActionsBuilder: IActionBuilder
    {
        public DbContextActionsBuilder()
        {
        }


        public Dictionary<string, ActionsModel> AddServiceMethods(object serviceRef)
        {
            Dictionary<string, ActionsModel> res = new();
            var names = serviceRef.GetOwnMethodNotStaticNames();
            this.Debug(names);
            foreach (var name in names)
            {
                var a = this.AddServiceMethod(serviceRef, name);
                res[a.Name] = a;
            }
            return res;
        }


        public ActionsModel AddServiceMethod(object serviceRef, string methodName)
        {
            using (var classDbContext = new ClassDbContext())
            {
                var classModel = classDbContext.CreateClassModel(serviceRef.GetType());
                var methodModel = classModel.Methods.FirstOrDefault(m => m.Name == methodName);
                var actionUrl = $"/api/services/{serviceRef.GetTypeName()}/actions/{methodName}";
                return new ActionsModel(actionUrl, (args, onsuccess, onerror) => {
                    //TODO: action
                    var argv = args.Select(kv => new KeyValuePair<string, string>(kv.Key, $"{kv.Value}")).ToDictionary();
                    serviceRef.Invoke(methodName, argv);
                    
                });
            }

        }

      

        public Dictionary<string, ActionsModel> Build(ActionDbContext.ActionServiceModel serviceModel)
        {
            Dictionary<string, ActionsModel> r = new();
            foreach (var type in Assembly.Load(serviceModel.AssemblyDll).GetTypesExtended<DbContextBase>())
            {
                using (var dbc = type.New<DbContextBase>())
                {
                    r.AddRange<string, ActionsModel>(AddServiceMethods(dbc));
                }
            }
            return r;
        }
    }
}