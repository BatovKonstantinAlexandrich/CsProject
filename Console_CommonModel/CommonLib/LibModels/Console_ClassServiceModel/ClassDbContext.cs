
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.LibModels.Console_ClassDataModel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Console_CommonModel.CommonLib.LibModels.Console_ClassServiceModel
{


    /// <summary>
    /// Контекст данные по типам, моделям, сущностям
    /// </summary>
    public class ClientClassDbContext : ClassDbContext
    {
        public ClientClassDbContext()
        {
        }

        public ClientClassDbContext(DbContextOptions options) : base(options)
        {
        }
    }
    public class ClassDbContext : DbContextBase
    {
        public virtual DbSet<AssemblyModel> Assemblies { get; set; }
        public virtual DbSet<ClassModel> Classes { get; set; }
        public virtual DbSet<PropertyModel> Properties { get; set; }
        public virtual DbSet<MethodModel> Methods { get; set; }
        public virtual DbSet<MethodParameterModel> MethodParameters { get; set; }
        public virtual DbSet<AssemblyModelAttribute> AssemblyModelAttributes { get; set; }
        public virtual DbSet<ClassModelAttribute> ClassAttributes { get; set; }
        public virtual DbSet<PropertyModelAttribute> PropertyAttributes { get; set; }
        public virtual DbSet<MethodModelAttribute> MethodAttributes { get; set; }
        public virtual DbSet<MethodParameterModelAttribute> MethodParameterAttributes { get; set; }


        [NotMapped]
        private ClassDbService classDbContextService;

        public ClassDbContext()
        {
            classDbContextService = new(this);
        }

        public ClassDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Data Source=MicroserviceConnector.db");
                optionsBuilder.UseSqlServer($@"Data Source=DESKTOP-IHJM9RD;Initial Catalog={GetType().Name};Integrated Security=True;Connect Timeout=30;Encrypt=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            }
        }

        public void ClearClassModel()
        {
            classDbContextService.ClearClassModel();
        }

        public ClassModel CreateAndSaveClassModel(Type type)
        {
            return classDbContextService.CreateAndSaveClassModel(type);
        }

        public ClassModel GetClassModel(int id)
        {
            return classDbContextService.GetClassModel(id);
        }

        public ClassModel GetClassModel(Type type)
        {
            return classDbContextService.GetClassModel(type);
        }



        public void RemoveClassModel(Type type) => classDbContextService.RemoveClassModel(type);

        public ClassModel CreateClassModel(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            var typeName = type.GetTypeName();
            try
            {                
                var existing = LoadClassModelGraph(typeName);
                if (existing is not null)
                    return existing;

                int? parentId = null;
                var baseType = type.BaseType;
                if (baseType is not null && baseType != typeof(object) && !baseType.IsPrimitive)
                    parentId = CreateClassModel(baseType).Id;

                var source = TypeHelperStrict.GetOrCreateClassModel(type);

                var classModelRec = new ClassModel
                {
                    Name = source.Name,
                    Label = source.Label,
                    Description = source.Description,
                    ParentId = parentId,
                };
                Classes.Add(classModelRec);
                SaveChanges();

                PersistClassAttributes(classModelRec.Id, source.Attributes);

                foreach (var prop in source.Properties)
                    PersistProperty(classModelRec.Id, prop);

                foreach (var method in source.Methods)
                    PersistMethod(classModelRec.Id, method);

                return LoadClassModelGraph(typeName) ?? classModelRec;
            }
            catch(Exception ex)
            {
                throw new Exception($"Не удалось создать модель типа {typeName}", ex);
            }
        }
        private void PersistProperty(int classId, PropertyModel source)
        {
            var property = new PropertyModel
            {
                ParentId = classId,
                Name = source.Name,
                Type = source.Type,
                Label = source.Label,
                Description = source.Description,
                Value = source.Value,
            };
            Properties.Add(property);
            SaveChanges();

            foreach (var attr in source.Attributes.GroupBy(a => a.Name).Select(g => g.Last()))
            {
                PropertyAttributes.Add(new PropertyModelAttribute
                {
                    ParentId = property.Id,
                    Name = attr.Name,
                    Value = attr.Value ?? string.Empty,
                });
            }
            SaveChanges();
        }

        private void PersistMethod(int classId, MethodModel source)
        {
            var method = new MethodModel
            {
                ParentId = classId,
                Name = source.Name,
                Label = source.Label,
                Description = source.Description,
                Icon = source.Icon,
                Help = source.Help,
                Type = source.Type,
                Body = source.Body,
            };
            Methods.Add(method);
            SaveChanges();

            foreach (var attr in source.Attributes.GroupBy(a => a.Name).Select(g => g.Last()))
            {
                MethodAttributes.Add(new MethodModelAttribute
                {
                    ParentId = method.Id,
                    Name = attr.Name,
                    Value = attr.Value ?? string.Empty,
                });
            }
            SaveChanges();

            foreach (var sourceParam in source.Parameters)
            {
                var parameter = new MethodParameterModel
                {
                    ParentId = method.Id,
                    Name = sourceParam.Name,
                    Type = sourceParam.Type,
                    Label = sourceParam.Label,
                    Description = sourceParam.Description,
                    Help = sourceParam.Help,
                    Value = sourceParam.Value,
                };
                MethodParameters.Add(parameter);
                SaveChanges();

                foreach (var attr in sourceParam.Attributes.GroupBy(a => a.Name).Select(g => g.Last()))
                {
                    MethodParameterAttributes.Add(new MethodParameterModelAttribute
                    {
                        ParentId = parameter.Id,
                        Name = attr.Name,
                        Value = attr.Value ?? string.Empty,
                    });
                }
                SaveChanges();
            }
        }
        private void PersistClassAttributes(int classId, IEnumerable<AttributeBase<ClassModel>> attributes)
        {
            foreach (var attr in attributes.GroupBy(a => a.Name).Select(g => g.Last()))
            {
                ClassAttributes.Add(new ClassModelAttribute
                {
                    ParentId = classId,
                    Name = attr.Name,
                    Value = attr.Value ?? string.Empty,
                });
            }
            SaveChanges();
        }
        private ClassModel LoadClassModelGraph(string typeName)
            => Classes
                .Include(c => c.Attributes)
                .Include(c => c.Properties).ThenInclude(p => p.Attributes)
                .Include(c => c.Methods).ThenInclude(m => m.Attributes)
                .Include(c => c.Methods).ThenInclude(m => m.Parameters).ThenInclude(p => p.Attributes)
                .FirstOrDefault(c => c.Name == typeName);
    }
}
