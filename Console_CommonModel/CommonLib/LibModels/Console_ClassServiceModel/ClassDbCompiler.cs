using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;


namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{


    /// <summary>
    /// Формирует dll из ClassModels
    /// </summary>
    public class ClassDbCompiler
    {


        [Label("Создать исходный код")]
        public Dictionary<string, string> CreateSources(List<ClassModel> classModels)
        {
            return Timed(() =>
            {
                Dictionary<string, string> results = new();
                foreach (var classModel in classModels)
                {
                    results[classModel.Name] = ToSource(classModel);
                }
                return results;
            });
        }


        public string CreateClassModelCs(ClassModel classModel)
        {
            return Timed(() =>
            {
                if (classModel.Name == "DictionaryTable")
                {
                    int x = 1;
                    this.Info(x);
                }
                string cs = "";

                List<string> deps = new() { "System", "System.ComponentModel","pickpoint_delivery_service","Microsoft.EntityFrameworkCore","System.Collections.Generic", "System.Linq", "System.ComponentModel.DataAnnotations.Schema",
                    "Console_CommonModel.CommonLib.LibViews.ViewEvents.List",
                    "System.ComponentModel.DataAnnotations",
                    "System.Text.Json.Serialization",
                    "System.Collections.Concurrent",
                    "System.Xml.Serialization"};
                foreach (var dep in deps)
                {
                    cs += $"using {dep};\n";
                }
                cs += $"\n\n";
                foreach (var attr in classModel.Attributes)
                {
                    if (!new List<string>() {
                        "ParamArrayAttribute",
                        "NullableContextAttribute",
                        "DebuggerStepThroughAttribute",
                        "AsyncStateMachineAttribute",
                        "NullableContext",
                        "DebuggerStepThrough",
                        "AsyncStateMachine"
                    }.Contains(attr.Name))
                        cs += $"[{attr.Name}(\"{attr.Value}\")]\n";
                }
                string suffix = "";
                using (var db = new ClassDbContext())
                {
                    if (classModel.ParentId is not null)
                    {
                        suffix += " : " + db.Classes.Find(classModel.ParentId).Name + " ";
                    }
                }
                cs += $"public class {classModel.Name} {suffix} {{\n";
                foreach (var prop in classModel.Properties)
                {
                    cs += $"\n";
                    foreach (var attr in prop.Attributes)
                    {
                        if (!new List<string>() {
                            "RangeAttribute",
                            "RequiredAttribute",
                            "ParamArrayAttribute",
                            "NullableContextAttribute",
                            "DebuggerStepThroughAttribute",
                            "AsyncStateMachineAttribute",
                            "NullableContext",
                            "DebuggerStepThrough",
                            "AsyncStateMachine"
                        }.Contains(attr.Name))
                        {
                            if (new List<string>() {
                                nameof(NotMappedAttribute),
                                nameof(KeyAttribute),
                                nameof(XmlIgnoreAttribute),
                                nameof(JsonIgnoreAttribute)
                            }.Contains(attr.Name))
                            {
                                cs += $"  [{attr.Name}()]\n";
                            }
                            else
                            {
                                cs += $"  [{attr.Name}(\"{attr.Value}\")]\n";
                            }

                        }

                    }
                    cs += $"  public {prop.Type} {prop.Name} {{ get; set; }}\n";
                }
                foreach (var method in classModel.Methods)
                {
                    cs += $"\n\n";
                    foreach (var attr in method.Attributes)
                    {
                        if (!new List<string>() {
                            "RangeAttribute",
                            "RequiredAttribute",
                            "ParamArrayAttribute",
                            "NullableContextAttribute",
                            "DebuggerStepThroughAttribute",
                            "AsyncStateMachineAttribute",
                            "NullableContext",
                            "DebuggerStepThrough",
                            "AsyncStateMachine"
                        }.Contains(attr.Name))
                            cs += $"  [{attr.Name}(\"{attr.Value}\")]\n";
                    }
                    cs += $"  public void {method.Name}(\n";
                    var ctn = method.Parameters.Count();
                    int i = 1;
                    foreach (var methodPar in method.Parameters)
                    {
                        foreach (var attr in methodPar.Attributes)
                        {
                            if (!new List<string>() {
                                "RangeAttribute",
                                "RequiredAttribute",
                                "ParamArrayAttribute",
                                "NullableContextAttribute",
                                "DebuggerStepThroughAttribute",
                                "AsyncStateMachineAttribute",
                                "NullableContext",
                                "DebuggerStepThrough",
                                "AsyncStateMachine"
                            }.Contains(attr.Name))
                                cs += $"    [{attr.Name}(\"{attr.Value}\")] )\n";
                        }
                        if (i == ctn)
                        {
                            cs += $"    {methodPar.Type} {methodPar.Name}\n";
                        }
                        else
                        {
                            cs += $"    {methodPar.Type} {methodPar.Name},\n";
                        }
                        i++;
                    }
                    cs += "  ){ \n";
                    cs += "" + FormatMethodBody(method.Body) + "\n  }\n";

                }
                cs += "}" + "\n";
                return cs;
            });
        }
        private string FormatMethodBody(string body)
        {
            return Timed(() =>
            {
                string code = "";
                if (body is not null)
                {
                    foreach (var line in body.Split("\r").Select(l => l.Trim()).Select(l => l.Replace("  ", " ")))
                    {
                        code += "      " + line + "\r";
                    }
                }

                return code;
            });
        }
        public string ToSource(ClassModel classModel)
        {
            return Timed(() =>
            {
                string cs = "";
                var usings = new HashSet<string>(StringComparer.Ordinal) { "System" };
                AppendBaseTypeForClass(classModel, usings, out var baseClause);
                foreach (var dep in usings.OrderBy(s => s))
                    cs += $"using {dep};\n";
                cs += "\n";
                foreach (var attr in classModel.Attributes ?? Enumerable.Empty<AttributeBase<ClassModel>>())
                {
                    cs += $"[{attr.Name}(\"{attr.Value}\")]\n";
                }
                cs += $"public class {classModel.Name}{baseClause}{{\n";
                foreach (var prop in classModel.Properties ?? Enumerable.Empty<PropertyModel>())
                {
                    cs += $"\n";
                    foreach (var attr in prop.Attributes ?? Enumerable.Empty<AttributeBase<PropertyModel>>())
                    {
                        cs += $"  [{attr.Name}(\"{attr.Value}\")]\n";
                    }
                    cs += $"  public {prop.Type} {prop.Name} {{ get; set; }}\n";
                }
                foreach (var method in classModel.Methods ?? Enumerable.Empty<MethodModel>())
                {
                    cs += "\n";
                    foreach (var attr in method.Attributes ?? Enumerable.Empty<AttributeBase<MethodModel>>())
                    {
                        cs += $"  [{attr.Name}(\"{attr.Value}\")]\n";
                    }
                    var retType = string.IsNullOrWhiteSpace(method.Type) ? "void" : method.Type;
                    cs += $"  public {retType} {method.Name}(";
                    var pars = method.Parameters ?? new List<MethodParameterModel>();
                    var parts = new List<string>();
                    foreach (var methodPar in pars)
                    {
                        var parAttrs = methodPar.Attributes ?? Enumerable.Empty<AttributeBase<MethodParameterModel>>();
                        var attrPrefix = string.Join("", parAttrs.Select(a => $"[{a.Name}(\"{a.Value}\")] "));
                        parts.Add($"{attrPrefix}{methodPar.Type} {methodPar.Name}");
                    }
                    cs += string.Join(", ", parts);
                    cs += ") { }\n";
                }
                cs += $"}}\n";
                return cs;
            });
        }

        private void AppendBaseTypeForClass(ClassModel classModel, HashSet<string> usings, out string baseClause)
        {
            string clause = "";
            Timed(() =>
            {
                if (!string.IsNullOrWhiteSpace(classModel.ExtendsTypeName))
                {
                    var full = classModel.ExtendsTypeName.Trim();
                    var lastDot = full.LastIndexOf('.');
                    if (lastDot > 0)
                    {
                        var ns = full.Substring(0, lastDot);
                        var shortName = full.Substring(lastDot + 1);
                        usings.Add(ns);
                        clause = " : " + shortName;
                    }
                    else
                    {
                        clause = " : global::" + full;
                    }
                    return;
                }
                if (classModel.ParentId is int pid && classModel.Parent != null && !string.IsNullOrWhiteSpace(classModel.Parent.Name))
                {
                    clause = " : " + classModel.Parent.Name;
                }
            });
            baseClause = clause;
        }

        public Type CreateDbContextType(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                var models = BuildCompileSources(dbContextName, classes);
                Assembly assembly = Compile(dbContextName, models);
                Type dbContextType = assembly.DefinedTypes.FirstOrDefault(t => t.Name == dbContextName);
                return dbContextType;
            });
        }


        public Assembly CreateDbContextAssembly(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                var models = BuildCompileSources(dbContextName, classes);
                Assembly assembly = Compile(dbContextName, models);
                return assembly;
            });
        }

        public byte[] CompileDll(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {

                var models = BuildCompileSources(dbContextName, classes);
                return CompileDll(dbContextName, models);
            });
        }

        /// <summary>Дополнительные .cs (например partial с делегированием интерфейса расширения).</summary>
        public byte[] CompileDll(string dbContextName, IEnumerable<ClassModel> classes, IDictionary<string, string> additionalSources)
        {
            return Timed(() =>
            {
                var models = BuildCompileSources(dbContextName, classes);
                if (additionalSources != null)
                {
                    foreach (var kv in additionalSources)
                        models[kv.Key] = kv.Value;
                }
                return CompileDll(dbContextName, models);
            });
        }
        public byte[] CreateAssembly(string dbContextTypeName, List<ClassModel> classModels)
        {
            return Timed(() => CompileDll(dbContextTypeName, classModels));
        }

        public byte[] CreateAssembly(string dbContextTypeName, List<ClassModel> classModels, Dictionary<string, string> additionalSources)
        {
            return Timed(() =>
            {
                if (additionalSources == null || additionalSources.Count == 0)
                    return CreateAssembly(dbContextTypeName, classModels);
                return CompileDll(dbContextTypeName, classModels, additionalSources);
            });
        }

        public Assembly Compile(string dir, Dictionary<string, string> models)
        {
            return Timed(() =>
            {
                string dll = CreateDll(dir, models);
                var res = Assembly.LoadFile(dll);
                return res;
            });
        }

        public byte[] CompileDll(string dir, Dictionary<string, string> models)
        {
            return Timed(() =>
            {
                string dll = CreateDll(dir, models);
                return File.ReadAllBytes(dll);
            });
        }

        public string CreateDll(string workName, Dictionary<string, string> models)
        {
            return Timed(() =>
            {
                var root = Path.Combine(Path.GetTempPath(), "Console_CommonModel.CommonLib.OthersCompile", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(root);
                const string projName = "DynEmit";
                var projPath = Path.Combine(root, projName + ".csproj");
                var hostDll = typeof(ClassDbContext).Assembly.Location;
                if (string.IsNullOrEmpty(hostDll) || !File.Exists(hostDll))
                    throw new InvalidOperationException("Не удалось определить путь к сборке Console_CommonModel.CommonLib.Others для ссылки временного проекта.");
                var hintXml = WebUtility.HtmlEncode(hostDll);
                File.WriteAllText(projPath, @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>net8.0-windows</TargetFramework>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <RootNamespace>Generated</RootNamespace>
    <AssemblyName>" + projName + @"</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
<PackageReference Include=""Microsoft.Data.SqlClient"" Version=""5.2.2"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore"" Version=""8.0.11"" />
    <PackageReference Include=""Microsoft.EntityFrameworkCore.SqlServer"" Version=""8.0.11"" />
  </ItemGroup>  
  <ItemGroup>
    <ProjectReference Include=""C:\Projects\CursorApp\WinFormsApp.csproj"" />
  </ItemGroup>
  <ItemGroup>
    <Reference Include=""Console_CommonModel.CommonLib.OthersHost"">
      <HintPath>" + hintXml + @"</HintPath>
    </Reference>
  </ItemGroup>
</Project>
");
                foreach (var kv in models)
                {
                    var safeName = string.Join("_", (kv.Key ?? "Type").Split(Path.GetInvalidFileNameChars()));
                    File.WriteAllText(Path.Combine(root, safeName + ".cs"), kv.Value ?? "");
                }




                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{projPath}\" -c Release --nologo -v q",
                    WorkingDirectory = root,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                using (var proc = Process.Start(psi) ?? throw new InvalidOperationException("Не удалось запустить dotnet."))
                {
                    //proc.WaitForExit(300000);
                    var err = "";// proc.StandardError.ReadToEnd();
                    var outp = proc.StandardOutput.ReadToEnd();
                    this.Info($"dotnet build \"{projPath}\" -c Release --nologo -v q");
                    if (proc.ExitCode != 0)
                        throw new Exception($"Сборка временного проекта не удалась (код {proc.ExitCode}).\n{err}\n{outp}");
                }


                var dllPath = Path.Combine(root, "bin", "Release", "net8.0", projName + ".dll");
                if (!File.Exists(dllPath))
                    throw new Exception($"Не найден результат сборки: {dllPath}");
                return dllPath;
            });
        }

        public void CopyCsFiles(string source, string target)
        {
            Timed(() =>
            {
                var allfiles = Directory.GetFiles(source, "*.cs", SearchOption.AllDirectories);
                foreach (var file in allfiles)
                {
                    var from = file;
                    var to = file.Replace(source, target);
                    var path = to.Substring(target.Length).Split("\\").ToList();
                    path.RemoveAt(path.Count() - 1);
                    string pdir = target;
                    foreach (string name in path)
                    {
                        pdir = Path.Combine(pdir, name);
                        if (!Directory.Exists(pdir))
                        {
                            Directory.CreateDirectory(pdir);
                        }
                    }
                    File.Copy(from, to, true);
                }
            });
        }

        /// <summary>Исходник DbContext с DbSet для выбранных сущностей (без записи в ClientClassDbContext).</summary>
        public string CreateDbContextClassCs(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                var sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using Microsoft.EntityFrameworkCore;");
                sb.AppendLine();
                sb.AppendLine($"public partial class {dbContextName} : DbContext");
                sb.AppendLine("{");
                sb.AppendLine($"  public {dbContextName}() {{ }}");
                foreach (var c in classes ?? Enumerable.Empty<ClassModel>())
                {
                    if (c == null || string.IsNullOrWhiteSpace(c.Name))
                        continue;
                    var setName = c.Name;
                    setName = setName.Replace("<", "").Replace(">", "");
                    setName = setName.ToMultiCount();
                    sb.AppendLine($"  public DbSet<{c.Name}> {setName} {{ get; set; }}");

                }
                sb.AppendLine($@"     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                    {{
                        optionsBuilder.UseSqlServer(GetConnectionString());
                    }}
public virtual string GetConnectionString()
        {{
            var builder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder();
            builder.DataSource = @""DESKTOP-IHJM9RD"";
            var databaseName = this.GetType().Name;
            //this.Info(databaseName);
            builder.InitialCatalog = databaseName;
            builder.TrustServerCertificate = true;
            builder.IntegratedSecurity= true;
            return builder.ToString();
        }}
            ");
                sb.AppendLine("}");
                return sb.ToString();
            });
        }

        public Dictionary<string, string> CreateClassModelsCs(IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                Dictionary<string, string> models = new();
                foreach (var classModel in classes)
                {
                    try
                    {
                        if (classModel is not null)
                        {
                            this.Info(classModel.Name);
                            var ptype = classModel.Name.ToType();
                            if (ptype.IsGenericType && !ptype.IsGenericTypeDefinition)
                                continue;
                            models[classModel.Name] = CreateClassModelCs(classModel);
                        }

                    }
                    catch (Exception ex)
                    {
                        this.Error($"{classModel.Name}" + ex.ToString());
                    }
                }
                return models;
            });
        }


        public string CreateClassModelCs(int classId)
        {
            return Timed(() =>
            {
                using (var db = new ClassDbContext())
                {
                    var classModel = db.Classes
                        .Include(c => c.Parent)
                        .Include(c => c.Attributes)
                        .Include(c => c.Properties)
                        .ThenInclude(p => p.Attributes)
                        .Include(c => c.Methods)
                        .ThenInclude(p => p.Attributes)
                        .Include(p => p.Methods)
                        .ThenInclude(p => p.Parameters)
                        .ThenInclude(p => p.Attributes)
                        .FirstOrDefault(c => c.Id == classId);
                    return CreateClassModelCs(classModel);
                }
            });
        }

        public ClassModel CreateDbContextModel(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                ClassModel result = null;
                using (var db = new ClassDbContext())
                {
                    db.Add(result = new ClassModel()
                    {
                        Name = dbContextName
                    });
                    db.SaveChanges();
                    var modelId = result.Id;
                    if (classes is not null)
                    {
                        foreach (var classModel in classes)
                        {
                            db.Add(new PropertyModel()
                            {
                                ParentId = modelId,
                                Type = $"DbSet<{classModel.Name}>",
                                Name = classModel.Name.ToMultiCount(),
                                Label = classModel.Name.ToMultiCount(),
                                Description = classModel.Name.ToMultiCount(),
                            });
                            db.SaveChanges();
                        }
                    }

                }
                return result;
            });
        }

        private Dictionary<string, string> CreateDbContext(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                var models = CreateClassModelsCs(classes);
                var dbcontext = CreateDbContextModel(dbContextName, classes);
                var dbcontextCs = CreateClassModelCs(dbcontext.Id);
                models[dbcontext.Name] = dbcontextCs;
                return models;
            });
        }

        private IEnumerable<ClassModel> GetExtendedClasses(ClassModel classModel)
        {
            return Timed(() =>
            {
                try
                {
                    List<ClassModel> results = new();
                    if (classModel.ParentId is not null)
                    {
                        if (classModel.ParentId is not null)
                        {
                            var parentClassModel = GetClassModelById((int)classModel.ParentId);
                            results.AddRange(GetExtendedClasses(parentClassModel));
                        }
                    }
                    results.Add(classModel);
                    return results;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Не удалось собрать родительские классы для {classModel.Name}");
                }
            });
        }

        public ClassModel GetClassModelById(int id)
        {
            return Timed(() =>
            {
                try
                {
                    using (var db = new ClassDbContext())
                    {
                        var classModel = db.Classes
                            .Include(c => c.Parent)
                            .Include(c => c.Attributes)
                            .Include(c => c.Properties)
                            .ThenInclude(p => p.Attributes)
                            .Include(c => c.Methods)
                            .ThenInclude(p => p.Attributes)
                            .Include(p => p.Methods)
                            .ThenInclude(p => p.Parameters)
                            .ThenInclude(p => p.Attributes)
                            .FirstOrDefault(c => c.Id == id);
                        return classModel;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при выполнении метода GetClassModelById({id}) => {ex.Message}");
                }
            });
        }

        public ClassModel GetClassModelById(string name)
        {
            return Timed(() =>
            {
                try
                {
                    using (var db = new ClassDbContext())
                    {
                        var classModel = db.Classes
                            .Include(c => c.Parent)
                            .Include(c => c.Attributes)
                            .Include(c => c.Properties)
                            .ThenInclude(p => p.Attributes)
                            .Include(c => c.Methods)
                            .ThenInclude(p => p.Attributes)
                            .Include(p => p.Methods)
                            .ThenInclude(p => p.Parameters)
                            .ThenInclude(p => p.Attributes)
                            .FirstOrDefault(c => c.Name == name);
                        return classModel;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при выполнении метода GetClassModelById({name}) => {ex.Message}");
                }
            });
        }


        private Dictionary<string, string> BuildCompileSources(string dbContextName, IEnumerable<ClassModel> classes)
        {
            return Timed(() =>
            {
                List<ClassModel> extended = new();
                foreach (var classModel in classes)
                {
                    var extClases = GetExtendedClasses(classModel);

                    foreach (var next in extClases)
                    {
                        try
                        {
                            this.Info(next.Name);
                            var ptype = next.Name.ToType();
                            if (ptype.IsGenericType)
                            {
                                if (ptype.IsGenericTypeDefinition)
                                {
                                    extended.Add(next);
                                }
                                else
                                {
                                    var td = ptype.GetGenericTypeDefinition().GetTypeName();
                                    var tdClassModel = GetClassModelById(td);
                                    extended.Add(tdClassModel);
                                }
                            }
                            else
                            {
                                extended.Add(next);
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Error($"Ошибка при обработке {next} => {ex}");
                        }
                    }
                }
                extended = extended.ToHashSet().ToList();
                this.Info(extended.Select(c => c.Name).ToJsonOnScreen());
                var models = CreateClassModelsCs(extended);
                models[dbContextName] = CreateDbContextClassCs(dbContextName, classes);
                return models;
            });
        }



        private T Timed<T>(Func<T> work, [CallerMemberName] string method = null)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return work();
            }
            finally
            {
                sw.Stop();
                this.Info($"{GetType().Name}.{method}: {sw.Elapsed.TotalMilliseconds:0.###} мс");
            }
        }

        private void Timed(Action work, [CallerMemberName] string method = null)
        {
            Timed<object>(() => { work(); return null; }, method);
        }

    }
}
