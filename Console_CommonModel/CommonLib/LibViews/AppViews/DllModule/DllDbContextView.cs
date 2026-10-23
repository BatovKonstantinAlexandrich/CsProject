using System.Reflection;
using System.Text;

using Console_CommonModel.CommonLib.LibAuth.AppAuth.AuthorizationModel.ServiceModel;
using Console_CommonModel.CommonLib.LibModels.ServiceBuilderModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;

using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MultiLineModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.SplitModule;
using Console_CommonModel.CommonLib.LibViews.AppViews.TextModule;

namespace Console_CommonModel.CommonLib.LibViews.AppViews.DllModule
{
    /// <summary>
    /// Навигация по сборкам в <see cref="DllDbContext"/>: просмотр классов,
    /// добавление DLL из файла и выгрузка байтов обратно в файл.
    /// </summary>
    [Label("Хранилище сборок")]
    [Description("Просмотр и управление DLL, сохранёнными в DllDbContext.")]
    [Icon("library_books")]
    public class DllDbContextView : AppViewRoot
    {

        public DllDbContextView()
        {
            Set(CreateSelectEntityList<ServiceResource, ServiceDbContext>(
                "Сборки",
                resource =>
                {

                    var assembly = Assembly.Load(resource.Dll);
                    var classNames = assembly.GetTypes().Select(t => t.GetTypeName()).ToList();
                    var list = CreateSelectList("Классы", classNames, className =>
                    {

                        var methods = assembly.GetType(className).GetMethods().Select(m => m.Name).ToList();
                        Set(CreateSelectList("Методы", methods, methodName =>
                        {

                            var method = assembly.GetType(className).GetMethods().FirstOrDefault(m => m.Name == methodName);

                            var p = assembly.GetType(className).New();
                            p.InvokeMethodWithInput<object>(methodName,
                            result =>
                            {
                            },
                            exception =>
                            {

                            });
                        }));
                    });

                    Set(list);
                }
            ));
        }

        /*private readonly TreeControlView _tree;
        private readonly MultiLineTextView _details;
        private readonly TextView _status;
        private DllAssemblyRecord _selectedAssembly;
        private string _selectedClassName;

        public DllDbContextView()
        {
            Dock = "Fill";
            MinWidth = 720;
            MinHeight = 480;

            _tree = TreeControlView.Create("Сборки");
            _tree.Width = 280;
            _tree.MinWidth = 220;
            _tree.root.Title = "Сборки";

            _details = new MultiLineTextView
            {
                Text = "Выберите сборку в дереве слева.",
                Dock = "Fill",
                MinHeight = 320,
            };

            _status = new TextView
            {
                Text = "",
                Height = 28,
                MinHeight = 28,
                Dock = "Bottom",
            };

            Top = CreateTools(new Dictionary<string, Action>
            {
                { "Обновить", RefreshAssemblies },
                { "Добавить сборку…", AddAssemblyFromFile },
                { "Выгрузить в файл…", ExportSelectedAssembly },
                { "Удалить", DeleteSelectedAssembly },
            });

            LayoutLeft(_tree);
            LayoutBottom(_status);
            LayoutFill(_details);

            OnLoad += (_, _) => RefreshAssemblies();
        }

        [Label("Выбранная сборка")]
        [NotInput]
        public DllAssemblyRecord SelectedAssembly
        {
            get => _selectedAssembly;
            private set => _selectedAssembly = value;
        }

        public void RefreshAssemblies()
        {
            try
            {
                using var db = CreateDb();
                db.Database.EnsureCreated();

                var assemblies = db.DllAssemblies
                    .OrderByDescending(a => a.CreatedUtc)
                    .ThenBy(a => a.Name)
                    .ToList();

                foreach (var child in _tree.root.Children.ToList())
                    _tree.root.Children.Remove(child);

                foreach (var record in assemblies)
                    _tree.root.Children.Add(CreateAssemblyNode(record));

                SetStatus($"Сборок в хранилище: {assemblies.Count}");
                if (assemblies.Count == 0)
                {
                    SelectedAssembly = null;
                    _selectedClassName = null;
                    _details.Text = "Хранилище пусто. Нажмите «Добавить сборку…», чтобы загрузить DLL.";
                }
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка загрузки: " + ex.Message);
                this.Error(ex);
            }
        }

        public void AddAssemblyFromFile()
        {
            var dialogs = new FileDialogService();
            dialogs.OpenFile(
                "Выберите сборку (.dll)",
                "Assemblies (*.dll)|*.dll|All files (*.*)|*.*",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                path =>
                {
                    try
                    {
                        ImportAssemblyFile(path);
                    }
                    catch (Exception ex)
                    {
                        SetStatus("Не удалось добавить сборку: " + ex.Message);
                        this.Error(ex);
                    }
                });
        }

        public void ExportSelectedAssembly()
        {
            if (SelectedAssembly is null)
            {
                SetStatus("Сначала выберите сборку.");
                return;
            }

            var bytes = SelectedAssembly.DllBytes;
            if (bytes is null || bytes.Length == 0)
            {
                // перечитать из БД на случай урезанного объекта в дереве
                using (var db = CreateDb())
                {
                    var fresh = db.DllAssemblies.Find(SelectedAssembly.Id);
                    bytes = fresh?.DllBytes;
                }
            }

            if (bytes is null || bytes.Length == 0)
            {
                SetStatus("У выбранной сборки нет данных DLL.");
                return;
            }

            var suggested = string.IsNullOrWhiteSpace(SelectedAssembly.Name)
                ? "assembly.dll"
                : (SelectedAssembly.Name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)
                    ? SelectedAssembly.Name
                    : SelectedAssembly.Name + ".dll");

            var dialogs = new FileDialogService();
            dialogs.SaveFile(
                "Assemblies (*.dll)|*.dll|All files (*.*)|*.*",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                path =>
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(Path.GetExtension(path)))
                            path += ".dll";
                        File.WriteAllBytes(path, bytes);
                        SetStatus($"Выгружено: {path}");
                    }
                    catch (Exception ex)
                    {
                        SetStatus("Ошибка выгрузки: " + ex.Message);
                        this.Error(ex);
                    }
                });
        }

        public void DeleteSelectedAssembly()
        {
            if (SelectedAssembly is null)
            {
                SetStatus("Сначала выберите сборку.");
                return;
            }

            try
            {
                using var db = CreateDb();
                var entity = db.DllAssemblies.Find(SelectedAssembly.Id);
                if (entity is not null)
                {
                    db.DllAssemblies.Remove(entity);
                    db.SaveChanges();
                }

                SelectedAssembly = null;
                _selectedClassName = null;
                RefreshAssemblies();
                SetStatus("Сборка удалена.");
            }
            catch (Exception ex)
            {
                SetStatus("Ошибка удаления: " + ex.Message);
                this.Error(ex);
            }
        }

        private void ImportAssemblyFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                throw new FileNotFoundException("Файл не найден", path);

            var bytes = File.ReadAllBytes(path);
            var assemblyName = Path.GetFileNameWithoutExtension(path);
            var classNames = TryGetTypeNames(bytes);

            var record = new DllAssemblyRecord
            {
                Name = assemblyName,
                DllBytes = bytes,
                IncludedClassNames = string.Join(", ", classNames),
                CreatedUtc = DateTime.UtcNow,
                DbContextTypeName = classNames.FirstOrDefault(n =>
                    n.EndsWith("DbContext", StringComparison.OrdinalIgnoreCase)),
            };

            using (var db = CreateDb())
            {
                db.Database.EnsureCreated();
                db.DllAssemblies.Add(record);
                db.SaveChanges();
            }

            RefreshAssemblies();
            SelectAssembly(record.Id);
            SetStatus($"Добавлена сборка «{assemblyName}» ({classNames.Count} типов).");
        }

        private TreeViewNodeModel CreateAssemblyNode(DllAssemblyRecord record)
        {
            var title = string.IsNullOrWhiteSpace(record.Name)
                ? $"#{record.Id}"
                : record.Name;
            if (record.CreatedUtc != default)
                title += $" ({record.CreatedUtc:yyyy-MM-dd})";

            var node = new TreeViewNodeModel
            {
                Title = title,
                Item = record,
                Icon = "extension",
            };
            node.OnClick = (_, _) => ShowAssembly(record);

            foreach (var className in ResolveClassNames(record))
            {
                var classNode = new TreeViewNodeModel
                {
                    Title = className,
                    Item = className,
                    Icon = "class",
                };
                var capturedRecord = record;
                var capturedClass = className;
                classNode.OnClick = (_, _) => ShowClass(capturedRecord, capturedClass);
                node.Children.Add(classNode);
            }

            node.ContextMenu.Items.Add(new ContextView
            {
                Label = "Выгрузить в файл…",
                OnClick = (_, _) =>
                {
                    SelectedAssembly = record;
                    ExportSelectedAssembly();
                },
            });
            node.ContextMenu.Items.Add(new ContextView
            {
                Label = "Удалить",
                OnClick = (_, _) =>
                {
                    SelectedAssembly = record;
                    DeleteSelectedAssembly();
                },
            });

            return node;
        }

        private void ShowAssembly(DllAssemblyRecord record)
        {
            SelectedAssembly = record;
            _selectedClassName = null;

            var sb = new StringBuilder();
            sb.AppendLine($"Сборка: {record.Name}");
            sb.AppendLine($"Id: {record.Id}");
            sb.AppendLine($"Создана (UTC): {record.CreatedUtc:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"DbContext: {record.DbContextTypeName ?? "—"}");
            sb.AppendLine($"Extension interface: {record.ExtensionInterfaceFullName ?? "—"}");
            sb.AppendLine($"Размер: {(record.DllBytes?.Length ?? 0)} байт");
            sb.AppendLine();
            sb.AppendLine("Классы:");
            foreach (var name in ResolveClassNames(record))
                sb.AppendLine("  • " + name);

            if (!string.IsNullOrWhiteSpace(record.IncludedClassIds))
            {
                sb.AppendLine();
                sb.AppendLine("Id классов (ClassDbContext): " + record.IncludedClassIds);
            }

            _details.Text = sb.ToString();
            SetStatus($"Сборка: {record.Name}");
        }

        private void ShowClass(DllAssemblyRecord record, string className)
        {
            SelectedAssembly = record;
            _selectedClassName = className;

            var sb = new StringBuilder();
            sb.AppendLine($"Класс: {className}");
            sb.AppendLine($"Сборка: {record.Name} (Id={record.Id})");
            sb.AppendLine();

            var typeInfo = TryDescribeType(record.DllBytes, className);
            if (typeInfo is not null)
                sb.Append(typeInfo);
            else
                sb.AppendLine("Не удалось загрузить метаданные типа из байтов DLL.");

            _details.Text = sb.ToString();
            SetStatus($"Класс: {className}");
        }

        private void SelectAssembly(int id)
        {
            using var db = CreateDb();
            var record = db.DllAssemblies.Find(id);
            if (record is not null)
                ShowAssembly(record);
        }

        private static List<string> ResolveClassNames(DllAssemblyRecord record)
        {
            if (!string.IsNullOrWhiteSpace(record.IncludedClassNames))
            {
                return record.IncludedClassNames
                    .Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => s.Length > 0)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(s => s)
                    .ToList();
            }

            return TryGetTypeNames(record.DllBytes);
        }

        private static List<string> TryGetTypeNames(byte[] dllBytes)
        {
            if (dllBytes is null || dllBytes.Length == 0)
                return new List<string>();

            try
            {
                var assembly = Assembly.Load(dllBytes);
                return assembly.GetTypes()
                    .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                    .Where(t => t.Assembly == assembly)
                    .Select(t => t.FullName ?? t.Name)
                    .OrderBy(n => n)
                    .ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return (ex.Types ?? Array.Empty<Type>())
                    .Where(t => t is not null)
                    .Select(t => t.FullName ?? t.Name)
                    .OrderBy(n => n)
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        private static string TryDescribeType(byte[] dllBytes, string className)
        {
            if (dllBytes is null || dllBytes.Length == 0 || string.IsNullOrWhiteSpace(className))
                return null;

            try
            {
                var assembly = Assembly.Load(dllBytes);
                var type = assembly.GetTypes()
                    .FirstOrDefault(t =>
                        string.Equals(t.FullName, className, StringComparison.Ordinal)
                        || string.Equals(t.Name, className, StringComparison.Ordinal));

                if (type is null)
                    return null;

                var sb = new StringBuilder();
                sb.AppendLine($"Полное имя: {type.FullName}");
                sb.AppendLine($"Базовый тип: {type.BaseType?.FullName ?? "—"}");
                sb.AppendLine($"Пространство имён: {type.Namespace ?? "—"}");
                sb.AppendLine($"IsPublic: {type.IsPublic}, IsAbstract: {type.IsAbstract}, IsInterface: {type.IsInterface}");
                sb.AppendLine();
                sb.AppendLine("Свойства:");
                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                             .OrderBy(p => p.Name))
                {
                    sb.AppendLine($"  • {prop.PropertyType.Name} {prop.Name}");
                }

                sb.AppendLine();
                sb.AppendLine("Методы:");
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                             .Where(m => !m.IsSpecialName)
                             .OrderBy(m => m.Name))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    sb.AppendLine($"  • {method.ReturnType.Name} {method.Name}({args})");
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "Ошибка чтения типа: " + ex.Message;
            }
        }

        private static DllDbContext CreateDb() => new DllDbContext();

        private void SetStatus(string text)
        {
            _status.Text = text ?? "";
            this.Info(text);
        }*/
    }
}
