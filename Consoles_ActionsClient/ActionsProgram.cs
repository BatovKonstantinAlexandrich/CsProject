using ClassModelLib.ActionsClient;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ActionModels;
using Console_CommonModel.CommonLib.LibData.AbstractionModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.Others;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using ClassModelLib.ClassServiceModel;

namespace RootConsumer.ActionsClient
{
    /// <summary>
    /// Программа настройки допустимых методов
    /// </summary>
    public class ActionsProgram
    {
        private string[] args = new string[0];
        public bool is_active { get; set; }

        public ActionsConsumer client;

        private string URL { get; }

        public List<object> services { get; } = new();
        public Dictionary<object,Action> rollback { get; } = new();

        public Action<string> OnError { get; set; } = error => { };


        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action OnConnected { 
            get 
            {
                return this.client.OnConnected;
            }
            set
            {
                this.client.OnConnected = value;
            }
        } 


        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Action<Exception> OnDisconnected {
            get
            {
                return this.client.OnDisconnected;
            }
            set
            {
                this.client.OnDisconnected = value;
            }
        }


        public ActionsProgram() : this("https://localhost:8000/hubs/ActionsHub") { }

        public ActionsProgram(string URL)
        {
            this.URL = URL;
            if(string.IsNullOrWhiteSpace(URL))
                throw new ArgumentNullException(nameof(URL));
            this.client = new ActionsConsumer(URL);
            this.client.OnPublish += method => {
                var model = new ActionModel(method.Name, pars => {
                    this.Info($"Request({method.ToJson()})");
                    var cts = new CancellationTokenSource();
                    object result = null;
                    string error = "";
                    bool success = false;
                    bool completed = false;
                    this.client.Request(method.Name, pars,
                        successResult => {
                            result = successResult;
                            completed = true;
                            success = true;
                        },
                        error => {
                            error = error;
                            completed = true;
                        });
                    int ctn = 0;
                    while (!completed && ctn < 100)
                    {
                        Thread.Sleep(100); ctn++;
                    }
                    if (!success)
                        throw new Exception(error);
                    return result;
                })
                {
                    Attributes = method.Attributes,
                    Parameters = method.Parameters,
                    Icon = method.Icon,
                    Label = method.Label,
                    Description = method.Description,
                    Type = method.Type
                };
                this.client._serviceContext.AddAction(method.Name, model);
            };
            this.client.OnUnPublish += method => {
                this.client._serviceContext.RemoveAction(method.Name);
            };
        }



        /// <summary>
        /// Событие публикации метода
        /// </summary>
        /// <param name="method"></param>
        public void OnPublish(Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodModel method)
        {
            this.Info($"OnPublish({method.ToJson()})");
            this.client.OnPublish(method);                   
        }
         

        public void OnUnPublish(Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodModel method)
        {
            this.Info($"OnUnPublish({method.ToJson()})");
            this.client.OnUnPublish(method);
            
        }

       
        public async Task StartAsync( )
        {
            Thread.Sleep(1000*2);
            this.is_active = true;            
            
            await client.Connect();

            rollback["."] = client.RegisterServiceReference(this);
            foreach (var service in services)
            {
                rollback[service]=client.RegisterServiceReference(service);
            }
            
        }

        public async Task StartAsync(IActionsClient actionsClient) 
        {
            Thread.Sleep(1000 * 2);
            this.is_active = true;

            client.OnConnected += actionsClient.OnConnected;
            client.OnDisconnected += actionsClient.OnDisconnected;
            client.OnPublish += actionsClient.OnPublish;
            client.OnUnPublish += actionsClient.OnPublish;

            await client.Connect();

            rollback["."] = client.RegisterServiceReference(this);
            foreach (var service in services)
            {
                rollback[service] = client.RegisterServiceReference(service);
            }

        }

        /// <summary>Добавить сервис после подключения к хабу (повторная публикация методов).</summary>
        public void RegisterServiceAfterConnect(object service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            services.Add(service);
            if (client != null)
                rollback[service] = client.RegisterServiceReference(service);
        }

        public void OnCreateAssemblySelected()
        {
            var args = new string[0];
            var path = InputConsole.InputFilePath("Путь к файлу dll", p => null, ref args);
            var data = System.IO.File.ReadAllBytes(path);
            var name = InputConsole.InputText("Название", p=>null, ref args);

            using (var db = new ClientClassDbContext())
            {
                db.Assemblies.Add(new()
                {
                    Name = name,
                    Dll = data
                });
                db.SaveChanges();
            }
            
            this.OnCreateAssemblySelected(data);
        }

        private void OnCreateAssemblySelected(byte[] data)
        {
            var build = Assembly.Load(data);
            var args = new string[0];
            var typeName = InputConsole.SingleSelect(
                "Типы",
                build.GetClassTypes().Select(c => c.GetTypeName()).ToList().Append("Назад"),
                ref args
            );
            if (typeName == "Назад")
                OnCreateAssemblySelected();


            using (var db = new ActionDbContext())
            {                              
                db.ActionServiceModels.Add(new()
                {
                    AssemblyDll = data,
                    TypeName = typeName
                });
                db.SaveChanges();
            }
            OnAddServiceSelected();
        }

        /// <summary>
        /// Загрузка моделей
        /// </summary>
        private static void load()
        {
            //var program = new Program();


            var t = new Task[] {
                ServiceFactory.Get().AddTypesAsync(typeof(BaseEntity).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(Program).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(LabelAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(BindableAttribute).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(UITypeEditor).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(CategoryAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(DisplayAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(IndexAttribute).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(NamedSeriesArray).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(MessageProperty).Assembly),
                //ServiceFactory.Get().AddTypesAsync(typeof(ParameterAttribute).Assembly),
                ServiceFactory.Get().AddTypesAsync(typeof(DisplayNameAttribute).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(NotMappedAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(KeyAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(System.Object).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(string).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(JsonPropertyAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(Newtonsoft.Json.JsonArrayAttribute).Assembly, true),
                ServiceFactory.Get().AddTypesAsync(typeof(System.Data.DataTable).Assembly, true),
                //ServiceFactory.Get().AddTypesAsync(typeof(System.Data.SqlClient.SqlClientPermission).Assembly, true)
            };
        }



        public static void RunActionsProgram(ref string[] args)
        {
            Console.WriteLine("Загрузка модулей");
            load();

            Console.WriteLine("Подключение к сервису");
            var program = new ActionsProgram("https://localhost:7110/hubs/ActionsHub");
            //ClassModelLibProgram.program = program;
            Task.Run(() =>
            {
                Console.WriteLine("Запускаю программу...");
                CancellationTokenSource cts = null;
                program.OnConnected += () => {
                    Console.WriteLine("Запуск консоли управления");
                    cts = new();
                    Task.Run(() =>
                    {
                        program.Load();
                        program.OnStartProgram();
                    }, cts.Token);
                };
                program.OnDisconnected += (ex) => {
                    cts.Cancel();
                };
                program.StartAsync().Wait();
                Thread.Sleep(Timeout.Infinite);
            });

            Thread.Sleep(Timeout.Infinite);



            //using (var dllDb = new DllDbContext())
            //    dllDb.Database.EnsureCreated();

            //var menuShell = args ?? Array.Empty<string>();
            //var quit = false;
            //void OpenDesigner() => RunModelDesignerLoop();
            //void OpenMicroservice() => OnSelectMicroservice(ref menuShell);
            //while (!quit)
            //{
            //    InputConsole.SelectAction(
            //        "Выберите действие",
            //        new Dictionary<string, Action>()
            //        {
            //            { "Редактор ClassModel и сборки DLL", OpenDesigner },
            //            { "Подключить микросервис", OpenMicroservice },
            //            { "Выход", () => { quit = true; } },
            //        },
            //        ref menuShell);
            //}
            //args = menuShell;
        }
         
        private static void ReinitDatabases()
        {
            using (var db = new ActionDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            Console.WriteLine("Формирую базу данных...");
            using (var db = new ClientClassDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        public void Load()
        {
            this.services.Clear();
            using (var db = new ActionDbContext())
            {
                foreach(var p in db.ActionServiceModels.ToList())
                {
                    try
                    {
                        this.Info($"Загружаю сборку {p.AssemblyDll.Length} байт");
                        var assembly = Assembly.Load(p.AssemblyDll);
                        ServiceFactory.Get().AddTypes(assembly, true);
                        var type = assembly.GetTypes().First(t => t.GetTypeName() == p.TypeName);
                        var service = AppProviderService.GetInstance().GetService(type);
                        services.Add(service);
                        if (client is not null && client.IsConnected().Result)
                        {
                            rollback[service] = client.RegisterServiceReference(service);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Info(ex);
                    }
                }
            }
        }

        private void OnTestServiceSelected()
        {
            var todos = new Dictionary<string, Action>();
            foreach (var p in this.services)
            {
                var temp = p;
                todos[p.GetTypeName()] = () => {
                    this.OnServiceSelectedForTest(temp);
                };
            }
            todos["Назад"] = OnStartProgram;
            InputConsole.SelectAction("Выберите сервис", todos, ref args);
        }

        private void OnServiceSelectedForTest(object service)
        {
            var methods = service.GetType().GetMethods().Where(m => m.DeclaringType == service.GetType() && !m.IsStatic && !m.IsAbstract).Select(m => m.Name);
            var classDbContext = new ClientClassDbContextService(new WinFormsApp.Console.Console_ClassModule.ClassServiceModel.ClientClassDbContext());
            var classModel = classDbContext.CreateAndSaveClassModel(service.GetType());
            this.Info($"{service.GetTypeName()} => {JsonConvert.SerializeObject(methods)}");
            var todos = new Dictionary<string, Action>();
            foreach (var method in classModel.Methods)
            {
                this.Info(method.Label);
                Func<Dictionary<string,string>,object> todo = parameters =>
                {
                    try
                    {
                        var methodInfo = service.GetType().GetMethod(method.Name);
                        var parameterNames = methodInfo.GetParameters().Select(p => p.Name).ToList();
                        List<object> args = new();
                        foreach (var parameter in parameterNames)
                        {
                            var par = methodInfo.GetParameters().FirstOrDefault(p => p.Name == parameter);

                            if (parameters.ContainsKey(parameter))
                            {
                                var textValue = parameters[parameter];
                                if (par.ParameterType == typeof(int))
                                {
                                    args.Add(int.Parse(textValue));
                                }
                                else if (par.ParameterType == typeof(DateTime))
                                {
                                    args.Add(DateTime.Parse(textValue));
                                }
                                else if (par.ParameterType == typeof(float))
                                {
                                    args.Add(float.Parse(textValue));
                                }
                                else
                                {
                                    args.Add(textValue);
                                }

                            }
                            else
                            {
                                args.Add(null);
                            }
                        }
                        var result = service.GetType().GetMethod(method.Name).Invoke(service, args.ToArray());
                        InputConsole.ConfirmContinue($"Результат: {(result is null? "": result.ToJson())}");
                        return result;
                    }
                    catch (Exception ex)
                    {
                        this.OnError($"Исключение при регистрации сервисных операций типа {service.GetTypeName()}: {ex.Message} ");
                        throw;
                    }
                };
                todos[method.Name] = () => {

                    try
                    {
                        Dictionary<string, string> pars = new();
                        var parameterNames = method.Parameters.Select(p => p.Name).ToList();
                        foreach (var par in parameterNames)
                        {
                            var args = new string[0];
                            pars[par] = InputConsole.InputString(par, p => null, ref args);
                        }
                        var result = todo(pars);
                        Console.WriteLine(result.ToJson());
                    }
                    catch(Exception ex)
                    {
                        this.Error($"Ошибка при выполнении метода {method.Name} => {ex.Message}");
                        InputConsole.ConfirmContinue($"Ошибка при выполнении метода {method.Name} => {ex.Message}");
                    }
                    finally
                    {
                        OnServiceSelectedForTest(service);
                    }
                    
                    
                    //if (InputConsole.Confirm("Вернуться к тестированию"))
                    //{
                    //    OnTestServiceSelected();
                    //}
                    //else
                    //{
                    //    OnStartProgram();
                    //}
                };
            }
            //var clientActions = client.ListClientActions().Result;
            todos["Назад"] = OnTestServiceSelected;
            InputConsole.SelectAction("Выберите метод",todos, ref args);
        }

        private void OnRemoveServiceSelected()
        {
            var todos = new Dictionary<string, Action>();
            foreach (var p in this.services)
            {
                var temp = p;
                todos[p.GetTypeName()] = () => {

                    this.services.Remove(temp);
                    using (var db = new ActionDbContext())
                    {
                        db.Remove(db.ActionServiceModels.FirstOrDefault(p => p.TypeName==temp.GetTypeName()));
                        db.SaveChanges();
                    }
                    this.rollback[temp]();
                    //this.client.UnPublish();
                    this.OnStartProgram();
                };
            }
            todos["Назад"] = this.OnStartProgram;
            InputConsole.SelectAction("Выберите сервис чтобы удалить",todos, ref args);
        }

        private void OnAddServiceSelected()
        {
            Dictionary<string, Action> todos = new();
            todos["Загрузить сборку"] = () => {
                OnCreateAssemblySelected();
            };
            todos["Собрать из классов"] = () => {
                OnCompileClassModelSelected();
            };
            using (var db = new ClientClassDbContext())
            {
                foreach(var dll in db.Assemblies)
                {
                    Action todo = () => {
                        
                        this.OnAddTypeFromAssembly(dll.Dll);
           
                    };                    
                    todos.Add(dll.Name, todo);
                } 
            }

            todos["Назад"] = () => {
                OnStartProgram();
            };
            InputConsole.SelectAction("Выберите сборку", todos, ref args);            
        }

        private void OnCompileClassModelSelected()
        {
            using (var db = new WinFormsApp.Console.Console_ClassModule.ClassServiceModel.ClientClassDbContext())
            {
                var service = new ClientClassDbContextService(db);
                var args = new string[0];
                var selected = InputConsole.MultiSelect("Выбери классы контекста данных", db.Classes.Select(c => c.Name).ToList(), ref args);
                var compiler = new DbContextCompiler();
                var name = InputConsole.InputText("Название сборки",p=>null,ref args);
                var data = compiler.CompileDll(name, db.Classes.Where(c=> selected.Contains(c.Name)));
                using (var dlldb = new ClientClassDbContext())
                {
                    dlldb.Assemblies.Add(new()
                    {
                        Name = name,
                        Dll = data
                    });
                    dlldb.SaveChanges();
                }
                this.OnCreateAssemblySelected(data);
            }
                
        }

     

        public void OnStartProgram()
        {            
            var s=($"API сервисы ");
            s += "  [" + client.Connection.State + "]\n";

            foreach (var p in this.services)
            {
                s+=("  " + p.GetTypeName())+"\n";
            }
            var args = new string[0];

            s += "Выберите действие";
            InputConsole.SelectAction(s, new() {
                    { "Авторизация", ()=>{
                        this.OnLoginSelected();
                    } },
                    { "Добавить сервис", ()=>{
                        this.OnAddServiceSelected();
                    } },
                    { "Удалить сервис", ()=>{
                        this.OnRemoveServiceSelected();
                    } },
                    { "Тестирование", ()=>{
                        this.OnTestServiceSelected();
                    } },
                    { "Сервисы", ()=>{
                        this.OnListServiceSelected();
                    } },
                    { "Сброс", ()=>{
                        this.OnDropConfigrationSelect();
                    } },
                    { "Выход", ()=>{
                        Process.GetCurrentProcess().Kill();
                    } },
                },ref args);
        }

        private void OnLoginSelected()
        {
            var args = new string[0];
            var serviceName = InputConsole.InputText("Название сервиса",p=>null,ref args);
            this.Login(serviceName);
            this.OnStartProgram();
        }

        private void OnDropConfigrationSelect()
        {
            using(var db = new ClientClassDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
            InputConsole.ConfirmContinue("Сброс выполнен");

            OnStartProgram();
        }

        private void OnListServiceSelected()
        {
            var args = new string[0];
            try
            {
                if (client is null)
                {
                    this.StartAsync().Wait();
                }

                var actions = client.ListClientActions().Result;
                Dictionary<string, Action> todos = new();
                actions.ToList().ForEach(p => todos[p.Key] = () => {

                    Dictionary<string, Action> _todos = new();
                    foreach (var kv in p.Value)
                    {
                        _todos[kv.Name] = () => {

                            this.OnServiceMethodSelected(kv);
                        };
                    }
                    _todos["Назад"] = () => {
                        OnListServiceSelected();
                    };
                    InputConsole.SelectAction("Выберите метод", _todos, ref args);

                });
                todos["Назад"] = () => {
                    OnStartProgram();
                };
                InputConsole.SelectAction("Выберите сервис", todos, ref args);
            }
            catch(Exception ex)
            {
                OnStartProgram();
            }
        }

        private void OnServiceMethodSelected(Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel.MethodModel method)
        {
            InputConsole.Clear();
            InputConsole.ConfirmContinue(method.ToJson());

            OnListServiceSelected();
        }

        private void OnAddTypeFromAssembly(byte[] data)
        {
            Assembly dll = Assembly.Load(data);
            var args = new string[0];

            Dictionary<string, Action> todos = new();
            foreach(var p in dll.GetTypes().Where(t => t.GetTypeName().IsEng()))
            {
                todos.Add(p.GetTypeName(), () => {
                    try
                    {
                        using (var db = new ActionDbContext())
                        {
                            db.ActionServiceModels.Add(new()
                            {
                                TypeName = p.GetTypeName(),
                                AssemblyDll = data
                            });
                            db.SaveChanges();
                        }
                        var serviceRef = p.New<object>();
                        this.services.Add(serviceRef);
                        this.rollback[serviceRef] = client.RegisterServiceReference(serviceRef);
                        this.OnStartProgram();
                    }
                    catch(Exception  ex)
                    {
                        Console.WriteLine(ex);   
                        this.OnError(ex.Message);
                    }                    
                });
            }
            InputConsole.SelectAction("Выберите тип", todos,ref args);
             
        }

        public void Login(string serviceName)
        {
            try
            {               
                this.client.Token = this.client.Connection.InvokeAsync<string>("Login", serviceName).Result;                                
            }
            catch (Exception ex)
            {
                this.Info(ex);
            }
        }
    }




}
