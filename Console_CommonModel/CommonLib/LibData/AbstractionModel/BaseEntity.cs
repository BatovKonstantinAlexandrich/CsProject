using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Xml.Serialization;
using Npgsql.Internal.Postgres;
using Console_CommonModel.CommonLib.LibData.Console_DataModule;
using Console_CommonModel.CommonLib.Others;

public interface IObjectWithId<T>
{
    public T Id { get; set; }
}

namespace Console_CommonModel.CommonLib.LibData.AbstractionModel
{
    [Label("Общий класс для всех сущностей")]
    public class BaseEntity : MyValidatableObject, IChangeSupport, IObjectWithId<int>
    {

        /// <summary>
        /// 
        /// </summary>
        
        public void Inject(IServiceProvider serviceProvider)
        {
            foreach(var prop in this.GetProperties())
            {
                if(prop.GetPropertyAttributes().ContainsKey(nameof(ServiceInjectionAttribute)))
                {
                    prop.SetValue(this, serviceProvider.GetService(prop.PropertyType));
                }
            }
        }

        [Key]
        [InputHidden()]
        [NotInput]
        public virtual int Id { get; set; }


        [NotMapped]
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public bool Changed { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        [NotMapped]
        [NotInput]
        public ConcurrentDictionary<string, object> DataState { get; set; }

        [NotMapped]
        [NotInput]
        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        public bool ValidateOnInput { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        [JsonIgnore]
        [XmlIgnore]
        [NotMapped]
        [NotInput]
        public Action<PropertyChangedMessage> OnChange { get; set; } = (evt) => { };



        public BaseEntity() : base()
        {
        }


        public BaseEntity(IContainer container) : base(container)
        {

        }



        /// <summary>
        /// Создание новой записи в базе данных
        /// </summary>    
        public bool Create()
        {
            using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
            {
                db.Add(this);
                return db.SaveChanges() == 1;
            }
        }
        public bool Create<TDbContext>()
            where TDbContext : DbContext
        {
            using (var db = typeof(TDbContext).New<TDbContext>())
            {
                db.Add(this);
                return db.SaveChanges() == 1;
            }
        }


        /// <summary>
        /// Обновление всех данных в базе( в том числе и зависимых сущностней
        /// </summary>
        public bool UpdateAll()
        {
            this.EnsureIsValide();
            Update();
            using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
            {
                foreach (var name in GetType().GetNavigationPropertiesNames())
                {
                    var property = GetType().GetProperty(name);
                    if (property.PropertyType.IsCollectionType())
                    {
                        foreach (var item in (dynamic)this.GetValue(name))
                        {
                            db.Update(item);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        db.Update(this);
                        db.SaveChanges();
                    }
                }
                return true;

            }
        }


        /// <summary>
        /// Обновление данных в базе
        /// </summary>
        public bool Update()
        {
            using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
            {
                db.Update(this);
                return db.SaveChanges() == 1;
            }
        }

        public bool Update<TDbContext>()
            where TDbContext : DbContextBase
        {
            using (var db = typeof(TDbContext).New<TDbContext>())
            {
                db.Update(this);
                return db.SaveChanges() == 1;
            }
        }


        /// <summary>
        /// Возвращает коллекцию объектов с внешними ссылками на текущий
        /// </summary>
        public IEnumerable<TModel> GetReferenceCollection<TDbContext, TModel>() where TModel : BaseEntity
            where TDbContext: DbContextBase
        {
            bool debug = false;
            if (debug) InputConsole.Clear();

            this.Info($"Выполнение {GetType().GetTypeName()}.GetReferenceCollection<{typeof(TModel).GetTypeName()}>()");
            try
            {
                var navs = GetType().GetNavigationProperties< TDbContext>();
                if (debug) this.Info("navs: \n" + navs.Select(n => new { name = n.Name, entityType = n.ClrType.GetTypeName() }).ToJsonOnScreen());
                var propertyNames = GetType().GetProperties<List<TModel>>().Select(p => p.Name).ToList();
                if (debug) this.Info("navs: \n" + propertyNames.ToJsonOnScreen());
                if (propertyNames.Count() == 0)
                {
                    throw new ArgumentException($"В типе {GetType().GetTypeName()} не обнаружено свойство типа {typeof(TModel).GetTypeName()}");
                }
                if (propertyNames.Count() == 0)
                {
                    throw new ArgumentException($"В типе {GetType().GetTypeName()} свойство типа {typeof(TModel).GetTypeName()} не обнаружено свойства навигации типа {typeof(TModel).GetTypeName()}");
                }
                if (propertyNames.Count() != 1)
                {
                    throw new AmbiguousMatchException($"В типе {GetType().GetTypeName()} обнаружено несколько свойств типа {typeof(TModel).GetTypeName()}: \n {propertyNames.ToJsonOnScreen()}");
                }
                var relativeEntityPropertyName = propertyNames.First();
                if (debug) this.Info($"relativeEntityPropertyName={relativeEntityPropertyName}");

                object presult = this.GetValue(relativeEntityPropertyName);
                if (debug) this.Info($"presult={presult.ToJsonOnScreen()}");
                if (presult == null || ((List<TModel>)presult).Count() == 0)
                {
                    List<TModel> list = new();
                    var entityType = typeof(TModel);
                    if (debug) this.Info($"entityType={entityType.GetTypeName()}");
                    if (debug) this.Info($"properties={entityType.GetPropertyNames().ToJsonOnScreen()}");

                    var property = entityType.GetProperties().FirstOrDefault(p => p.PropertyType == GetType());
                    if (debug) this.Info($"property={property?.Name}");
                    var foreignKey = property.Name + "Id";
                    if (debug) this.Info($"foreignKey={foreignKey}");
                    using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
                    {
                        object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);
                        foreach (object item in (dynamic)dbset)
                        {
                            if (item.GetValue(foreignKey).ToString().ToInt() == this.GetValue("Id").ToString().ToInt())
                            {
                                list.Add((TModel)item);
                            }
                        }
                    }
                    if (debug) this.Info("list=\n" + list.ToJsonOnScreen());
                    this.SetValue(relativeEntityPropertyName, list);
                    return list;
                }
                return (List<TModel>)presult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Возвращает те ссылки которые не ссылаются на объект
        /// </summary>
        public IEnumerable<TModel> GetReferenceCollectionManyToMany<TDbContext, TMappingModel, TModel>() where TMappingModel : BaseEntity where TModel : BaseEntity
            where TDbContext : DbContextBase

        {
            var props = typeof(TMappingModel).GetProperties<TModel>();
            if (props.Count() == 0) throw new ArgumentException($"Тип {typeof(TMappingModel).GetTypeName()} не объявляет сссылку на {typeof(TModel).GetTypeName()}");
            if (props.Count() != 1) throw new ArgumentException($"Тип {typeof(TMappingModel).GetTypeName()} объявляет НЕСКОЛЬКО сссылок на {typeof(TModel).GetTypeName()}");
            var fk = props.Select(p => p.Name + "Id").FirstOrDefault();
            var map = GetReferenceCollection<TDbContext, TMappingModel>();
            this.Info($"{"UserGroups"}:\n" + map.ToJsonOnScreen());
            var ids = map.Select(p => p.GetValue(fk)).Where(p => p is not null).Select(p => p.ToString().ToInt()).ToList();
            this.Info($"{fk}:\n" + ids.ToJsonOnScreen());
            return typeof(TModel).GetEntityFasade<TModel>().Get(ids.ToArray());
        }

        public IEnumerable<TModel> GetReferenceCollectionManyToManyOptions<TDbContext, TMappingModel, TModel>() where TMappingModel : BaseEntity where TModel : BaseEntity
            where TDbContext: DbContextBase
        {
            var props = typeof(TMappingModel).GetProperties<TModel>();
            if (props.Count() == 0) throw new ArgumentException($"Тип {typeof(TMappingModel).GetTypeName()} не объявляет сссылку на {typeof(TModel).GetTypeName()}");
            if (props.Count() != 1) throw new ArgumentException($"Тип {typeof(TMappingModel).GetTypeName()} объявляет НЕСКОЛЬКО сссылок на {typeof(TModel).GetTypeName()}");
            var fk = props.Select(p => p.Name + "Id").FirstOrDefault();
            var map = GetReferenceCollection<TDbContext, TMappingModel>();
            //this.Info($"{"UserGroups"}:\n" + map.ToJsonOnScreen());
            var ids = map.Select(p => p.GetValue(fk)).Where(p => p is not null).Select(p => p.ToString().ToInt()).ToList();
            //this.Info($"{fk}:\n" + ids.ToJsonOnScreen());
            return typeof(TModel).GetEntityFasade<TModel>().GetAll().Where(p => !ids.Contains(p.Id));
        }



        /// <summary>
        /// Возвращает те ссылки которые не ссылаются на объект
        /// </summary>
        public IEnumerable<TModel> GetReferenceCollectionOptions<TDbContext, TModel>() where TModel : BaseEntity
            where TDbContext : DbContextBase

        {
            var navs = GetType().GetNavigationProperties< TDbContext>();
            //this.Info(navs.Select(n => new { name = n.Name, entityType = n.ClrType.GetTypeName() }).ToJsonOnScreen());
            var propertyNames = GetType().GetProperties<List<TModel>>().Select(p => p.Name).ToList();
            // this.Info(propertyNames.ToJsonOnScreen());
            if (propertyNames.Count() == 0)
            {
                throw new ArgumentException($"В типе {GetType().GetTypeName()} не обнаружено свойство типа {typeof(TModel).GetTypeName()}");
            }
            if (propertyNames.Count() == 0)
            {
                throw new ArgumentException($"В типе {GetType().GetTypeName()} свойство типа {typeof(TModel).GetTypeName()} не обнаружено свойства навигации типа {typeof(TModel).GetTypeName()}");
            }
            if (propertyNames.Count() != 1)
            {
                throw new AmbiguousMatchException($"В типе {GetType().GetTypeName()} обнаружено несколько свойств типа {typeof(TModel).GetTypeName()}: \n {propertyNames.ToJsonOnScreen()}");
            }
            var relativeEntityPropertyName = propertyNames.First();

            var list = (dynamic)typeof(List<>).MakeGenericType(typeof(TModel)).New();
            var entityType = typeof(TModel);
            entityType.GetProperties().Select(p => p.Name).ToJsonOnScreen().WriteToConsole();
            //this.Info(typeof(TModel).GetTypeName());
            var referenceToThisProperty = entityType.GetProperties().Where(p => p.PropertyType == GetType()).Select(p => p.Name).ToList();
            if (referenceToThisProperty.Count() == 0)
            {
                throw new Exception($"Тип {entityType} не объявляет ссылку на {GetType().GetTypeName()}");
            }
            var property = referenceToThisProperty.FirstOrDefault();
            var foreignKey = property + "Id";
            int id = this.GetValue("Id").ToString().ToInt();
            //this.Info($"{typeof(TModel).GetTypeName()}.{foreignKey} = {id}");
            using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
            {
                object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);
                foreach (object item in (dynamic)dbset)
                {
                    //this.Info(item.ToJson());
                    if (item.GetValue(foreignKey).ToString().ToInt() != id)
                    {
                        list.Add(item);
                    }
                }
            }

            return list;
        }


        /// <summary>
        /// Получение вторичной ссылки
        /// </summary>
        public TModel GetReference<TDbContext, TModel>()
            where TModel : BaseEntity
            where TDbContext : DbContextBase
        {
            var navs = GetType().GetNavigationProperties< TDbContext>();
            var propertyNames = navs.Where(p => p.ClrType == typeof(TModel)).Select(P => P.Name).ToList();
            if (GetType().GetPropertyNames().Where(p => GetType().GetProperty(p).PropertyType == typeof(TModel)).Count() == 0)
            {
                throw new ArgumentException($"В типе {GetType().GetTypeName()} не обнаружено свойство типа {typeof(TModel).GetTypeName()}");
            }
            if (propertyNames.Count() == 0)
            {
                throw new ArgumentException($"В типе {GetType().GetTypeName()} свойство типа {typeof(TModel).GetTypeName()} не обнаружено свойства навигации типа {typeof(TModel).GetTypeName()}");
            }
            if (propertyNames.Count() != 1)
            {
                throw new AmbiguousMatchException($"В типе {GetType().GetTypeName()} обнаружено несколько свойств типа {typeof(TModel).GetTypeName()}: \n {propertyNames.ToJsonOnScreen()}");
            }
            var relativeEntityPropertyName = propertyNames.First();
            var foreignKeyRelativeEntity = relativeEntityPropertyName + "Id";
            if (!GetType().GetPropertyNames().Contains(foreignKeyRelativeEntity))
            {
                throw new Exception($"В типе {GetType().GetTypeName()} не обнаружено свойство c именем: {foreignKeyRelativeEntity} для нешнего ключа");
            }

            object presult = this.GetValue(relativeEntityPropertyName);
            if (presult == null)
            {
                var entityType = typeof(TModel);
                using (var db = typeof(TDbContext).New<DbContext>())
                {

                    object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);

                    foreach (object item in (dynamic)dbset)
                    {
                        if (item.GetValue("Id").ToString().ToInt() == this.GetValue(foreignKeyRelativeEntity).ToString().ToInt())
                        {
                            presult = item;
                            this.SetValue(relativeEntityPropertyName, presult);
                            return (TModel)presult;
                        }
                    }
                }
            }
            return (TModel)presult;
        }


        public object GetReferenceByType(Type realtiveType)
        {
            return GetType().GetMethod(nameof(GetReference)).MakeGenericMethod(realtiveType).Invoke(this, new object[] { });
        }

        //public TModel GetReference<TModel>() where TModel : BaseEntity
        //{
        //    var navs = GetType().GetNavigationProperties();
        //    var propertyNames = navs.Where(p => p.ClrType == typeof(TModel)).Select(P => P.Name).ToList();
        //    if (GetType().GetPropertyNames().Where(p => GetType().GetProperty(p).PropertyType == typeof(TModel)).Count() == 0)
        //    {
        //        throw new ArgumentException($"В типе {GetType().GetTypeName()} не обнаружено свойство типа {typeof(TModel).GetTypeName()}");
        //    }
        //    if (propertyNames.Count() == 0)
        //    {
        //        throw new ArgumentException($"В типе {GetType().GetTypeName()} свойство типа {typeof(TModel).GetTypeName()} не обнаружено свойства навигации типа {typeof(TModel).GetTypeName()}");
        //    }
        //    if (propertyNames.Count() != 1)
        //    {
        //        throw new AmbiguousMatchException($"В типе {GetType().GetTypeName()} обнаружено несколько свойств типа {typeof(TModel).GetTypeName()}: \n {propertyNames.ToJsonOnScreen()}");
        //    }
        //    var relativeEntityPropertyName = propertyNames.First();
        //    var foreignKeyRelativeEntity = relativeEntityPropertyName + "Id";
        //    if (!GetType().GetPropertyNames().Contains(foreignKeyRelativeEntity))
        //    {
        //        throw new Exception($"В типе {GetType().GetTypeName()} не обнаружено свойство c именем: {foreignKeyRelativeEntity} для нешнего ключа");
        //    }

        //    object presult = this.GetValue(relativeEntityPropertyName);
        //    if (presult == null)
        //    {
        //        var entityType = typeof(TModel);
        //        using (var db = GetType().GetDbContextWithEntityType().New<DbContext>())
        //        {

        //            object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);

        //            foreach (object item in (dynamic)dbset)
        //            {
        //                if (item.GetValue("Id").ToString().ToInt() == this.GetValue(foreignKeyRelativeEntity).ToString().ToInt())
        //                {
        //                    presult = item;
        //                    this.SetValue(relativeEntityPropertyName, presult);
        //                    return (TModel)presult;
        //                }
        //            }
        //        }
        //    }
        //    return (TModel)presult;
        //}


        public bool Is<TType>() => GetType().IsExtendsFrom(typeof(TType));
        public TType Cast<TType>() =>
            Is<TType>() ? (TType)MemberwiseClone() :
            throw new Exception($"Объект типа {GetType().Name} не возможно привети к типу {typeof(TType).Name}");
        public TType Convert<TType>() =>
            Is<TType>() ? (TType)MemberwiseClone() :
            this.ToJson().FromJson<TType>();


        /// <summary>
        /// Возвращает названия свойств с определённых как коллекции классов сущностей BaseEntity
        /// и не помеченые атрибутом NotMappedAttribute.
        /// </summary>
        /// <returns></returns>
        public List<string> GetManyRelationProperties()
        {
            var entityType = GetType();
            return entityType.GetProperties().Where(p =>
                p.PropertyType.IsCollectionType() &&
                !entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
               p.PropertyType.GetValueType().ToType().IsExtendsFrom(typeof(BaseEntity))
            ).Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Возвращает названия свойств с определённых как коллекции классов сущностей BaseEntity
        /// и не помеченые атрибутом NotMappedAttribute.
        /// </summary>
        /// <returns></returns>
        public List<string> GetOneRelationProperties()
        {
            var entityType = GetType();
            return entityType.GetProperties().Where(p =>
                !entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                p.PropertyType.IsExtendsFrom(typeof(BaseEntity)) &&
                entityType.GetProperties().Any(pp => pp.Name == p.Name + "Id")
            
            ).Select(p => p.Name).ToList();
        }

        /// <summary>
        /// Возвращает названия свойств типа коллекций.
        /// </summary>
        /// <returns></returns>
        public List<string> GetManyNotMappedProperties()
        {
            var entityType = GetType();
            return entityType.GetProperties().Where(p =>
                p.PropertyType.IsCollectionType() &&
                entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
               p.PropertyType.GetValueType().ToType().IsExtendsFrom(typeof(BaseEntity))
            ).Select(p => p.Name).ToList();
        }



        /// <summary>
        /// Возвращает названия свойств с определённых как коллекции классов сущностей BaseEntity
        /// и не помеченые атрибутом NotMappedAttribute.
        /// </summary>
        /// <returns></returns>
        public List<EntityRelations> GetEntityRelations()
        {
            List<EntityRelations> relations = new();
            var entityType = GetType();

            var manyRelations = entityType.GetProperties().Where(p =>
                p.PropertyType.IsCollectionType() &&
                !entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
               p.PropertyType.GetValueType().ToType().IsExtendsFrom(typeof(BaseEntity))
            ).Select(p => new
            {
                PropertyName = p.Name,
                RelativeEntityType = p.PropertyType.GetValueType().ToType(),
            }).ToList();


            foreach (var relation in manyRelations)
            {

                var r = relation.RelativeEntityType.GetProperties().Where(p =>
                    !relation.RelativeEntityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                    p.PropertyType.IsExtendsFrom(typeof(BaseEntity)) &&
                    relation.RelativeEntityType.GetProperties().Any(pp => pp.Name == p.Name + "Id")
                );
                var _oneRelations = r.Select(p => new
                {
                    PropertyName = p.Name,
                    RelativeEntityType = p.PropertyType
                }).ToList(); ;
                if (_oneRelations.Any(r => r.RelativeEntityType == entityType))
                {
                    relations.Add(new EntityRelations
                    {
                        EntityTypeName = entityType.Name,
                        RelativeEntityType = relation.RelativeEntityType.Name,
                        PropertyName = relation.PropertyName,
                        ForeignKey = _oneRelations.FirstOrDefault(r => r.RelativeEntityType == entityType).PropertyName + "Id",
                        RelationshipType = "many-to-one"
                    });
                }
                else
                {
                    var message = $"Сущность {entityType.GetLabel()}[{entityType.GetTypeName()}] объявляет множественную связь с " +
                        $"{relation.RelativeEntityType.GetLabel()}[{relation.RelativeEntityType.GetTypeName()}], но сущность {relation.RelativeEntityType.GetLabel()}[{relation.RelativeEntityType.GetTypeName()}]" +
                        $" не обявляет связи с ней.";
                    this.Warn(message);
                    throw new DataModelException(message);
                }
            }

            var oneRelations = entityType.GetProperties().Where(p =>
                !entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                p.PropertyType.IsExtendsFrom(typeof(BaseEntity)) &&
                entityType.GetProperties().Any(pp => pp.Name == p.Name + "Id")
            
            ).Select(p => new
            {
                PropertyName = p.Name,
                RelativeEntityType = p.PropertyType
            }).ToList();
            this.Info(oneRelations.Select(p => p.PropertyName));

            foreach (var relation in oneRelations)
            {
                var r = relation.RelativeEntityType.GetProperties().Where(p =>
                    !relation.RelativeEntityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                    !relation.RelativeEntityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(ManyToManyAttribute)) &&
                    p.PropertyType.IsExtendsFrom(typeof(BaseEntity)) &&
                    relation.RelativeEntityType.GetProperties().Any(pp => pp.Name == p.Name + "Id")
                );
                var _oneRelations = r.Select(p => new
                {
                    PropertyName = p.Name,
                    RelativeEntityType = p.PropertyType
                }).ToList();
                if (_oneRelations.Any(r => r.RelativeEntityType == entityType))
                {
                    relations.Add(new EntityRelations
                    {
                        EntityTypeName = entityType.Name,
                        RelativeEntityType = relation.RelativeEntityType.Name,
                        PropertyName = relation.PropertyName,
                        ForeignKey = _oneRelations.FirstOrDefault(r => r.RelativeEntityType == entityType).PropertyName + "Id",
                        RelationshipType = "one-to-one"
                    });
                }
                else
                {

                    var rtype = relation.RelativeEntityType;
                    var rManyRealtions = rtype.GetProperties().Where(p =>
                        p.PropertyType.IsCollectionType() &&
                        !rtype.GetPropertyAttributes(p.Name).ContainsKey(nameof(ManyToManyAttribute)) &&
                        !rtype.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                       p.PropertyType.GetValueType().ToType().IsExtendsFrom(typeof(BaseEntity))
                    ).Select(p => new
                    {
                        PropertyName = p.Name,
                        RelativeEntityType = p.PropertyType.GetValueType().ToType(),
                    }).ToList();

                    if (rManyRealtions.Any(r => r.RelativeEntityType == entityType))
                    {
                        var e = rManyRealtions.FirstOrDefault(r => r.RelativeEntityType == entityType);
                        var fk = e.PropertyName + "Id";
                        relations.Add(new EntityRelations
                        {
                            EntityTypeName = entityType.Name,
                            RelativeEntityType = relation.RelativeEntityType.Name,
                            PropertyName = relation.PropertyName,
                            ForeignKey = fk,
                            RelationshipType = "one-to-many"
                        });
                    }
                    else
                    {

                        relations.Add(new EntityRelations
                        {
                            EntityTypeName = entityType.Name,
                            RelativeEntityType = relation.RelativeEntityType.Name,
                            PropertyName = relation.PropertyName,
                            ForeignKey = null,
                            RelationshipType = "one-to-many"
                        });
                    }

                }
            }
            var manyToManyTargets = entityType.GetProperties().Where(p =>
                p.PropertyType.IsCollectionType() &&
                entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) && p.PropertyType.GetValueType() is not null &&
               p.PropertyType.GetValueType().ToType().IsExtendsFrom(typeof(BaseEntity))
            ).Select(p => p.Name).ToList();
            foreach (var pname in manyToManyTargets)
            {
                var collectionProperty = entityType.GetProperties().FirstOrDefault(p =>
                    entityType.GetPropertyAttributes(p.Name).ContainsKey(nameof(NotMappedAttribute)) &&
                    entityType.GetPropertyAttributes(p.Name).First(kv => kv.Key == nameof(NotMappedAttribute)).Value == pname
                );
                if (collectionProperty is not null)
                {
                    var connectionEntity = collectionProperty.PropertyType.GetValueType().ToType();

                    var targetEntityType = entityType.GetProperty(pname).PropertyType.GetValueType().ToType();
                    var entityProperty = connectionEntity.GetPropertiesByType(entityType).FirstOrDefault();

                    var props = connectionEntity.GetPropertiesByType(targetEntityType);


                    if (props.Count() == 0)
                    {

                    }
                    else
                    {

                    }

                }
            }




            return relations;

        }


        public void JoinData<TDbContext>() where TDbContext : DbContext
        {
            var firstUser = this;
            var dataType = firstUser.GetType();
            var navs = dataType.GetNavigation<TDbContext>();
            this.Info(dataType.GetTypeName() + " " + navs.Count());
            foreach (INavigation nav in navs)
            {
                //firstUser.Info(nav.Name);               
                if (nav.ClrType.IsImplements(typeof(IEnumerable)))
                {
                    var entityType = nav.ClrType.GetGenericArguments()[0];
                    string typeName = entityType.GetTypeName();

                    Type typeType = typeName.ToType();
                    var fasade = typeType.GetEntityFasadeForType();

                    string fk = "Id";
                    if (!firstUser.GetType().GetProperties().Select(p => p.Name).Contains(fk))
                    {
                        throw new Exception($"Свойство {fk} в типе {firstUser.GetType().GetTypeName()}");
                    }

                    var list = typeof(List<>).MakeGenericType(entityType).New<IList>();
                    firstUser.SetValue(nav.Name, list);


                    int fid = firstUser.GetValue("Id").ToString().ToInt();
                    using (var db = typeof(TDbContext).New<DbContext>())
                    {

                        object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);
                        var mainType = GetType();
                        db.Info(mainType.GetTypeName());
                        var all = entityType.GetPropertiesWithType(mainType);
                        db.Info(all.Select(p => p.Name).ToJson());
                        if (all.Count() == 0)
                        {
                            throw new Exception($"На удалось найти ссылку на {GetType().GetTypeName()} в типе {entityType.GetTypeName()}");
                        }
                        var fkProperty = all.First().Name + "Id";
                        if (entityType.GetProperties().FirstOrDefault(p => p.Name == fkProperty) == null)
                        {
                            throw new Exception($"На удалось найти свойство {fkProperty} внешнего ключа для {GetType().GetTypeName()} в типе {entityType.GetTypeName()}");
                        }
                        foreach (object item in (dynamic)dbset)
                        {
                            try
                            {
                                if ($"{item.GetValue(fkProperty)}" == $"{fid}")
                                {
                                    if (item.GetType() != entityType)
                                    {
                                        Console.WriteLine($"{item.GetType()}{entityType}");
                                    }
                                    list.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Error(ex);
                            }
                        }
                    }
                    firstUser.Info(entityType.GetTypeName() + " " + nav.Name + "=" + fk + $".Where(p => p.{nav.Inverse?.Name}==" + fid + ")");
                }
                else
                {
                    string typeName = nav.ClrType.GetTypeName();
                    //firstUser.Info($"Тип {typeName}");
                    Type entityType = typeName.ToType();


                    dynamic fkFasade = typeof(EntityExtensions).GetMethods().First(m => m.Name == "GetEntityFasade" && m.GetGenericArguments().Count() == 1).MakeGenericMethod(entityType).Invoke(null, new object[] { entityType });
                    string fk = nav.Name + "Id";
                    if (GetType().GetProperties().FirstOrDefault(p => p.Name == fk) == null)
                    {
                        this.Info(typeof(UserContext).GetProperties().Select(p => p.Name).ToList().ToJsonOnScreen());
                        throw new Exception($"На удалось найти свойство {fk} внешнего ключа в типе {GetType().GetTypeName()}");
                    }
                    object fid = firstUser.GetValue(fk) is null ? null : firstUser.GetValue(fk);
                    if (fid is null)
                    {
                        firstUser.SetValue(nav.Name, null);
                        continue;
                    }
                    //firstUser.Info(typeName + " " + nav.Name + "=" + fk + "==" + fid);
                    object target = null;
                    target = fkFasade.Get(fid.ToString().ToInt());

                    //firstUser.Info($"{(target is not null? target.ToJsonOnScreen(): "")}");
                    firstUser.SetValue(nav.Name, target);
                    //firstUser.Info($"Выполнен JOIN {nav.FieldInfo.Name}: \n{target.ToJsonOnScreen()}");
                }
            }
        }
        public void JoinAll<TDBContext>()
            where TDBContext: DbContextBase
        {
            var firstUser = this;
            var dataType = firstUser.GetType();
            foreach (INavigation nav in dataType.GetNavigationProperties<TDBContext>())
            {
                //firstUser.Info(nav.Name);               
                if (nav.ClrType.IsImplements(typeof(IEnumerable)))
                {
                    var entityType = nav.ClrType.GetGenericArguments()[0];
                    string typeName = entityType.GetTypeName();

                    Type typeType = typeName.ToType();
                    var fasade = typeType.GetEntityFasadeForType();

                    string fk = "Id";
                    if (!firstUser.GetType().GetProperties().Select(p => p.Name).Contains(fk))
                    {
                        throw new Exception($"Свойство {fk} в типе {firstUser.GetType().GetTypeName()}");
                    }

                    var list = typeof(List<>).MakeGenericType(entityType).New<IList>();
                    firstUser.SetValue(nav.Name, list);


                    int fid = firstUser.GetValue("Id").ToString().ToInt();
                    using (var db = entityType.GetDbContextWithEntityType().New<DbContext>())
                    {

                        object dbset = db.GetType().GetMethods().First(f => f.Name == "Set").MakeGenericMethod(entityType).Invoke(db, new object[0]);
                        var mainType = GetType();
                        db.Info(mainType.GetTypeName());
                        var all = entityType.GetPropertiesWithType(mainType);
                        db.Info(all.Select(p => p.Name).ToJson());
                        if (all.Count() == 0)
                        {
                            throw new Exception($"На удалось найти ссылку на {GetType().GetTypeName()} в типе {entityType.GetTypeName()}");
                        }
                        var fkProperty = all.First().Name + "Id";
                        if (entityType.GetProperties().FirstOrDefault(p => p.Name == fkProperty) == null)
                        {
                            throw new Exception($"На удалось найти свойство {fkProperty} внешнего ключа для {GetType().GetTypeName()} в типе {entityType.GetTypeName()}");
                        }
                        foreach (object item in (dynamic)dbset)
                        {
                            try
                            {
                                if ($"{item.GetValue(fkProperty)}" == $"{fid}")
                                {
                                    if (item.GetType() != entityType)
                                    {
                                        Console.WriteLine($"{item.GetType()}{entityType}");
                                    }
                                    list.Add(item);
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Error(ex);
                            }
                        }
                    }
                    firstUser.Info(entityType.GetTypeName() + " " + nav.Name + "=" + fk + $".Where(p => p.{nav.Inverse?.Name}==" + fid + ")");
                }
                else
                {
                    string typeName = nav.ClrType.GetTypeName();
                    //firstUser.Info($"Тип {typeName}");
                    Type entityType = typeName.ToType();


                    dynamic fkFasade = typeof(EntityExtensions).GetMethods().First(m => m.Name == "GetEntityFasade" && m.GetGenericArguments().Count() == 1).MakeGenericMethod(entityType).Invoke(null, new object[] { entityType });
                    string fk = nav.Name + "Id";
                    if (GetType().GetProperties().FirstOrDefault(p => p.Name == fk) == null)
                    {
                        this.Info(typeof(UserContext).GetProperties().Select(p => p.Name).ToList().ToJsonOnScreen());
                        throw new Exception($"На удалось найти свойство {fk} внешнего ключа в типе {GetType().GetTypeName()}");
                    }
                    object fid = firstUser.GetValue(fk) is null ? null : firstUser.GetValue(fk);
                    if (fid is null)
                    {
                        firstUser.SetValue(nav.Name, null);
                        continue;
                    }
                    //firstUser.Info(typeName + " " + nav.Name + "=" + fk + "==" + fid);
                    object target = null;
                    target = fkFasade.Get(fid.ToString().ToInt());

                    //firstUser.Info($"{(target is not null? target.ToJsonOnScreen(): "")}");
                    firstUser.SetValue(nav.Name, target);
                    //firstUser.Info($"Выполнен JOIN {nav.FieldInfo.Name}: \n{target.ToJsonOnScreen()}");
                }
            }
        }

        internal void Delete()
        {
        }



        public void ApplyPropertyChangedEvent(PropertyChangedMessage evt)
        {
        }



        public void RemovePropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
        }

        public void AddPropertyChangeListener(Action<PropertyChangedMessage> listener)
        {
        }

        public void AddPropertyChangeListener(string property, Action<PropertyChangedMessage> listener)
        {
        }

        public T Get<T>(string key)
        {
            return this.GetValue<T>(key);
        }

        public void OnGet<T>(string key, T value)
        {
        }

        public void Set<T>(string key, T value)
        {
        }

        public bool WasChanged()
        {
            throw new NotImplementedException();
        }

        public void AddPropertyChangeListener<T>(string property, Action<T> changed)
        {
            throw new NotImplementedException();
        }

        public void Bind(string propertyName, IChangeSupport target, string relativePropertyName)
        {
            throw new NotImplementedException();
        }

        internal object GetOwnPropertiesValues()
        {
            throw new NotImplementedException();
        }
    }

}

/// <summary>
/// Исключение пробрасывается при наличии ошибках в модели данных.
/// Это исключение говорит о том что создание невозможно создать
/// структуру данных по модели.
/// </summary>
[Serializable]
internal class DataModelException : Exception
{
    public DataModelException()
    {
    }

    public DataModelException(string message) : base(message)
    {
    }

    public DataModelException(string message, Exception innerException) : base(message, innerException)
    {
    }
}