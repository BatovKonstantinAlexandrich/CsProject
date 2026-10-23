using ClassModelLib.ClassServiceModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp.Console.Console_ClassModule.ClassDataModel;



namespace WinFormsApp.Console.Console_ClassModule.ClassServiceModel
{

    public class ClientClassDbContextService : IClassDbContextService
    {

        private readonly ClientClassDbContext classDbContext;
        private readonly DbContextCompiler classDbContextCompiler = new();


        public ClientClassDbContextService(ClientClassDbContext classDbContext)
        {
            this.classDbContext = classDbContext;
        }


        public byte[] CompileDbContext(string dbContextName, IEnumerable<ClassModel> classes)
        {            
            return classDbContextCompiler.CompileDll(dbContextName, classes);
        }

        private IEnumerable<ClassModel> GetExtendedClasses(ClassModel classModel)
        {
            List<ClassModel> results = new();
            if(classModel.ParentId is not null)
            {
                var parentClassModel = GetClassModelById((int)classModel.ParentId);
                results.AddRange(GetExtendedClasses(parentClassModel));
            }
            results.Add(classModel);
            return results;
        }

        public ClassModel GetClassModel(Type type)
        {
            try
            {
                var classModel = classDbContext.Classes
                        .Include(c => c.Parent)
                        .Include(c => c.Attributes)
                        .Include(c => c.Properties)
                        .ThenInclude(p => p.Attributes)
                        .Include(c => c.Methods)
                        .ThenInclude(p => p.Attributes)
                        .Include(p => p.Methods)
                        .ThenInclude(p => p.Parameters)
                        .ThenInclude(p => p.Attributes)
                        .FirstOrDefault(c => c.Name == type.GetTypeName());
                return classModel;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении метода GetClassModel({type.Name}) => {ex.Message}");
            }
        }

        public ClassModel GetClassModelById(int id)
        {
            try
            {
                var classModel = classDbContext.Classes
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
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении метода GetClassModelById({id}) => {ex.Message}");
            }
        }


        public ClassModel GetClassModel(int id)
        {
            try
            {
                var classModel = classDbContext.Classes
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
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при выполнении метода GetClassModel({id}) => {ex.Message}");
            }
        }


        public ClassModel CreateAndSaveClassModel(Type type)
        {
            TypeHelper typeHelper = new();
            var db = this;
            var model = GetClassModel(type);
            if (model is not null)
                return model;
            var classModelRec = new ClassModel()
            {
                Name = type.GetTypeName()
            };
            classDbContext.Classes.Add(classModelRec);
            classDbContext.SaveChanges();
            var classId = classModelRec.Id;

            var attributes = typeHelper.GetAttributes(type);
            foreach (var attr in attributes.Where(a => a.Key != "NullableAttribute" && a.Key != "NullableContextAttribute").Select(attr => new ClassModelAttribute()
            {
                ParentId = classId,
                Name = attr.Key,
                Value = attr.Value
            }))
            {
                //attr.Validate();
                classDbContext.Add(attr);
                classDbContext.SaveChanges();
            }

            foreach (var prop in type.GetOwnPropertyNames().Select(prop => new PropertyModel()
            {
                ParentId = classId,
                Name = prop,
                Type = type.GetProperties().FirstOrDefault(p => p.Name == prop).PropertyType.GetTypeName(),
                Label = prop,
                Description = prop
            }))
            {
                if (prop.Type == "Int32")
                    prop.Type = "int";
                classDbContext.Add(prop);
                classDbContext.SaveChanges();
                var propertyId = prop.Id;
                var propertyAttributes = typeHelper.GetPropertyAttributes(type, prop.Name);
                foreach (var attr in propertyAttributes.Select(attr => new PropertyModelAttribute()
                {
                    ParentId = propertyId,
                    Name = attr.Key,
                    Value = attr.Value
                }))
                {
                    classDbContext.Add(attr);
                    classDbContext.SaveChanges();
                }
            }
            foreach (var method in type.GetMethods().Where(m => !m.Name.StartsWith("get_") && !m.Name.StartsWith("set_") && !m.IsStatic && m.DeclaringType.Name == type.Name).Select(m => m.Name).Select(method => new MethodModel()
            {
                ParentId = classId,
                Name = method,
                Label = method,
                Description = method
            }))
            {
                method.Type = type.GetMethods().FirstOrDefault(m => m.Name == method.Name).ReturnType.GetTypeName();
                this.Info(classDbContext.Methods.Where(m => m.ParentId == classId).Count());
                classDbContext.Add(method);
                classDbContext.SaveChanges();
                var methodId = method.Id;

                var methodAttributes = typeHelper.GetMethodAttributes(type, method.Name);
                foreach (var attr in methodAttributes.Select(attr => new MethodModelAttribute()
                {
                    ParentId = methodId,
                    Name = attr.Key,
                    Value = attr.Value
                }))
                {
                    classDbContext.Add(attr);
                    classDbContext.SaveChanges();
                }
                foreach (var par in type.GetMethods().FirstOrDefault(m => m.Name == method.Name).GetParameters())
                {
                    var mpm = new MethodParameterModel()
                    {
                        ParentId = methodId,
                        Name = par.Name,
                        Type = par.ParameterType.GetTypeName()
                    };

                    classDbContext.Add(mpm);
                    classDbContext.SaveChanges();
                    var parameterId = mpm.Id;
                    var parameterAttributes = typeHelper.GetParameterAttributes(par);
                    mpm.Label = parameterAttributes.ContainsKey("LabelAttribute") ?
                        parameterAttributes.First(m => m.Key == "LabelAttribute").Value : par.Name;

                    foreach (var attr in parameterAttributes.Select(attr => new MethodParameterModelAttribute()
                    {
                        ParentId = parameterId,
                        Name = attr.Key,
                        Value = attr.Value
                    }))
                    {
                        classDbContext.Add(attr);
                        classDbContext.SaveChanges();
                    }
                }
            }
            if (type.BaseType != typeof(Object) && !type.IsPrimitive)
            {
                if(type.BaseType.IsGenericType && !type.BaseType.IsGenericTypeDefinition)
                {
                    var gtype = type.BaseType.GetGenericTypeDefinition();
                    if(!classDbContext.Classes.Any(cl => cl.Name == gtype.GetTypeName()))
                    {
                        CreateAndSaveClassModel(gtype);
                    }
                }
                var pcl = classDbContext.Classes.FirstOrDefault(cl => cl.Name == type.BaseType.GetTypeName());
                if (pcl is null)
                {
                    pcl = CreateAndSaveClassModel(type.BaseType);
                }
                if (pcl is not null)
                {
                    classModelRec.ParentId = pcl.Id;
                }
            }
            var classModel = classDbContext.Classes
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
            if (type.BaseType != typeof(object))
            {
                classModel.ParentId = CreateAndSaveClassModel(type.BaseType).Id;
            }
            classDbContext.SaveChanges();
            return classModel;
        }

        public void ClearClassModel()
        {
            classDbContext.Database.EnsureDeleted();
            classDbContext.Database.EnsureCreated();
        }

        /// <summary>Удаление модели класса по идентификатору (вместе с вложенными сущностями).</summary>
        public void RemoveClassModelById(int classId)
        {
            var classModel = classDbContext.Classes.FirstOrDefault(c => c.Id == classId);
            if (classModel is null)
                return;
            RemoveClassModelCore(classModel.Id);
        }

        internal void RemoveClassModel(Type type)
        {
            if (type is null)
            {
                return;
            }

            var className = type.Name;
            var classModel = classDbContext.Classes.FirstOrDefault(c => c.Name == className);
            if (classModel is null)
            {
                return;
            }

            var classId = classModel.Id;
            RemoveClassModelCore(classId);
        }

        private void RemoveClassModelCore(int classId)
        {

            // If this class is a parent for other class models, unlink children first.
            var childClasses = classDbContext.Classes.Where(c => c.ParentId == classId).ToList();
            foreach (var child in childClasses)
            {
                child.ParentId = null;
            }

            var propertyIds = classDbContext.Properties
                .Where(p => p.ParentId == classId)
                .Select(p => p.Id)
                .ToList();
            if (propertyIds.Count > 0)
            {
                var propertyAttributes = classDbContext.PropertyAttributes
                    .Where(a => a.ParentId.HasValue && propertyIds.Contains(a.ParentId.Value))
                    .ToList();
                if (propertyAttributes.Count() > 0)
                {
                    classDbContext.RemoveRange(propertyAttributes);
                }
            }

            var methodIds = classDbContext.Methods
                .Where(m => m.ParentId == classId)
                .Select(m => m.Id)
                .ToList();
            if (methodIds.Count() > 0)
            {
                var methodAttributes = classDbContext.MethodAttributes
                    .Where(a => a.ParentId.HasValue && methodIds.Contains(a.ParentId.Value))
                    .ToList();
                if (methodAttributes.Count() > 0)
                {
                    classDbContext.RemoveRange(methodAttributes);
                }

                var parameterIds = classDbContext.MethodParameters
                    .Where(p => p.ParentId.HasValue && methodIds.Contains(p.ParentId.Value))
                    .Select(p => p.Id)
                    .ToList();
                if (parameterIds.Count > 0)
                {
                    var parameterAttributes = classDbContext.MethodParameterAttributes
                        .Where(a => a.ParentId.HasValue && parameterIds.Contains(a.ParentId.Value))
                        .ToList();
                    if (parameterAttributes.Count() > 0)
                    {
                        classDbContext.RemoveRange(parameterAttributes);
                    }

                    var methodParameters = classDbContext.MethodParameters
                        .Where(p => parameterIds.Contains(p.Id))
                        .ToList();
                    if (methodParameters.Count > 0)
                    {
                        classDbContext.RemoveRange(methodParameters);
                    }
                }

                var methods = classDbContext.Methods
                    .Where(m => methodIds.Contains(m.Id))
                    .ToList();
                if (methods.Count > 0)
                {
                    classDbContext.RemoveRange(methods);
                }
            }

            var classAttributes = classDbContext.ClassAttributes
                .Where(a => a.ParentId == classId)
                .ToList();
            if (classAttributes.Count > 0)
            {
                classDbContext.RemoveRange(classAttributes);
            }

            var properties = classDbContext.Properties
                .Where(p => p.ParentId == classId)
                .ToList();
            if (properties.Count > 0)
            {
                classDbContext.RemoveRange(properties);
            }

            var toRemove = classDbContext.Classes.FirstOrDefault(c => c.Id == classId);
            if (toRemove != null)
                classDbContext.Remove(toRemove);
            classDbContext.SaveChanges();
        }
    }
}
