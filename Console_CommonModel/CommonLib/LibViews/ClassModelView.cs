using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;

using Microsoft.EntityFrameworkCore;

using System.Reflection;
using Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel;
using Console_CommonModel.CommonLib.LibViews.AppViews.BaseComponent;
using Console_CommonModel.CommonLib.LibViews.AppViews.EditModule;
using Console_CommonModel.CommonLib.LibViews.AppViewModules.MenuModule;

[Label("Представление настройки элементов")]
public class ElementsCustomizerView : AppViewBase
{
    private readonly TreeControlView classTree;
    private readonly AppViewBase contentPane;

    public ElementsCustomizerView()
    {
        var factory = new TreeFactory();
        this.classTree = factory.CreateClassTree(typeof(AppViewBase), this.OnElementSelected);
        this.LayoutFill(this.contentPane = new AppViewBase());
        this.Left = classTree;
    }

    private void OnElementSelected(Type type)
    {
        this.contentPane.LayoutFill(type.New<AppViewBase>());
    }
}

[Label("Редактор типа")]
public class ClassModelView : AppViewBase
{
    private readonly ClassModelBuilder classModelBuilder;
    private readonly EditView editForm;
    private Assembly compiledAssembly;

    [Label("Selected class")]
    [InputSelectExpression(nameof(ClassNames))]
    public string SelectedClassName { get; set; }

    
    [Label("DbContext name")]
    public string DbContextName { get; set; } = "GeneratedDbContext";

    
    [Label("Project name")]
    public string BuildProjectName { get; set; } = "GeneratedDbContextApp";

    
    [Label("Last build path")]
    public string LastCompiledAssemblyPath { get; set; }

    
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public List<string> ClassNames => GetClassNames();

    public ClassModelView() : this(new ClassModel()) { }
    public ClassModelView(ClassModel classModel)
    {
        this.classModelBuilder = new ClassModelBuilder();
        EnsureClassModelInitialized(classModelBuilder.classModel= classModel);
        var props = classModel.GetType().GetInputPropertiesSorted();
        this.editForm = EditView.CreateEditView(classModel, props);
        this.LayoutFill(this.editForm);
        this.Top = new MenuView();
        InitMainMenu();
        editForm.ContextMenu.Items.Add(this.CreateContextView("Operations", this));
    }

    [Label("Создать новый тип")]
    public void NewClass(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name is required");
        var next = new ClassModel() { Name = className.Trim() };
        EnsureClassModelInitialized(next);
        CopyClassModel(next, classModelBuilder.classModel);
        SelectedClassName = next.Name;
        editForm.BindEditControls(classModelBuilder.classModel);
    }

    [Label("Сохранить тип")]
    public void SaveClass()
    {
        EnsureClassModelInitialized(classModelBuilder.classModel);
        if (string.IsNullOrWhiteSpace(classModelBuilder.classModel.Name))
            throw new ArgumentException("Class name is required");
        using var db = new ClassDbContext();
        var existing = db.Classes.FirstOrDefault(c => c.Name == classModelBuilder.classModel.Name);
        if (existing is not null)
            DeleteClass(db, existing.Id);
        var classId = db.Create(new ClassModel()
        {
            Name = classModelBuilder.classModel.Name,
            ParentId = classModelBuilder.classModel.ParentId
        });
        foreach (var attr in classModelBuilder.classModel.Attributes ?? new())
        {
            db.Create(new AttributeBase<ClassModel>() { ParentId = classId, Name = attr.Name, Value = attr.Value });
        }
        foreach (var prop in classModelBuilder.classModel.Properties ?? new())
        {
            var propertyId = db.Create(new PropertyModel()
            {
                ParentId = classId,
                Name = prop.Name,
                Type = prop.Type,
                Value = prop.Value,
                Label = prop.Label,
                Description = prop.Description
            });
            foreach (var attr in prop.Attributes ?? new())
            {
                db.Create(new AttributeBase<PropertyModel>() { ParentId = propertyId, Name = attr.Name, Value = attr.Value });
            }
        }
        foreach (var method in classModelBuilder.classModel.Methods ?? new())
        {
            var methodId = db.Create(new MethodModel()
            {
                ParentId = classId,
                Name = method.Name,
                Label = method.Label,
                Description = method.Description
            });
            foreach (var attr in method.Attributes ?? new())
            {
                db.Create(new AttributeBase<MethodModel>() { ParentId = methodId, Name = attr.Name, Value = attr.Value });
            }
            foreach (var parameter in method.Parameters ?? new())
            {
                var parameterId = db.Create(new MethodParameterModel()
                {
                    ParentId = methodId,
                    Name = parameter.Name,
                    Type = parameter.Type
                });
                foreach (var attr in parameter.Attributes ?? new List<MethodParameterModelAttribute>())
                {
                    db.Create(new AttributeBase<MethodParameterModel>() { ParentId = parameterId, Name = attr.Name, Value = attr.Value });
                }
            }
        }
        SelectedClassName = classModelBuilder.classModel.Name;
        this.Info($"Class '{SelectedClassName}' saved");
    }

    [Label("Load class")]
    public void LoadClass(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            throw new ArgumentException("Class name is required");
        using var db = new ClassDbContext();
        var model = LoadClassModelGraph(db, className.Trim());
        if (model is null)
            throw new Exception($"Class '{className}' not found");
        EnsureClassModelInitialized(model);
        CopyClassModel(model, classModelBuilder.classModel);
        SelectedClassName = classModelBuilder.classModel.Name;
        editForm.BindEditControls(classModelBuilder.classModel);
    }

    [Label("Compile DbContext")]
    public void CompileDbContext()
    {
        if (string.IsNullOrWhiteSpace(DbContextName))
            throw new ArgumentException("DbContext name is required");
        if (string.IsNullOrWhiteSpace(BuildProjectName))
            throw new ArgumentException("Project name is required");
        using var db = new ClassDbContext();
        var classModels = db.Classes
            .Include(c => c.Attributes)
            .Include(c => c.Properties).ThenInclude(p => p.Attributes)
            .Include(c => c.Methods).ThenInclude(m => m.Attributes)
            .Include(c => c.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Attributes)
            .ToList();
        if (classModels.Count == 0)
            throw new Exception("No classes available for compilation");
        var builder = new ClassDbService(db) {   };
      
        
        compiledAssembly = Assembly.Load(builder.CompileDbContext(DbContextName, classModels));
        LastCompiledAssemblyPath = compiledAssembly.Location;
        this.Info($"Build completed: {LastCompiledAssemblyPath}");
    }

    [Label("Connect DbContext")]
    public void ConnectCompiledDbContext()
    {
        if (compiledAssembly is null)
        {
            if (string.IsNullOrWhiteSpace(LastCompiledAssemblyPath) || !File.Exists(LastCompiledAssemblyPath))
                throw new Exception("Compile DbContext first");
            compiledAssembly = Assembly.LoadFile(LastCompiledAssemblyPath);
        }
        ServiceFactory.Get().AddTypes(compiledAssembly);
        var dbContextType = compiledAssembly.GetTypes()
            .FirstOrDefault(t => !t.IsAbstract && typeof(DbContextBase).IsAssignableFrom(t) && t.GetTypeName() == DbContextName);
        if (dbContextType is null)
            throw new Exception($"DbContext type '{DbContextName}' was not found in assembly");
        using var db = dbContextType.New<DbContext>();
        db.Database.EnsureCreated();
        this.Info($"DbContext connected: {dbContextType.GetTypeName()}");
    }

    private void InitMainMenu()
    {
        ((MenuView)Top).AddMenuActions("Типы", new Dictionary<string, Action>()
        {
            { "Создать тип", ()=> NewClass($"Class{DateTime.Now:HHmmss}") },
            { "Сохранить тип", SaveClass },
            { "Загрузить тип", ()=> LoadClass(SelectedClassName) }
        });
        ((MenuView)Top).AddMenuActions("Базы данных", new Dictionary<string, Action>()
        {
            { "Собрать контекст данных", CompileDbContext },
            { "Присоединить модель данных",  ConnectCompiledDbContext }
        });
        ((MenuView)Top).AddMenuActions("Диаграммы", new Dictionary<string, Action>()
        {
            { "Диаграмма классов", ShowClassDiagram }
        });
    }

    private void ShowClassDiagram()
    {
        var customizer = new ElementsCustomizerView();
        customizer.ShowDialog();
        
    }

    private static void EnsureClassModelInitialized(ClassModel model)
    {
        model.Properties ??= new();
        model.Methods ??= new();
        model.Attributes ??= new();
        foreach (var property in model.Properties)
            property.Attributes ??= new();
        foreach (var method in model.Methods)
        {
            method.Attributes ??= new();
            method.Parameters ??= new();
            foreach (var parameter in method.Parameters)
                parameter.Attributes ??= new List<MethodParameterModelAttribute>();
        }
    }

    private static void CopyClassModel(ClassModel source, ClassModel target)
    {
        target.Id = source.Id;
        target.Name = source.Name;
        target.Label = source.Label;
        target.Description = source.Description;
        target.ParentId = source.ParentId;
        target.Attributes = (source.Attributes ?? new()).Select(a => new ClassModelAttribute() { Name = a.Name, Value = a.Value }).ToList();
        target.Properties = (source.Properties ?? new()).Select(p => new PropertyModel()
        {
            Name = p.Name,
            Type = p.Type,
            Value = p.Value,
            Label = p.Label,
            Description = p.Description,
            Attributes = (p.Attributes ?? new()).Select(a => new PropertyModelAttribute() { Name = a.Name, Value = a.Value }).ToList()
        }).ToList();
        target.Methods = (source.Methods ?? new()).Select(m => new MethodModel()
        {
            Name = m.Name,
            Label = m.Label,
            Description = m.Description,
            Attributes = (m.Attributes ?? new()).Select(a => new MethodModelAttribute() { Name = a.Name, Value = a.Value }).ToList(),
            Parameters = (m.Parameters ?? new()).Select(par => new MethodParameterModel()
            {
                Name = par.Name,
                Type = par.Type,
                Attributes = (par.Attributes ?? new List<MethodParameterModelAttribute>()).Select(a => new MethodParameterModelAttribute() { Name = a.Name, Value = a.Value }).ToList()
            }).ToList()
        }).ToList();
    }

    private static ClassModel LoadClassModelGraph(ClassDbContext db, string className)
    {
        return db.Classes
            .Include(c => c.Attributes)
            .Include(c => c.Properties).ThenInclude(p => p.Attributes)
            .Include(c => c.Methods).ThenInclude(m => m.Attributes)
            .Include(c => c.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Attributes)
            .FirstOrDefault(c => c.Name == className);
    }

    private static void DeleteClass(ClassDbContext db, int classId)
    {
        var propertyIds = db.Properties.Where(p => p.ParentId == classId).Select(p => p.Id).ToList();
        if (propertyIds.Count > 0)
            db.RemoveRange(db.PropertyAttributes.Where(a => a.ParentId.HasValue && propertyIds.Contains(a.ParentId.Value)).ToList());
        var methodIds = db.Methods.Where(m => m.ParentId == classId).Select(m => m.Id).ToList();
        if (methodIds.Count > 0)
        {
            db.RemoveRange(db.MethodAttributes.Where(a => a.ParentId.HasValue && methodIds.Contains(a.ParentId.Value)).ToList());
            var parameterIds = db.MethodParameters.Where(p => p.ParentId.HasValue && methodIds.Contains(p.ParentId.Value)).Select(p => p.Id).ToList();
            if (parameterIds.Count > 0)
                db.RemoveRange(db.MethodParameterAttributes.Where(a => a.ParentId.HasValue && parameterIds.Contains(a.ParentId.Value)).ToList());
            db.RemoveRange(db.MethodParameters.Where(p => parameterIds.Contains(p.Id)).ToList());
            db.RemoveRange(db.Methods.Where(m => methodIds.Contains(m.Id)).ToList());
        }
        db.RemoveRange(db.ClassAttributes.Where(a => a.ParentId == classId).ToList());
        db.RemoveRange(db.Properties.Where(p => p.ParentId == classId).ToList());
        var classRow = db.Classes.FirstOrDefault(c => c.Id == classId);
        if (classRow is not null)
            db.Remove(classRow);
        db.SaveChanges();
    }

    private List<string> GetClassNames()
    {
        using var db = new ClassDbContext();
        return db.Classes.Select(c => c.Name).OrderBy(n => n).ToList();
    }
}