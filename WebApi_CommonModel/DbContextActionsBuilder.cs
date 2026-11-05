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


        public Dictionary<string, HubActionModel> AddServiceMethods (object serviceRef)
        {
            Dictionary<string, HubActionModel> res = new();
            var names = serviceRef.GetOwnMethodNotStaticNames();
            this.Debug(names);
            foreach (var name in names)
            {
                var a = this.AddServiceMethod(serviceRef, name);
                res[a.Name] = a;
            }
            return res;
        }

        public HubActionModel AddServiceMethod(object serviceRef, string methodName)
        {
            using (var classDbContext = new ClassDbContext())
            {
                var classModel = classDbContext.CreateClassModel(serviceRef.GetType());
                var methodModel = classModel.Methods.FirstOrDefault(m => m.Name == methodName);
                var actionUrl = $"/api/servises/{serviceRef.GetTypeName()}/actions/{methodName}";
                return new HubActionModel(actionUrl, (args, onsuccess, onerror) => {
                    //TODO: action
                    var argv = args.Select(kv => new KeyValuePair<string, string>(kv.Key, $"{kv.Value}")).ToDictionary();
                    serviceRef.Invoke(methodName, argv);
                    
                });
            }

        }

      

        public Dictionary<string, HubActionModel> Build(ActionDbContext.ActionServiceModel serviceModel)
        {
            Dictionary<string, HubActionModel> r = new();
            foreach (var type in Assembly.Load(serviceModel.AssemblyDll).GetTypesExtended<DbContextBase>())
            {
                using (var dbc = type.New<DbContextBase>())
                {
                    r.AddRange<string, HubActionModel>(AddServiceMethods(dbc));
                }
            }
            return r;
        }
    }
}