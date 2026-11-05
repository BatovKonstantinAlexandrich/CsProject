using ClassModelLib.ActionsClient;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model;
using pickpoint_delivery_service;
using System.Collections.Concurrent;
using System.Text;
using static ClassModelLib.ActionsClient.ActionDbContext;

namespace WebApi_CommonModel.MiddlewareComponents
{

    [Label("Выполнение динамические функции")]
    public class ServiceMethodsMiddleware<TServiceDbContext> : AsyncContext, IMiddleware, IServiceMethodsMiddleware
        where TServiceDbContext : ActionDbContext
    {

        /// <summary>
        /// Операции по наименованиям внешних сервисов
        /// </summary>
        public ConcurrentDictionary<string, HubActionModel> Actions = new ConcurrentDictionary<string, HubActionModel>();
        private IServiceScopeFactory _serviceProvider;

        [NotInput]
        public ServiceMethodsMiddleware<TServiceDbContext> SetServiceProvider(IServiceScopeFactory serviceProvider) 
        {
            _serviceProvider = serviceProvider;
            return this;
        }

        public ServiceMethodsMiddleware(IServiceScopeFactory serviceProvider = null)
        {
            this._serviceProvider = serviceProvider;
            this.InitSelfControl();

            this.AddServiceMethod<ServiceMethodsMiddleware<TServiceDbContext>>(nameof(ServiceMethodsMiddleware<TServiceDbContext>.Test1));
            this.AddServiceMethods<SigninUser<DeliveryDbContext>>();

            this.LoadWithBuilders<TServiceDbContext>();
        }


        [NotInput]
        private Action InitSelfControl()
        {
            var ctrl = this;
            Dictionary<string, HubActionModel> res = new();

            var methodNames = typeof(IServiceMethodsMiddleware).GetMethods().Where(m => !m.IsStatic && m.DeclaringType!=typeof(Object)).Select(m => m.Name).ToList();
            Actions[$"/api/services/"] = new HubActionModel($"/api/services/", (args, onsuccess, onerror) =>
            {
                try
                {
                    onsuccess(methodNames);
                }
                catch (Exception ex)
                {
                    this.Error(ex);
                    onerror(ex.Message);
                    throw;
                }
            });
            foreach (var methodName in methodNames)
            {
                using (var classDbContext = new ClassDbContext())
                {
                    var classModel = classDbContext.CreateClassModel(ctrl.GetType());
                    var methodModel = classModel.Methods.FirstOrDefault(m => m.Name == methodName);
                    var actionUrl = $"/api/actions/{methodName}";
                    Actions[actionUrl] = res[methodName] = new HubActionModel(actionUrl, (args, onsuccess, onerror) =>
                    {
                        //DoAction(actionUrl, args);
                        try
                        {
                            using (var scope = _serviceProvider.CreateScope())
                            {
                                var service = ctrl;

                                Dictionary<string, object> objectArgs = new();
                                foreach (var kv in args)
                                {
                                    objectArgs[kv.Key] = kv.Value;
                                }
                                object result = service.Invoke(methodName, objectArgs);
                                if (result is IAsyncResult ar)
                                {
                                    result = ((dynamic)result).Result;
                                }
                                onsuccess(result);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Error(ex);
                            onerror(ex.Message);
                            throw;
                        }
                    });
                }
            }

            return () => {
                Actions.TryRemove($"/api/services/", out var p);
                foreach(var name in methodNames.Select(methodName => $"/api/actions/{methodName}").ToList())
                {
                    Actions.TryRemove(name, out var pp);
                }                
            };
        }


        [Label("Построить сборки")]
        public Dictionary<string, HubActionModel> LoadWithBuilder<TActionDbContext, TActionBuilder>()
            where TActionDbContext : ActionDbContext
            where TActionBuilder : class, IActionBuilder
        {
            Dictionary<string, HubActionModel> res = new();
            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
            {
                this.InfoTime("Формирование операция сервиса", () =>
                {
                    var builder = typeof(TActionBuilder).New<TActionBuilder>();
                    foreach (var serviceModel in db.ActionServiceModels.ToList())
                    {
                        res.AddRange<string, HubActionModel>(builder.Build(serviceModel));
                    }
                });
            }
            return res;
        }

        [Label("Построить сборки")]
        [NotInput]
        public Dictionary<string, HubActionModel> LoadWithBuilders<TActionDbContext>(params IActionBuilder[] builders)
            where TActionDbContext : ActionDbContext
        {
            Dictionary<string, HubActionModel> res = new();
            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
            {
                this.Info(db.GetTypeName());
                this.InfoTime("Формирование операция сервиса", () =>
                {
                    foreach (var builder in builders)
                    {
                        foreach (var serviceModel in db.ActionServiceModels.ToList())
                        {
                            res.AddRange<string, HubActionModel>(builder.Build(serviceModel));
                        }
                    }
                });
            }
            return res;
        }

        [Label("Собрать элементы управления")]
        [NotInput]
        public Dictionary<string, HubActionModel> LoadControls<TActionDbContext>(params IControlService<ActionServiceModel>[] controls)
            where TActionDbContext : ActionDbContext
        {
            Dictionary<string, HubActionModel> res = new();
            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
            {
                this.InfoTime("Формирование операция сервиса", () =>
                {
                    foreach (ActionServiceModel serviceModel in db.ActionServiceModels.ToList())
                    {
                        foreach (var control in controls)
                        {
                            control.SetControl(serviceModel);
                        }
                    }

                });
            }
            return res;
        }


        [NotInput]
        public int Test1(int x) => x + 1;


        [NotInput]
        public List<string> GetRoutes() => Actions.Keys.ToList();



        /// <summary>
        /// Список операций
        /// </summary>
        [NotInput]
        public string GetActionList()
        {
            string result = "";
            Actions.Keys.ToList().ForEach(k => result += "," + k);
            return Actions.Count() > 0 ? result.Substring(1) : "Нет операций";
        }


        /// <summary>
        /// Регистрация операции
        /// </summary>
        [Label("Добавить метод")]
        public void AddAction(string Name, HubActionModel model)
        {
            System.Console.WriteLine($"Выполняем процедуру {nameof(AddAction)}");
            this.Actions[Name] = model;
        }


        /// <summary>
        /// Выполнение операции
        /// </summary>
        [NotInput()]
        public object DoAction(string Action, Dictionary<string, string> args)
        {
            System.Console.WriteLine($"Выполняем процедуру {Action}");
            HubActionModel todo = null;
            if (Actions.ContainsKey(Action) == false)
            {
                foreach (var kv in Actions)
                {
                    var parseResult = IsRoute(Action, kv.Key);
                    if (parseResult.Item1)
                    {
                        todo = kv.Value;
                        args.AddRange(parseResult.Item2);
                    }
                }
            }
            else
            {
                todo = Actions[Action];
            }

            if (todo is null)
            {
                throw new HttpRequestException(404, $"Не найден метод соответствующий зарегистрированным маршрутам.{GetRoutes().ToJsonOnScreen()}");
            }

            var result = todo.Exec(args);
            if (result is Task rt)
            {
                rt.Wait();
                return ((dynamic)rt).Result;
            }
            return result;
        }


        [NotInput]
        [Description("Сравнивает url с заданным маршрутом")]
        public (bool, Dictionary<string, string>) IsRoute(string url, string route)
        {
            Dictionary<string, string> values = new();
            try
            {
                List<string> expressions = new List<string>();
                while (route.IndexOf("{{") >= 0)
                {
                    int x2 = route.IndexOf("}}");
                    int x1 = route.IndexOf("{{");
                    var expression = route.Substring(x1 + 2, x2 - x1 - 2);
                    expressions.Add(expression);

                    if (url.Substring(0, x1).ToLower() != route.Substring(0, x1).ToLower())
                        return (false, new());
                    route = route.Length < (x2 + 2) ? "" : route.Substring(x2 + 2);
                    var i = url.Substring(x1).IndexOf("/");

                    var routeValue = url.Substring(x1, i == -1 ? (url.Length - x1) : i);
                    if (routeValue.IndexOf("/") != -1)
                        return (false, new());

                    values[expression] = routeValue;
                    url = url.Substring(x1 + routeValue.Length);
                }
                return (url == route, values);
            }
            catch (Exception ex)
            {
                return (false, new());
            }

        }



        [Label("Удалить метод")]
        public void RemoveAction(string Action)
        {
            if (Actions.ContainsKey(Action))
            {
                Actions.TryRemove(Action, out var p);
            }
        }





        [NotInput]
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var request = await GetRequestModel(context);
            var response = await HandleHttpRequest(request);
            await WriteResponse(context, response);

            await next.Invoke(context);
        }

        [NotInput]
        private async Task WriteResponse(HttpContext context, ActionResponse response)
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(response.ToJsonOnScreen());
        }


        [NotInput]
        public async Task<ActionResponse> HandleHttpRequest(RequestModel request)
        {
            var Token = GetToken(request);
            bool completed = false;
            bool success = false;
            object result = null;
            string _error = null;
            var serial = this.Put(new()
            {
                OnSuccess = response =>
                {
                    result = response;
                    success = true;
                    completed = true;
                },
                OnError = error =>
                {
                    _error = error;
                    success = false;
                    completed = true;
                },
            });
            var model = new ActionRequest()
            {
                SerialKey = serial,
                AccessToken = Token,
                ActionName = request.url,
                MessageObject = request.args
            };

            try
            {
                var data = this.DoAction(model.ActionName, model.MessageObject);
                var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                while (((DateTimeOffset.Now.ToUnixTimeMilliseconds() - started) > 3000) && !completed)
                {
                    Thread.Sleep(100);
                }
                return new ActionResponse()
                {
                    RequestMessage = model,
                    MessageObject = data,
                    Success = success,
                    SerialKey = serial
                };
            }
            catch (Exception ex)
            {
                this.Error(ex.Message);
                return new ActionResponse()
                {
                    StatusText = _error,
                    RequestMessage = model,
                    MessageObject = result,
                    Success = false,
                    SerialKey = serial
                };
            }

            
        }


        [NotInput]
        private string GetToken(RequestModel request)
        {
            return request.headers.ContainsKey("Authorization") ?
                request.headers["Authorization"] : null;
        }



        [NotInput]
        public async Task<RequestModel> GetRequestModel(HttpContext http)
        {
            var requestModel = new RequestModel();
            
            requestModel.url = Uri.UnescapeDataString(http.Request.Path.ToString());
            requestModel.headers = http.Request.Headers.ToDictionary(
                    h => h.Key,
                    h => string.Join(", ", h.Value)
            );
            requestModel.args = http.Request.Query.ToDictionary(
                    h => h.Key,
                    h => string.Join(", ", h.Value)
            );
            requestModel.body = await ReadBody(http);
            return requestModel;
        }


        [NotInput]
        public async Task<string> ReadBody(HttpContext context)
        {
            try
            {
                // Проверяем, что тело запроса можно читать
                if (context?.Request?.Body == null || !context.Request.Body.CanRead)
                    return string.Empty;

                using var reader = new StreamReader(
                    context.Request.Body,
                    Encoding.UTF8,
                    detectEncodingFromByteOrderMarks: false,
                    bufferSize: 1024,
                    leaveOpen: true
                );

                var body = await reader.ReadToEndAsync();

                // Сбрасываем позицию потока для возможного повторного чтения
                //context.Request.Body.Position = 0;


                return body;
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                this.Error($"Ошибка чтения тела запроса: {ex.Message}");
                return string.Empty;
            }
        }

        [NotInput]
        [Label("Зарегистрировать сервис")]
        public Dictionary<string, HubActionModel> AddServiceMethods<TService>()
            where TService : class
        {
            return AddServiceMethods(typeof(TService));
        }

        [NotInput]
        [Label("Зарегистрировать сервис")]
        public Dictionary<string, HubActionModel> AddServiceMethods(Type tservice)

        {
            Dictionary<string, HubActionModel> res = new();
            var names = tservice.GetOwnMethodNotStaticNames();
            this.Debug(names);
            Actions[$"/api/services/{tservice.GetTypeName()}/actions"] = new HubActionModel($"/api/services/{tservice.GetTypeName()}/actions", (args, onsuccess, onerror) => {
                onsuccess(new List<string>(names));
            });
            foreach (var name in names)
            {
                var a = this.AddServiceMethod(tservice, name);
                Actions[a.Name] = res[a.Name] = a;
            }
            return res;
        }

        [NotInput]
        [Label("Зарегистрировать сервис")]
        public Dictionary<string, HubActionModel> AddServiceMethodsAll(Type tservice)
        {
            Dictionary<string, HubActionModel> res = new();
            var names = tservice.GetMethods().Where(m => m.Name.StartsWith("add_") == false && m.Name.StartsWith("remove_") == false && m.Name.StartsWith("get_") == false && m.Name.StartsWith("set_") == false && !m.IsStatic && m.DeclaringType != typeof(Object)).Select(m => m.Name).ToList();
            this.Debug(names);
            Actions[$"/api/services/{tservice.GetTypeName()}/actions"] = new HubActionModel($"/api/services/{tservice.GetTypeName()}/actions", (args, onsuccess, onerror) => {
                onsuccess(new List<string>(names));
            });
            foreach (var name in names)
            {                
                if (!tservice.GetMethodAttributes(name).ContainsKey(nameof(NotInputAttribute)))
                {
                    var a = this.AddServiceMethod(tservice, name);
                    Actions[a.Name] = res[a.Name] = a;
                }
            }
            return res;
        }


        [NotInput]
        [Label("Зарегистрировать метод сервис")]
        public HubActionModel AddServiceMethod<TService>(string methodName)
            where TService : class
        {
            return AddServiceMethod(typeof(TService), methodName);
        }

        [NotInput]
        [Label("Зарегистрировать метод сервис")]
        public HubActionModel AddServiceMethod(Type tservice, string methodName)
        { 
            using (var classDbContext = new ClassDbContext())
            {
                var classModel = classDbContext.CreateClassModel(tservice);
                
                
                //classModel = classDbContext.CreateClassModel(tservice.GetMethodByName(methodName).DeclaringType);
                var methodModel = classModel.Methods.FirstOrDefault(m => m.Name == methodName);
                if(methodModel is null)
                {
                    methodModel = classDbContext.Methods.FirstOrDefault(m => m.Name == methodName);
                }
                if(methodModel is null)
                {
                    throw new ArgumentNullException($"Не удалось найти метод {methodName}");
                }
                var actionUrl = $"/api/services/{tservice.GetTypeName()}/actions/{methodName}";
                return new HubActionModel(actionUrl, (args, onsuccess, onerror) =>
                {
                    //DoAction(actionUrl, args);
                    try
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var service = scope.ServiceProvider.GetService(tservice);
                            if (service is null)
                                throw new Exception($"Не удалось получить ссылку на {tservice.GetTypeName()}");
                            Dictionary<string, object> objectArgs = new();
                            foreach (var kv in args)
                            {
                                objectArgs[kv.Key] = kv.Value;
                            }
                            object result = service.Invoke(methodName, objectArgs);
                            if (result is IAsyncResult ar)
                            {
                                result = ((dynamic)result).Result;
                            }
                            onsuccess(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error(ex);
                        onerror(ex.Message);
                        throw;
                    }
                })
                {
                    Attributes = methodModel.Attributes,
                    Parameters = methodModel.Parameters,
                    Label = methodModel.Label,
                    Description = methodModel.Description
                };
            }
        }

        [NotInput]
        [Label("Удалить сервис")]
        public HubActionModel RemoveServiceMethod<TService>(string methodName)
             where TService : class
        {
            var actionUrl = $"/api/services/{typeof(TService).GetTypeName()}/actions/{methodName}";
            Actions.TryRemove(actionUrl, out var action);
            return action;
        }


        [Label("Получить пункты меню навигации")]
        public List<string> GetNavigations(string url)
        {
            return Actions.Keys.Where(key => key.StartsWith(url)).ToList();
        }


        [Label("Добавить контроллер данных DbContext")]        
        public Action AddDbContext<TService>() 
            where TService : DbContextBase
        {
            var actions = this.AddServiceMethodsAll(typeof(TService));
            foreach(var kv in actions)
            {
                this.Actions[kv.Key] = kv.Value;
            }
 
            return () => {
                foreach (var kv in actions)
                {
                    this.Actions.TryRemove(kv.Key, out var p);
                }
            };
        }


        [Label("Убрать контроллер данных DbContext")]
        public Action RemoveDbContext<TService>() 
            where TService : DbContextBase
        {
            return () => { };
        }
    }
}


//using ClassModelLib.ActionsClient;

//using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
//using Console_CommonModel.CommonLib.LibModels.Console_DeliveryModule.DeliveryServices.DeliveryAppServices;
//using Console_CommonModel.CommonLib.LibModels.Console_WebModule.Model;
//using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;

//using Google.Protobuf.WellKnownTypes;

//using System.Collections.Concurrent;
//using System.Text;

//using WebApi_CommonModel.BackgroungHosting;

//using ZstdSharp.Unsafe;

//using static ClassModelLib.ActionsClient.ActionDbContext;


//namespace WebApi_CommonModel.MiddlewareComponents
//{

//    public class ServiceMethodsService
//    {
//        /// <summary>
//        /// Операции по наименованиям внешних сервисов
//        /// </summary>
//        public ConcurrentDictionary<string, HubActionModel> Actions = new ConcurrentDictionary<string, HubActionModel>();


//        public ServiceMethodsService()
//        {
//            this.AddServiceMethod<ServiceMethodsMiddleware>(nameof(ServiceMethodsService.Test1));
//        }


//        public Dictionary<string, HubActionModel> Load<TActionDbContext, TActionBuilder>()
//            where TActionDbContext : ActionDbContext
//            where TActionBuilder : class, IActionBuilder
//        {
//            Dictionary<string, HubActionModel> res = new();
//            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
//            {
//                this.InfoTime("Формирование операция сервиса", () => {
//                    var builder = typeof(TActionBuilder).New<TActionBuilder>();
//                    foreach (var serviceModel in db.ActionServiceModels.ToList())
//                    {
//                        res.AddRange<string, HubActionModel>(builder.Build(serviceModel));
//                    }
//                });
//            }
//            return res;
//        }


//        public Dictionary<string, HubActionModel> Load<TActionDbContext>(params IActionBuilder[] builders)
//            where TActionDbContext : ActionDbContext
//        {
//            Dictionary<string, HubActionModel> res = new();
//            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
//            {
//                this.Info(db.GetTypeName());
//                this.InfoTime("Формирование операция сервиса", () => {
//                    foreach (var builder in builders)
//                    {
//                        foreach (var serviceModel in db.ActionServiceModels.ToList())
//                        {
//                            res.AddRange<string, HubActionModel>(builder.Build(serviceModel));
//                        }
//                    }
//                });
//            }
//            return res;
//        }


//        public Dictionary<string, HubActionModel> LoadControls<TActionDbContext>(params IControlService<ActionServiceModel>[] controls)
//            where TActionDbContext : ActionDbContext
//        {
//            Dictionary<string, HubActionModel> res = new();
//            using (TActionDbContext db = typeof(TActionDbContext).New<TActionDbContext>())
//            {
//                this.InfoTime("Формирование операция сервиса", () => {
//                    foreach (ActionServiceModel serviceModel in db.ActionServiceModels.ToList())
//                    {
//                        foreach (var control in controls)
//                        {
//                            control.SetControl(serviceModel);
//                        }
//                    }

//                });
//            }
//            return res;
//        }

//        public int Test1(int x) => x + 1;


//        public List<string> GetRoutes() => Actions.Keys.ToList();



//        /// <summary>
//        /// Список операций
//        /// </summary>
//        public string GetActionList()
//        {
//            string result = "";
//            Actions.Keys.ToList().ForEach(k => result += "," + k);
//            return Actions.Count() > 0 ? result.Substring(1) : "Нет операций";
//        }


//        /// <summary>
//        /// Регистрация операции
//        /// </summary>
//        public void AddAction(string Name, HubActionModel model)
//        {
//            System.Console.WriteLine($"Выполняем процедуру {nameof(AddAction)}");
//            this.Actions[Name] = model;
//        }


//        /// <summary>
//        /// Выполнение операции
//        /// </summary>
//        public object DoAction(string Action, Dictionary<string, string> args)
//        {
//            System.Console.WriteLine($"Выполняем процедуру {Action}");
//            HubActionModel todo = null;
//            if (Actions.ContainsKey(Action) == false)
//            {
//                foreach (var kv in Actions)
//                {
//                    var parseResult = IsRoute(Action, kv.Key);
//                    if (parseResult.Item1)
//                    {
//                        todo = kv.Value;
//                        args.AddRange(parseResult.Item2);
//                    }
//                }
//            }
//            else
//            {
//                todo = Actions[Action];
//            }

//            if (todo is null)
//            {
//                throw new Exception($"Не найден метод соответствующий зарегистрированным маршрутам.{GetRoutes().ToJsonOnScreen()}");
//            }

//            var result = todo.Exec(args);
//            if (result is Task rt)
//            {
//                rt.Wait();
//                return ((dynamic)rt).Result;
//            }
//            return result;
//        }



//        [Description("Сравнивает url с заданным маршрутом")]
//        public (bool, Dictionary<string, string>) IsRoute(string url, string route)
//        {
//            Dictionary<string, string> values = new();
//            try
//            {
//                List<string> expressions = new List<string>();
//                while (route.IndexOf("{{") >= 0)
//                {
//                    int x2 = route.IndexOf("}}");
//                    int x1 = route.IndexOf("{{");
//                    var expression = route.Substring(x1 + 2, x2 - x1 - 2);
//                    expressions.Add(expression);

//                    if (url.Substring(0, x1).ToLower() != route.Substring(0, x1).ToLower())
//                        return (false, new());
//                    route = route.Length < (x2 + 2) ? "" : route.Substring(x2 + 2);
//                    var i = url.Substring(x1).IndexOf("/");

//                    var routeValue = url.Substring(x1, i == -1 ? (url.Length - x1) : i);
//                    if (routeValue.IndexOf("/") != -1)
//                        return (false, new());

//                    values[expression] = routeValue;
//                    url = url.Substring(x1 + routeValue.Length);
//                }
//                return (url == route, values);
//            }
//            catch (Exception ex)
//            {
//                return (false, new());
//            }

//        }

//        public void RemoveAction(string Action)
//        {
//            if (Actions.ContainsKey(Action))
//            {
//                Actions.TryRemove(Action, out var p);
//            }
//        }

//        public Dictionary<string, HubActionModel> AddServiceMethods<TService>()
//        {
//            Dictionary<string, HubActionModel> res = new();
//            var names = typeof(TService).GetOwnMethodNotStaticNames();
//            this.Debug(names);
//            foreach (var name in names)
//            {
//                var a = this.AddServiceMethod<TService>(name);
//                Actions[a.Name] = res[a.Name] = a;
//            }
//            return res;
//        }



//        public HubActionModel AddServiceMethod<TService>(string methodName)
//        {
//            using (var classDbContext = new ClassDbContext())
//            {
//                var classModel = classDbContext.CreateClassModel(typeof(TService));
//                var methodModel = classModel.Methods.FirstOrDefault(m => m.Name == methodName);
//                var actionUrl = $"/api/services/{typeof(TService).GetTypeName()}/actions/{methodName}";
//                return new HubActionModel(actionUrl, (args, onsuccess, onerror) => {
//                    DoAction(actionUrl, args);
//                });
//            }

//        }

//        public HubActionModel RemoveServiceMethod<TService>(string methodName)
//        {
//            var actionUrl = $"/api/services/{typeof(TService).GetTypeName()}/actions/{methodName}";
//            Actions.TryRemove(actionUrl, out var action);
//            return action;
//        }
//    }





//    public class ServiceMethodsWorker : IWorker
//    {
//        public Task RunWorkerAsync(CancellationToken stoppingToken)
//        {
//            throw new NotImplementedException();
//        }
//    }



//    public class ServiceMethodsContext
//    {

//    }



//    [Label("Выполнение динамические функции")]
//    public class ServiceMethodsMiddleware : AsyncContext, IMiddleware, IServiceMethodsMiddleware
//    {

//        public void OnActionRequest(ActionRequest model)
//        {
//            try
//            {
//                var request = model.MessageObject.ToJsonOnScreen().FromJson<RequestModel>();
//                var result = this.DoAction(request.url, request.args);
//                var response = new ActionResponse()
//                {
//                    RequestMessage = model,
//                    Success = true
//                };
//            }
//            catch (Exception ex)
//            {
//                this.Error(ex.Message);
//            }
//        }










//        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//        {
//            var request = await GetRequestModel(context);
//            var response = await HandleHttpRequest(request);
//            await next.Invoke(context);
//        }

//        public async Task<ActionResponse> HandleHttpRequest(RequestModel request)
//        {
//            var Token = GetToken(request);
//            bool completed = false;
//            bool success = false;
//            object result = null;
//            object error = null;
//            var serial = this.Put(new()
//            {
//                OnSuccess = response => {
//                    result = response;
//                    success = true;
//                    completed = true;
//                },
//                OnError = error => {
//                    error = error;
//                    success = false;
//                    completed = true;
//                },
//            });
//            var arequest = new ActionRequest()
//            {
//                SerialKey = serial,
//                AccessToken = Token,
//                ActionName = nameof(this.OnActionRequest),
//                MessageObject = request.ToTextDictionary()
//            };
//            OnActionRequest(arequest);

//            var started = DateTimeOffset.Now.ToUnixTimeMilliseconds();
//            while ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - started < 3000) && !completed)
//            {
//                Thread.Sleep(100);
//            }
//            return new ActionResponse()
//            {
//                RequestMessage = arequest,
//                MessageObject = result,
//                Success = success,
//                SerialKey = serial
//            };
//        }

//        private string GetToken(RequestModel request)
//        {
//            return request.headers.ContainsKey("Authorization") ?
//                request.headers["Authorization"] : null;
//        }


//        public async Task<RequestModel> GetRequestModel(HttpContext http)
//        {
//            var requestModel = new RequestModel();
//            requestModel.url = http.Request.Path.ToString();
//            requestModel.headers = http.Request.Headers.ToDictionary(
//                    h => h.Key,
//                    h => string.Join(", ", h.Value)
//            );
//            requestModel.args = http.Request.Query.ToDictionary(
//                    h => h.Key,
//                    h => string.Join(", ", h.Value)
//            );
//            requestModel.body = await ReadBody(http);
//            return requestModel;
//        }


//        public async Task<string> ReadBody(HttpContext context)
//        {
//            try
//            {
//                // Проверяем, что тело запроса можно читать
//                if (context?.Request?.Body == null || !context.Request.Body.CanRead)
//                    return string.Empty;

//                using var reader = new StreamReader(
//                    context.Request.Body,
//                    Encoding.UTF8,
//                    detectEncodingFromByteOrderMarks: false,
//                    bufferSize: 1024,
//                    leaveOpen: true
//                );

//                var body = await reader.ReadToEndAsync();

//                // Сбрасываем позицию потока для возможного повторного чтения
//                //context.Request.Body.Position = 0;


//                return body;
//            }
//            catch (Exception ex)
//            {
//                // Логируем ошибку
//                this.Error($"Ошибка чтения тела запроса: {ex.Message}");
//                return string.Empty;
//            }
//        }



//    }
//}
