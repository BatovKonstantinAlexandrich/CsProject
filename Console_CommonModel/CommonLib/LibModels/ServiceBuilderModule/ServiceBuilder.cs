using NSwag.CodeGeneration.CSharp;
using NSwag.CodeGeneration.TypeScript;
using NSwag;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Net.Http;
using Console_CommonModel.CommonLib.Others;

namespace Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule
{
    public class ServiceBuilder<TServiceDbContext, TServiceModel> : IServiceBuilder<TServiceModel>
        where TServiceDbContext : ServiceDbContextBase<TServiceModel>
        where TServiceModel : ServiceModel
    {
        [NotInput]
        public ConcurrentDictionary<string, Func<Dictionary<string, object>, MethodResult<object>>> endpoints = new();

        [NotInput]
        public Action<string> Error = error => { };

        [NotInput]
        private TServiceDbContext serviceDbContext;

        public ServiceBuilder(TServiceDbContext serviceDbContext)
        {
            this.serviceDbContext = serviceDbContext;

        }

        public void Init()
        {
            try
            {
                foreach (var model in serviceDbContext?.ServiceModels?.ToList())
                {
                    CreateServiceModel(model);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public ConcurrentDictionary<string, Func<Dictionary<string, object>, MethodResult<object>>>
            CreateOpenApiServiceModel(ServiceModel model)
        {
            var assembly = Assembly.Load(model.Dll);
            ConcurrentDictionary<string, Func<Dictionary<string, object>, MethodResult<object>>> results = new();
            var clients = assembly.GetTypes().Where(p => p.Name.IndexOf("Client") != -1);
            foreach (var client in clients)
            {
                var clientType = client;

                foreach (var method in client.GetMethods())
                {
                    var url = $"/{client.Name}/{method.Name}";
                    endpoints[url] = (arguments) =>
                    {
                        try
                        {
                            int n = model.Schema.IndexOf("://") + 3;
                            var lastIndex = model.Schema.Substring(n, model.Schema.Length - n).IndexOf("/");
                            lastIndex = lastIndex == -1 ? model.Schema.Substring(0, model.Schema.Length - n).Length - 1 : lastIndex;
                            lastIndex += n;
                            var url = model.Schema.Substring(0, lastIndex);

                            var clientRef = clientType.New<object>(new object[] { url, new HttpClient() });
                            var methodRef = clientType.GetMethods().FirstOrDefault(m => m.Name == method.Name);
                            var methodParams = methodRef.GetParameters();
                            var paramKeys = methodParams.OrderBy(mp => mp.Position).Select(mp => mp.Name).ToList();
                            var missed = paramKeys.Where(k => !arguments.ContainsKey(k));
                            if (missed.Any())
                            {
                                return MethodResult<object>.OnError(new Exception($"Передайте параметры: {missed}"));
                            }
                            else
                            {
                                object result = clientRef.Call(method.Name, arguments);
                                if (result is IAsyncResult)
                                {
                                    result = ((dynamic)result).Result;
                                }

                                return MethodResult<object>.OnResult(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            return MethodResult<object>.OnError(ex);
                        }
                    };
                }
            }
            return results;
        }

        public void CreateServiceModel(TServiceModel model)
        {
            try
            {
                var assembly = Assembly.Load(model.Dll);
                switch (model.Type)
                {
                    case "WSDL":
                        InstallWsdl(model.Schema, assembly); break;
                        break;
                    case "OpenAPI":
                        var clients = assembly.GetTypes().Where(p => p.Name.IndexOf("Client") != -1);
                        foreach (var client in clients)
                        {
                            var clientType = client;

                            foreach (var method in client.GetMethods())
                            {
                                var url = $"/{client.Name}/{method.Name}";
                                endpoints[url] = (arguments) =>
                                {
                                    try
                                    {
                                        int n = model.Schema.IndexOf("://") + 3;
                                        var lastIndex = model.Schema.Substring(n, model.Schema.Length - n).IndexOf("/");
                                        lastIndex = lastIndex == -1 ? model.Schema.Substring(0, model.Schema.Length - n).Length - 1 : lastIndex;
                                        lastIndex += n;
                                        var url = model.Schema.Substring(0, lastIndex);

                                        var clientRef = clientType.New<object>(new object[] { url, new HttpClient() });
                                        var methodRef = clientType.GetMethods().FirstOrDefault(m => m.Name == method.Name);
                                        var methodParams = methodRef.GetParameters();
                                        var paramKeys = methodParams.OrderBy(mp => mp.Position).Select(mp => mp.Name).ToList();
                                        var missed = paramKeys.Where(k => !arguments.ContainsKey(k));
                                        if (missed.Any())
                                        {
                                            return MethodResult<object>.OnError(new Exception($"Передайте параметры: {missed}"));
                                        }
                                        else
                                        {
                                            object result = clientRef.Call(method.Name, arguments);
                                            if (result is IAsyncResult)
                                            {
                                                result = ((dynamic)result).Result;
                                            }

                                            return MethodResult<object>.OnResult(result);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return MethodResult<object>.OnError(ex);
                                    }
                                };
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            catch (Exception ex)
            {
                Error($"При загрузки сервиса {model.Name} выброшено исключение: {ex}");
            }
        }

        /*public void Init()
{
   using (var db = new ServiceDbContext())
   {
       if(!db.Database.CanConnect())
       {
           Console.WriteLine("Не удаётся присоединиться");
       }
       foreach (var model in db.ServiceModels.ToList())
       {
           try
           {
               var assembly = Assembly.Load(model.Dll);
               switch (model.Type)
               {
                   case "WSDL":    InstallWsdl( model.Schema, assembly); break;
                   case "OpenAPI": InstallOpenApi( model.Schema ); break;
                   default:
                       throw new NotImplementedException();
               }
           }
           catch (Exception ex)
           {
               this.LogError($"При загрузки сервиса {model.Name} выброшено исключение: {ex}");
           }
       }
   }
}*/
        public MethodResult<object> ExecuteEndpoint(string key, Dictionary<string, object> args)
        {
            if (endpoints.ContainsKey(key))
            {
                try
                {
                    return endpoints[key](args);
                }
                catch (Exception ex)
                {
                    Error($"Исключение при выполнении: {key} => {ex.Message}");
                    throw;
                }
            }
            else
            {
                throw new ArgumentException();
            }

        }
        public void AddOpenApi(string url)
        {

            OpenApiDocument document = null;

            if (url.IsFile())
            {
                var readFile = OpenApiDocument.FromFileAsync(url);
                document = readFile.Result;
            }
            else if (url != null && url.ToLower().StartsWith("http"))
            {
                document = CreateOpenApiForUrl(url);
            }
            else
            {
                document = CreateOpenApiForUrl(url);
            }

            string output = "Client.cs";
            var shg = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
            string code = shg.GenerateFile();
            var build = CompileCs(code, output);

            using (var db = new ServiceDbContext())
            {
                if (!db.ServiceModels.Any(x => x.Name == url))
                {
                    db.Add(new ServiceModel()
                    {
                        Name = url,
                        Schema = url,
                        Type = "OpenAPI",
                        Dll = build
                    });
                    db.SaveChanges();
                }
            }

            var assembly = Assembly.Load(build);
            var clients = assembly.GetTypes().Where(p => p.Name.IndexOf("Client") != -1);
            foreach (var client in clients)
            {
                foreach (var method in client.GetMethods())
                {
                    Console.WriteLine($"/{client.Name}/{method.Name}");
                }
            }
        }


        private OpenApiDocument InstallOpenApi(string schema)
        {
            var url = schema;
            OpenApiDocument document = null;

            if (url.IsFile())
            {
                var readFile = OpenApiDocument.FromFileAsync(url);
                document = readFile.Result;
            }
            else if (url != null && url.ToLower().StartsWith("http"))
            {
                document = CreateOpenApiForUrl(url);
            }
            else
            {
                document = CreateOpenApiForUrl(url);
            }


            return document;
        }


        public OpenApiDocument CreateOpenApiForUrl(string url)
        {
            OpenApiDocument document;
            var readUrl = OpenApiDocument.FromUrlAsync(url);
            document = readUrl.Result;

            OnOpenApiDocumentReaded(document, url);
            return document;
        }


        public static string ReplaceAll(string text, string s1, string s2)
        {
            string p = text;
            while (p.IndexOf(s1) != -1)
            {
                p = p.Replace(s1, s2);
            }
            return p;
        }


        public static string CmdExec(string command)
        {
            command = ReplaceAll(ReplaceAll(ReplaceAll(ReplaceAll(command, "\n", ""), "\r", ""), @"\\", @"\"), @"//", @"/");
            Console.WriteLine(command);
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C " + command);

            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            Process process = Process.Start(info);

            string response = process.StandardOutput.ReadToEnd();
            string result = ReplaceAll(response, "\r", "\n");
            result = ReplaceAll(result, "\n\n", "\n");
            while (result.EndsWith("\n"))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }


        private static byte[] CompileCs(string code, string output)
        {
            var path = "D:\\project1";
            string filename = Path.Combine(path, output);
            var name = filename.Substring(0, filename.LastIndexOf(".") - 1);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //clear dir
            Directory.GetDirectories(path).ToList().ForEach(p => Directory.Delete(p, true));
            Directory.GetFiles(path).ToList().ForEach(File.Delete);

            //новый проект
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet new webapi -f net6.0 --force");
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft");
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft.Json");

            var fileout = Path.Combine(path, filename);
            File.WriteAllText(fileout, code);

            Console.WriteLine(CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet build"));
            var dll_path = Directory.GetFiles(path, "Project1.dll", SearchOption.AllDirectories).FirstOrDefault();
            path = Path.Combine(path, "bin");
            path = Path.Combine(path, "Debug");
            path = Path.Combine(path, "net6.0");
            dll_path = Directory.GetFiles(path, "Project1.dll", SearchOption.AllDirectories).FirstOrDefault();
            var build = Assembly.LoadFile(dll_path);
            foreach (var type in build.GetTypes())
            {
                Console.WriteLine(type.GetTypeName());
            }

            Console.WriteLine($"Записано а {filename} {code.Length} байт");
            return File.ReadAllBytes(dll_path);
        }



        private void OnOpenApiDocumentReaded(OpenApiDocument document, string url)
        {
            string output = "Client.cs";
            var shg = new CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
            string code = shg.GenerateFile();
            var build = CompileCs(code, output);

            using (var db = new ServiceDbContext())
            {
                if (!db.ServiceModels.Any(x => x.Name == url))
                {
                    db.Add(new ServiceModel()
                    {
                        Name = url,
                        Schema = url,
                        Type = "OpenAPI",
                        Dll = build
                    });
                    db.SaveChanges();
                }
            }
        }


        /// <summary>
        /// Считывание WSDL
        /// </summary>    
        private Dictionary<Type, IEnumerable<Type>> InstallWsdl(string url, Assembly build)
        {
            //интерфейсы служб
            var services = build.GetTypes().Where(t => t.GetCustomAttributes(true).Any(a => a.GetType().GetTypeName() == "ServiceContractAttribute"));

            Dictionary<Type, IEnumerable<Type>> serviceTypes = new Dictionary<Type, IEnumerable<Type>>();
            foreach (var service in services)
            {
                var serviceImplementactions = build.GetTypes().Where(t => t.IsInterface == false && t.IsImplements(service));
                serviceTypes[service] = serviceImplementactions;
            }

            Dictionary<string, Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>>> validations = new();
            Dictionary<string, Func<Dictionary<string, object>, MethodResult<object>>> api = new();
            foreach (var kv in serviceTypes)
            {
                Type interfaceType = kv.Key;
                Dictionary<string, Type> names = new(kv.Value.Select(t => new KeyValuePair<string, Type>(t.GetTypeName(), t)));
                foreach (var nkv in names)
                {
                    var name = nkv.Key;
                    var serviceType = nkv.Value;
                    foreach (var action in nkv.Value.GetOwnMethodNames())
                    {
                        Func<Dictionary<string, object>, MethodResult<object>> todo = CreateCaller(serviceType, action);
                        api[$"/{interfaceType.GetTypeName()}/{action}"] = todo;
                        validations[$"/{interfaceType.GetTypeName()}/{action}"] = CreateValidator(serviceType, action);
                    }
                }
            }


            /*if (InputConsole.ConfirmActionDialog("Желаете протестировать методы API", () => {

                var options = new List<string>(api.Keys);
                options.Add("Завершить тестирование");
                string selected = null;
                object result = new object();
                switch ((selected = InputConsole.SingleSelect("Выберите метод", options)))
                {
                    case "Завершить тестирование": break;
                    default:
                        {

                            Dictionary<string, object> args = new();
                            var _args = new string[0];
                            var typeName = selected.Substring(2);
                            var actionName = typeName.Substring(typeName.IndexOf("/") + 1);
                            typeName = typeName.Substring(0, typeName.IndexOf("/"));
                            var argumentValidations = validations[selected](args);
                            bool completed = false;

                            while (!completed)
                            {
                                switch (InputConsole.SingleSelect($"Тестирование метода: {selected}\n\n" +
                                    $"Аргументы:\n" + $"{args.ToJsonOnScreen()}\n\n" +
                                    $"Валидация:\n" + $"{argumentValidations.ToJsonOnScreen()}\n\n" +
                                    $"Результат:\n" + $"{result.ToJsonOnScreen()}",
                                        "задать значение,удалить значение,выполнить метод,завершить тестирование".Split(",").ToList(), ref _args))
                                {
                                    case "удалить значение":
                                        {
                                            var key = InputConsole.SingleSelect("Выберите аргумент", args.Keys.ToList(), ref _args);
                                            args.Remove(key);
                                            break;
                                        }
                                    case "задать значение":
                                        {
                                            var key = InputConsole.InputString("Имя аргумента", null, ref _args);
                                            var value = InputConsole.InputString("Значение аргумента", null, ref _args);
                                            args[key] = value;
                                            break;
                                        }
                                    case "выполнить метод":
                                        {
                                            result = api[selected](args);
                                            break;
                                        }
                                    case "завершить тестирование":
                                        completed = true;
                                        break;
                                }
                            }

                            break;
                        }

                }

            })) ;*/
            foreach (var p in api)
            {
                endpoints[p.Key] = p.Value;
            }
            return serviceTypes;
        }


        public void AddWsdl(string url)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "project");
            var name = url.Substring(url.LastIndexOf("/") + 1);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            //clear dir
            try
            {
                Directory.GetDirectories(path).ToList().ForEach(p => Directory.Delete(p, true));
                Directory.GetFiles(path).ToList().ForEach(File.Delete);
            }
            catch
            {
            }

            //новый проект
            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet new webapi -f net6.0 --force");

            //добавление ссылки
            ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $" /C {path.Substring(0, 2)} && cd {path} && dotnet-svcutil {url}");
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;
            info.UseShellExecute = false;
            Process process = Process.Start(info);
            Console.WriteLine(process.StandardOutput.ReadToEnd());

            CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet build");
            var dll_path = Directory.GetFiles(path, "project.dll", SearchOption.AllDirectories).FirstOrDefault();
            var build = Assembly.LoadFile(dll_path);

            using (var db = new ServiceDbContext())
            {
                if (!db.ServiceModels.Any(x => x.Name == name))
                {
                    db.Add(new ServiceModel()
                    {
                        Name = name,
                        Schema = url,
                        Type = "WSDL",
                        Dll = File.ReadAllBytes(dll_path)
                    });
                    db.SaveChanges();
                }

            }
            //интерфейсы служб
            var services = build.GetTypes().Where(t => t.GetCustomAttributes(true).Any(a => a.GetType().GetTypeName() == "ServiceContractAttribute"));

            Dictionary<Type, IEnumerable<Type>> serviceTypes = new Dictionary<Type, IEnumerable<Type>>();
            foreach (var service in services)
            {
                var serviceImplementactions = build.GetTypes().Where(t => t.IsInterface == false && t.IsImplements(service));
                serviceTypes[service] = serviceImplementactions;
            }

            Dictionary<string, Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>>> validations = new();
            Dictionary<string, Func<Dictionary<string, object>, MethodResult<object>>> api = new();
            foreach (var kv in serviceTypes)
            {
                Type interfaceType = kv.Key;
                Dictionary<string, Type> names = new(kv.Value.Select(t => new KeyValuePair<string, Type>(t.GetTypeName(), t)));
                foreach (var nkv in names)
                {
                    var serviceName = nkv.Key;
                    var serviceType = nkv.Value;
                    foreach (var action in nkv.Value.GetOwnMethodNames())
                    {
                        Func<Dictionary<string, object>, MethodResult<object>> todo = CreateCaller(serviceType, action);
                        api[$"/{interfaceType.GetTypeName()}/{action}"] = todo;
                        validations[$"/{interfaceType.GetTypeName()}/{action}"] = CreateValidator(serviceType, action);
                    }
                }
            }

            foreach (var kv in api)
            {
                endpoints[kv.Key] = kv.Value;
            }
        }

        public Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> CreateValidator(Type serviceType, string action)
        {
            string methodName = action;
            Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> validate = (request) =>
            {
                var started = DateTime.Now;
                var service = serviceType.New<object>(new object[0]);
                var action = serviceType.GetMethod(methodName);
                var validations = action.Validate(request);
                return validations;
            };
            return validate;
        }
        public Func<Dictionary<string, object>, MethodResult<object>> CreateCaller(Type serviceType, string action)
        {
            string methodName = action;
            Func<Dictionary<string, object>, MethodResult<object>> todo = (request) =>
            {
                var started = DateTime.Now;
                var service = serviceType.Create(new object[0]);
                var action = serviceType.GetMethod(methodName);
                var validations = action.Validate(request);
                MethodResult<object> response = null;
                if (validations != null && validations.Count() > 0 && validations.Any(res => res.Value.Count() > 0))
                {
                    try
                    {
                        var args = action.GetArgumentsFromDictionary(request);
                        var result = action.Invoke(service, args.ToArray());
                        response = MethodResult<object>.OnComplete(result, request, started);
                    }
                    catch (Exception ex)
                    {
                        response = MethodResult<object>.OnError(request, new Exception("Ошибка при выполнении", ex));
                    }
                }
                else
                {
                    response = MethodResult<object>.OnValidationError(started, request, validations);
                }
                return response;
            };
            return todo;
        }

        internal void Print()
        {

            InputConsole.Clear();
            InputConsole.WriteLine("Конечные точки");
            foreach (var kv in endpoints)
            {
                InputConsole.WriteLine(kv.Key);
            }
        }

        public List<string> GetRouteOptions(string path = "/")
        {
            InputConsole.Clear();
            InputConsole.WriteLine("Конечные точки");
            return endpoints.Keys.Where(p => p.StartsWith(path)).ToList();
        }

        public List<string> GetAllRouteOptions()
        {
            InputConsole.Clear();
            InputConsole.WriteLine("Конечные точки");
            return endpoints.Keys.ToList();
        }
    }
}
