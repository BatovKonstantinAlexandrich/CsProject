using System;
using System.Collections.Generic;
using System.IO;
using NSwag;
using NSwag.CodeGeneration.TypeScript;
using NSwag.CodeGeneration.CSharp;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.Others;

using Microsoft.EntityFrameworkCore;

/// <summary>
/// Программа подключения служб
/// </summary>
public class ServiceBuilderProgram 
{

    public static void Run(string[] args, bool loadRegisteredServices = true)
    {
        var program = new ServiceBuilderProgram();
        if (loadRegisteredServices)
            program.Load(ref args);
        program.Start(ref args);
    }

    public void Load(ref string[] args)
    {
        using (var db = new ServiceDbContextBase<ServiceResource>())
        {
            db.Database.SetCommandTimeout(15);
            foreach (var model in db.ServiceModels.ToList())
            {
                try
                {
                    var assembly = Assembly.Load(model.Dll);
                    var name = model.Name;
                    switch(model.Type)
                    {
                        case "WSDL":    InstallWsdl(ref args, model.Schema, assembly);              break;
                        case "OpenAPI": InstallOpenApi(ref args, model.Schema, assembly); break;
                        default: 
                            throw new NotImplementedException();   
                    }
                }
                catch (Exception ex)
                {
                    this.Error($"При загрузки сервиса {model.Name} выброшено исключение: {ex}");
                }
            }
        }
    }


    public Action<string> Error = error => { };

    public void Start(ref string[] args)
    {

        string services = "";
        using(var db = new ServiceDbContextBase<ServiceResource>())
        {
            db.Database.SetCommandTimeout(15);
            services = db.ServiceModels.ToList().Select(s => s.Name).ToJsonOnScreen();
        }
        switch (InputConsole.SingleSelect("Выберите способ интеграции\n"+ services, new List<string> {
            "Использовать ServiceController для создания клиента по WSDL документу",
            "Использовать NSwag для создания клиента OpenApi",
            "Выполнить запрос",
            "Назад"  }, ref args))
        {
            case "Использовать ServiceController для создания клиента по WSDL документу":
                this.OnCreateServiceByWsdlSelected(ref args);
                Start(ref args);
                break;
            case "Использовать NSwag для создания клиента OpenApi":
                string[] toargs = null;
                this.OnCreateServiceBySwaggerJsonSelected(ref args);
                args = toargs;
                Start(ref args);
                break;            
            case "Назад":
                return;
            default:
                throw new Exception("нет обработчика");
        }

    }

    public void OnExecuteRequestSelected(ref string[] args)
    {
        
       
    }

    public void OnCreateServiceByWsdlSelected(ref string[] args)
    {
        string wsdlText = null;
        switch (InputConsole.SingleSelect("Выберите", new List<string> {
            "URL адрес",
            "Файла",
            "Выход"  }, ref args))
        {
            case "URL адрес":
                string url = InputConsole.InputUrl(ref args, "Url WSDL");
                InstallWsdl(ref args, url);
                break;
            case "Файла":
                string file = InputConsole.InputFile("File swagger.json", value => new List<string>(), ref args);
                wsdlText = System.IO.File.ReadAllText(file);
                break;
            case "Выход": this.Stop(ref args); break;
            default: throw new Exception("нет обработчика");
        }
    }


    /// <summary>
    /// Считывание WSDL
    /// </summary>    
    private static Dictionary<Type, IEnumerable<Type>> InstallWsdl(ref string[] args, string url, Assembly build)
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
        Dictionary<string, Func<Dictionary<string,object>, MethodResult<object>>> api = new();
        foreach (var kv in serviceTypes) {
            Type interfaceType = kv.Key;
            Dictionary<string, Type> names = new(kv.Value.Select(t => new KeyValuePair<string,Type>(t.GetTypeName(), t)));
            foreach(var nkv in names)
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

        
        if (InputConsole.ConfirmActionDialog("Желаете протестировать методы API", () => {

            var options = new List<string>(api.Keys);
            options.Add("Завершить тестирование");
            string selected = null;
            object result = new object();
            switch((selected = InputConsole.SingleSelect("Выберите метод", options)))
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

        }));        
        return serviceTypes;
    }

    public static Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> CreateValidator(Type serviceType, string action)
    {
        string methodName = action;
        Func<Dictionary<string, object>, IDictionary<string, IEnumerable<string>>> validate = (request) =>
        {
            var started = DateTime.Now;
            var service = serviceType.Create(new object[0]);
            var action = serviceType.GetMethod(methodName);
            var validations = action.Validate(request);
            return validations;
        };
        return validate;
    }

   
    public static Func<Dictionary<string, object>, MethodResult<object>> CreateCaller(Type serviceType, string action)
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


    

    private static Dictionary<Type, IEnumerable<Type>> InstallWsdl(ref string[] args, string url)
    {
        var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "project");
        var name = url.Substring(url.LastIndexOf("/") + 1);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        //clear dir
        try
        {
            System.IO.Directory.GetDirectories(path).ToList().ForEach(p => System.IO.Directory.Delete(p, true));
            System.IO.Directory.GetFiles(path).ToList().ForEach(System.IO.File.Delete);
        }
        catch { 
        }

        //новый проект
        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet new webapi -f net6.0 --force");

        //добавление ссылки
        ProcessStartInfo info = new ProcessStartInfo("CMD.exe", $" /C {path.Substring(0, 2)} && cd {path} && dotnet-svcutil {url}");
        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
        Console.WriteLine(process.StandardOutput.ReadToEnd());
        
        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet build");
        var dll_path = System.IO.Directory.GetFiles(path, "project.dll", SearchOption.AllDirectories).FirstOrDefault();
        var build = Assembly.LoadFile(dll_path);

        using (var db = new ServiceDbContextBase<ServiceResource>())
        {
            if(!db.ServiceModels.Any(x => x.Name == name))
            {
                db.Add(new ServiceModel()
                {
                    Name = name,
                    Schema = url,
                    Type = "WSDL",
                    Dll = System.IO.File.ReadAllBytes(dll_path)
                });
                db.SaveChanges();
            }
            
        }

        
        return InstallWsdl(ref args, url, build);
    }

    /// <summary>
    /// Замена подстрок
    /// </summary>
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
        ProcessStartInfo info = new ProcessStartInfo("CMD.exe", "/C "+command);

        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);
         
        string response = process.StandardOutput.ReadToEnd();
        string result = ReplaceAll(response, "\r", "\n");
        result = ReplaceAll(result, "\n\n", "\n");
        while (result.EndsWith("\n"))
        {
            result = result.Substring(0, result.Length - 1);
        }
 

        return result;
    }

    public static string Exec(string command)
    {
         
        ProcessStartInfo info = new ProcessStartInfo(command);

        info.RedirectStandardError = true;
        info.RedirectStandardOutput = true;
        info.UseShellExecute = false;
        System.Diagnostics.Process process = System.Diagnostics.Process.Start(info);

        string response = process.StandardOutput.ReadToEnd();
         

        return response;
    }

    public void OnCreateServiceBySwaggerJsonSelected(ref string[] args)
    {
        OpenApiDocument document = null;
 
        switch (InputConsole.SingleSelect("Выберите", new List<string> {
            "URL адрес",
            "Файла",
            "Выход"  }, ref args))
        {
            case "URL адрес":
                string url = InputConsole.InputString("Url", val => new List<string>(), ref args);
                document = InstallOpenApi(ref args, url);
                break;
            case "Файла":
                string file = InputConsole.InputFile("File swagger.json", value => new List<string>(), ref args);
                var readFile = NSwag.OpenApiDocument.FromFileAsync(file);
                document = readFile.Result;
                OnOpenApiDocumentReaded(document, file, ref args);
                break;
            case "Выход": this.Stop(ref args); break;
            default: throw new Exception("нет обработчика");
        }
        InputConsole.ConfirmContinue();
    }

    public static void InstallOpenApi(ref string[] args, string url, Assembly build)
    {
        var readUrl = NSwag.OpenApiDocument.FromUrlAsync(url);
        var document = readUrl.Result;
        /*foreach(var p in document.Paths)
        {
            foreach(var k in p.Value.Keys)
            {
                OpenApiOperation operation = p.Value[k];
                
                //operation.Responses
            }
        }*/
        foreach (var type in build.GetTypes())
        {
            if (type.Name.EndsWith("Client"))
            {
                Console.WriteLine(type.Name);
            }
        }
    }

    public static OpenApiDocument InstallOpenApi(ref string[] args, string url)
    {
        var program = new ServiceBuilderProgram();     
        OpenApiDocument document = null;
        try
        {
            if (url.IsFile())
            {
                var readFile = NSwag.OpenApiDocument.FromFileAsync(url);
                document = readFile.Result;
            }
            else if (url != null && url.ToLower().StartsWith("http"))
            {
                document = program.CreateOpenApiForUrl(ref args, url);
            }
            else
            {
                document = program.CreateOpenApiForUrl(ref args, url);
            }
        }
        catch (ArgumentException ex)
        {
            if( InputConsole.UserInteractive )                
                InputConsole.Warn($"Ресурс {url} не установлен рекомендую выполнить проверку ресурса, либо исправить конфигурайцию ", ex);
            InputConsole.ConfirmActionDialog("Выполнить переход к редактору конфигурации?",
                () => throw new NotImplementedException("Функция пока ещё не реализована"));
        }
        catch (Exception ex)
        {
            //throw new Exception("Внешние ресурсы не интегрированы", ex);
        }
        return document;

    }
    public OpenApiDocument CreateOpenApiForUrl(ref string[] args, string url)
    {
        OpenApiDocument document;
        var readUrl = NSwag.OpenApiDocument.FromUrlAsync(url);
        document = readUrl.Result;

        OnOpenApiDocumentReaded(document, url, ref args);
        return document;
    }

    private void OnOpenApiDocumentReaded(OpenApiDocument document, string url, ref string[] args)
    {
        string code = null;
        switch (InputConsole.SingleSelect("Какой генератор использовать", new List<string> {
            "TypeScript",
            //"Angular",
            //"AngularJS",
            "CSharp",
            "Выход"  }, ref args))
        {
            case "TypeScript":
                var generator = new TypeScriptClientGenerator(document, new TypeScriptClientGeneratorSettings
                {
                    ClassName = "{controller}Client",
                });
                code = generator.GenerateFile();
                break;
            case "CSharp":
                var shg = new NSwag.CodeGeneration.CSharp.CSharpClientGenerator(document, new CSharpClientGeneratorSettings() { });
                code = shg.GenerateFile();
                break;
            case "Выход": this.Stop(ref args); break;
            default: throw new Exception("нет обработчика");
        }

        args = OnSaveFileDialog(ref args, url, code);
    }

    private static string[] OnSaveFileDialog(ref string[] args, string url, string code)
    {
        InputConsole.Clear();
        string output = "";
        InputConsole.InputString("Имя файла", filename =>
        {

            var results = new List<string>();
            if (filename.IndexOf(".") == -1)
            {
                output = filename;
            }
            else
            {
                string name = filename.Substring(0, filename.LastIndexOf("."));
                string ext = filename.Substring(filename.LastIndexOf(".") + 1);
                if (name.IsEng() == false || ext.IsEng() == false)
                {
                    results.Add("Используйте латинские символы для имени файла");
                }
                output = filename;
            }
            if (System.IO.File.Exists(filename))
            {
                results.Add("Файл уже существует");
            }
            return results;
        }, ref args);

        var build = CompileCs(code, output);

        using (var db = new ServiceDbContextBase<ServiceResource>())
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
        return args;
    }

    private static byte[] CompileCs(string code, string output)
    {
        var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "project");
        string filename = Path.Combine(path, output);
        var name = filename.Substring(0, filename.LastIndexOf(".") - 1);
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        //clear dir
        System.IO.Directory.GetDirectories(path).ToList().ForEach(p => System.IO.Directory.Delete(p, true));
        System.IO.Directory.GetFiles(path).ToList().ForEach(System.IO.File.Delete);

        //новый проект
        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet new webapi -f net6.0 --force");
        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft");
        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet add package Newtonsoft.Json");

        var fileout = Path.Combine(path, filename);
        System.IO.File.WriteAllText(fileout, code);

        CmdExec($"{path.Substring(0, 2)} && cd {path} && dotnet build");
        var dll_path = System.IO.Directory.GetFiles(path, "project.dll", SearchOption.AllDirectories).FirstOrDefault();
        var build = Assembly.LoadFile(dll_path);        
        foreach (var type in build.GetTypes())
        {
            Console.WriteLine(type.GetTypeName());
        }

        Console.WriteLine($"Записано а {filename} {code.Length} байт");
        return System.IO.File.ReadAllBytes(dll_path);
    }

    public void Stop(ref string[] args)
    {
    }
}
 